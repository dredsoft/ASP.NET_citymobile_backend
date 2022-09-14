using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Models;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Api.Models.Citation;
using CityApp.Data.Models;
using System.Threading.Tasks;
using System;
using CityApp.Common.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CityApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using CityApp.Data.Enums;

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{accountNum:long}/Citations")]
    public class CitationsController : BaseAccountApiController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<CitationsController>();
        private CommonContext _commonCtx;
        private AccountContext _accountCtx;
        private readonly FileService fileService;
        private IHostingEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly CitationService _citationSvc;

        public CitationsController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, AccountContext accountCtx, FileService _fileService, IHostingEnvironment environment, CitationService citationSvc) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
            fileService = _fileService;
            _environment = environment;
            _appSettings = appSettings.Value;
            _citationSvc = citationSvc;
            _commonCtx = commonContext;
        }

        /// <summary>
        /// Get All Citation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Get")]
        public async Task<IActionResult> Get([FromBody] CitationModel model)
        {
            var response = new APIResponse<CitationModel>() { Success = true };
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var citations = _accountCtx.Citations
                .Include(x => x.Violation)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments).Include(m => m.CreateUser)
                .Include(x => x.Attachments).ThenInclude(m => m.Attachment)
                .OrderByDescending(x => x.CreateUtc)
                .ForAccount(CommonAccount.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.CreatedFrom))
            {
                citations = citations.Where(s => s.CreateUtc >= (Convert.ToDateTime(model.CreatedFrom).UTCToAccountLocalTime(CommonAccount.CityTimeZone)));
            }
            if (!string.IsNullOrWhiteSpace(model.CreatedTo))
            {
                citations = citations.Where(s => s.CreateUtc <= (Convert.ToDateTime(model.CreatedTo).UTCToAccountLocalTime(CommonAccount.CityTimeZone)));
            }
            if (model.CreatedById != null)
            {
                citations = citations.Where(s => s.CreateUserId == model.CreatedById);
            }
            if (!string.IsNullOrWhiteSpace(model.LicensePlate))
            {
                citations = citations.Where(s => s.LicensePlate == model.LicensePlate);
            }

            var totalQueryCount = await citations.CountAsync();
            var results = await citations.Skip(offset).Take(model.PageSize).ToListAsync();
            var data = Mapper.Map<List<CitationListModel>>(results);
            model.CitationList = data;
            model.CurrentPage = currentPageNum;
            model.ItemsPerPage = model.PageSize;
            model.TotalItems = totalQueryCount;
            response.Data = model;
            response.Success = true;
            return Ok(response);
        }

        /// <summary>
        /// Save Citation 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CitationRequestModel model)
        {
            var response = new APIResponse<CitationResponseModel>() { Success = true };
            try
            {
                //Check Disabled user
                bool activeUser = _commonCtx.UserAccounts.Where(x => x.Disabled == false && x.AccountId== CommonAccount.Id && x.UserId == User.GetJWTLoggedInUserId().Value).Any();
                if (activeUser)
                {
                    if (ModelState.IsValid)
                    {
                        long citatioNextNumber = 1000000;
                        bool citatoinExists = false;
                        var citatoinNum = _accountCtx.Counters.ForAccount(CommonAccount.Id).Select(x => x).Where(x => x.Name == "Citations").SingleOrDefault();
                        if (citatoinNum != null)
                        {
                            citatioNextNumber = citatoinNum.NextValue;
                        }
                        var timeSpan = DateTime.UtcNow.AddMinutes(_appSettings.CitationTimeSpan);

                        //Only check to see if a citation exists if there is License Plan and VIN

                        if (!string.IsNullOrWhiteSpace(model.LicensePlate) && !string.IsNullOrWhiteSpace(model.VinNumber))
                        {
                            citatoinExists = _accountCtx.Citations.ForAccount(CommonAccount.Id)
                                .Where(x => x.ViolationId == model.ViolationId)
                                .Where(x => (x.LicensePlate == model.LicensePlate || x.VinNumber == model.VinNumber) && x.CreateUtc >= timeSpan).Any();
                        }
                        else if (string.IsNullOrWhiteSpace(model.LicensePlate) && !string.IsNullOrWhiteSpace(model.VinNumber))
                        {
                            citatoinExists = _accountCtx.Citations.ForAccount(CommonAccount.Id)
                                .Where(x => x.ViolationId == model.ViolationId)
                                .Where(x => x.VinNumber == model.VinNumber && x.CreateUtc >= timeSpan).Any();
                        }
                        else if (!string.IsNullOrWhiteSpace(model.LicensePlate) && string.IsNullOrWhiteSpace(model.VinNumber))
                        {
                            citatoinExists = _accountCtx.Citations.ForAccount(CommonAccount.Id)
                                .Where(x => x.ViolationId == model.ViolationId)
                                .Where(x => x.LicensePlate == model.LicensePlate && x.CreateUtc >= timeSpan).Any();
                        }

                        if (!citatoinExists)
                        {

                            var citation = Mapper.Map<Citation>(model);
                            if (model.ViolationId != null || model.ViolationId != Guid.Empty)
                            {
                                //get Violation that are disabled or that is related to passed accountid
                                var violations = await _accountCtx.Violations
                                    .ForAccount(CommonAccount.Id)
                                    .Where(m => m.Id == model.ViolationId).SingleOrDefaultAsync();

                                if (violations != null)
                                {
                                    citation.FineAmount = violations.Fee;
                                }
                            }



                            citation.Status = await SetStatus();
                            citation.AccountId = CommonAccount.Id;
                            citation.CitationNumber = citatioNextNumber;
                            citation.CreateUtc = DateTime.UtcNow;
                            citation.CreateUserId = User.GetJWTLoggedInUserId().Value;
                            citation.UpdateUserId = User.GetJWTLoggedInUserId().Value;

                            _accountCtx.Citations.Add(citation);

                            if (citatoinNum == null)
                            {
                                Counter count = new Counter()
                                {
                                    AccountId = CommonAccount.Id,
                                    Name = "Citations",
                                    NextValue = citatioNextNumber + 1,
                                    CreateUserId = User.GetJWTLoggedInUserId().Value,
                                    UpdateUserId = User.GetJWTLoggedInUserId().Value
                                };
                                _accountCtx.Counters.Add(count);
                            }
                            else
                            {
                                citatoinNum.NextValue = citatioNextNumber + 1;
                                citatoinNum.UpdateUserId = User.GetJWTLoggedInUserId().Value;
                                _accountCtx.Counters.Update(citatoinNum);
                            }

                            await _accountCtx.SaveChangesAsync();
                            var citationResponse = Mapper.Map<CitationResponseModel>(citation);
                            response.Data = citationResponse;


                            //Create Audit Log
                            //TODO, this should also be created in the background. 
                            await _citationSvc.CreateAuditEvent(citation.AccountId, citation.Id, "Citation Created from mobile device.", citation.CreateUserId, CitationAuditEvent.NewCitation);

                            //Reverse Geo Code
                            //We want to to run this in the background.  We don't have to use await here. 
                            _citationSvc.ReverseGeoCodeCitation(citation.AccountId, citation.Id);

                            //Create a citation reminder in the background
                            _citationSvc.OverrideCitationFee(citation.AccountId, citation.Id);

                            //Override Fee

                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "A citation for this vehicle has already been submitted";

                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.AddRange(ModelState.ToErrors());
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "This submission was not created because you have been disabled from this account.";

                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return Ok(response);
            }
        }

        private async Task<CitationStatus> SetStatus()
        {
            var defaultStatus = CitationStatus.Open;

            var accountUser = await CommonContext.UserAccounts.Where(m => m.AccountId == CommonAccount.Id && m.UserId == User.GetJWTLoggedInUserId().Value).SingleAsync();

            if (accountUser.Permissions.HasFlag(AccountPermissions.CitationApprover))
            {
                defaultStatus = CitationStatus.Approved;
            }

            return defaultStatus;
        }


        /// <summary>
        /// Save Attachment Detail after upload to AWS3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/Attachment")]
        public async Task<IActionResult> Attachment([FromBody] AttachmentRequestModel model)
        {
            var response = new APIResponse<AttachmentResponseModel>() { Success = true };
            AttachmentResponseModel responseModel = new AttachmentResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var check = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking().AnyAsync(x => x.Id == model.CitationId);
                    if (check)
                    {
                        foreach (var attachment in model.Attachments)
                        {
                            // put dude in the database
                            Attachment attach = new Attachment()
                            {
                                AccountId = CommonAccount.Id,
                                ContentLength = attachment.Length,
                                MimeType = attachment.MimeType,
                                FileName = attachment.FileName,
                                Description = attachment.Description,
                                Key = attachment.Key,
                                Duration = attachment.Duration,
                                AttachmentType = attachment.FileName.GetAttachmentType(),
                                CreateUserId = User.GetJWTLoggedInUserId().Value,
                                UpdateUserId = User.GetJWTLoggedInUserId().Value
                            };
                            var citationAttach = await SaveAttachments(attach, model.CitationId);
                            var obj = Mapper.Map<AttachmentResponse>(citationAttach);
                            responseModel.Response.Add(obj);

                        }
                        //Create Receipt
                        await _citationSvc.CreateCitationReceipt(CommonAccount.Id, model.CitationId, model.DeviceReceipt, model.DevicePublicKey, User.GetJWTLoggedInUserId().Value);
                        response.Data = responseModel;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error { Code = 0, Message = "Invalid Citation Id " });
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(ModelState.ToErrors());
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);

        }



        /// <summary>
        /// Upload multiple Files on AWS Directly
        /// </summary>
        /// <param name="files"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files, Guid id)
        {
            var response = new APIResponse<Attachment>() { Success = true };
            try
            {
                // Get File Type from Appsetting
                List<string> Types = _appSettings.FileTypes.Split(',').ToList();


                var check = await _accountCtx.Citations.ForAccount(CommonAccount.Id).Select(x => x).Where(x => x.Id == id).SingleOrDefaultAsync();
                if (check != null)
                {

                    foreach (var formFile in files)
                    {
                        //Get current file extension
                        string ext = System.IO.Path.GetExtension(formFile.FileName).ToLower();

                        if (Types.Contains(ext))
                        {
                            if (formFile.Length > 0)
                            {
                                Guid fileGuid = Guid.NewGuid();
                                using (var fileStream = formFile.OpenReadStream())
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    //convert image into Bytes
                                    var fileByte1s = ms.ToArray();
                                    var fileNameInFolder = $"accounts/{CommonAccount.Number}/citations/{check.CitationNumber}/{fileGuid + ext}";

                                    //Upload the file on Amazon S3 Bucket
                                    var result = await fileService.UploadFile(fileByte1s, fileNameInFolder, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey);
                                    if (result)
                                    {
                                        // put dude in the database
                                        Attachment attach = new Attachment()
                                        {
                                            AccountId = CommonAccount.Id,
                                            ContentLength = formFile.Length,
                                            MimeType = formFile.ContentType,
                                            FileName = formFile.FileName,
                                            Key = fileNameInFolder,
                                            AttachmentType = CitationAttachmentType.Video,
                                            CreateUserId = User.GetJWTLoggedInUserId().Value,
                                            UpdateUserId = User.GetJWTLoggedInUserId().Value
                                        };
                                        await SaveAttachments(attach, id);
                                    }
                                }
                                response.Message = "File uploaded successfully. ";
                            }
                        }
                        else
                        {
                            response.Errors.Add(new Error { Code = 0, Message = $"Invalid File Formate {formFile.FileName}." });

                        }
                    }

                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error { Code = 0, Message = "Invalid Citation Id " });
                }

            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Errors.Add(new Error { Code = 0, Message = ex.Message });
            }

            return Ok(response);
        }

        /// <summary>
        /// Save Attachment (Common method)
        /// </summary>
        /// <param name="attach"></param>
        /// <param name="CitationId"></param>
        /// <returns></returns>
        public async Task<CitationAttachment> SaveAttachments(Attachment attach, Guid CitationId)
        {
            _accountCtx.Attachments.Add(attach);

            CitationAttachment citationAttach = new CitationAttachment()
            {
                AccountId = CommonAccount.Id,
                AttachmentId = attach.Id,
                CitationId = CitationId,
                CreateUserId = User.GetJWTLoggedInUserId().Value,
                UpdateUserId = User.GetJWTLoggedInUserId().Value
            };

            _accountCtx.CitationAttachments.Add(citationAttach);
            await _accountCtx.SaveChangesAsync();
            return citationAttach;
        }

        /// <summary>
        /// Save and Update Citation Comment
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/Comment")]
        public async Task<IActionResult> Comments([FromBody] CommentRequestModel model, Guid id)
        {

            var response = new APIResponse<CommentResponseModel>() { Success = true };
            try
            {
                if (ModelState.IsValid)
                {

                    var check = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking().AnyAsync(x => x.Id == id);
                    if (check)
                    {
                        var user = await _accountCtx.AccountUsers.AsNoTracking().AnyAsync(x => x.Id == model.CreatedUserId);
                        if (user)
                        {
                            var result = await _citationSvc.SaveComment(CommonAccount.Id, id, model.CommentId, model.Comment, model.IsPublic, model.CreatedUserId);
                            response.Message = "Comment Saved Successfully.";
                        }
                        else
                        {
                            response.Success = false;
                            response.Errors.Add(new Error { Code = 0, Message = "Invalid Created User Id" });
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error { Code = 0, Message = "Invalid Citation Id " });
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(ModelState.ToErrors());
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        /// <summary>
        /// Save and Update Citation Comment
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}/Comment")]
        public async Task<IActionResult> Comments(Guid id)
        {

            var response = new APIResponse<List<CitationCommentModel>>() { Success = true };
            try
            {

                var citationComments = await _accountCtx.CitationComment.Include(m => m.CreateUser).AsNoTracking().Where(m => m.CitationId == id).ToListAsync();

                response.Data = AddS3URLToImageProfile(Mapper.Map<List<CitationCommentModel>>(citationComments));

                response.Success = true;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        private List<CitationCommentModel> AddS3URLToImageProfile(List<CitationCommentModel> comments)
        {
            comments.ForEach(m =>
            {
                if (!string.IsNullOrWhiteSpace(m.CreateUserProfileImageKey))
                {
                    m.CreateUserProfileImageKey = AppSettings.AmazonS3Url + m.CreateUserProfileImageKey;
                }
            });

            return comments;
        }

    }
}
