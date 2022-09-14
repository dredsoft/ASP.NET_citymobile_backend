using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CityApp.Services;
using CityApp.Data;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;
using CityApp.Web.Models.Citations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using CityApp.Common.Extensions;
using CityApp.Web.Models;
using CityApp.Common.Utilities;
using CityApp.Web.Filters;
using CityApp.Data.Models;
using CityApp.Services.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.ComponentModel;
using CityApp.Data.Extensions;
using System.Text;

namespace CityApp.Web.Controllers
{
    public class CitationsController : AccountBaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<CitationsController>();
        private readonly CitationService _citationSvc;
        private AccountContext _accountCtx;
        private readonly MailService _mailSvc;
        private CommonContext _commonContext;
        private readonly FileService _fileService;
        private readonly TicketWorkFlowService _ticketWorkFlowService;
        private readonly ViolationService _violationSvc;
        private readonly AppSettings _appSettings;

        public CitationsController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CitationService citationSvc, MailService mailSvc, FileService fileService, TicketWorkFlowService ticketWorkFlowService, ViolationService violationSvc)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _citationSvc = citationSvc;
            _accountCtx = accountCtx;
            _mailSvc = mailSvc;
            _commonContext = commonContext;
            _fileService = fileService;
            _appSettings = appSettings.Value;
            _ticketWorkFlowService = ticketWorkFlowService;
            _violationSvc = violationSvc;
        }

        /// <summary>
        /// getting all citation list
        /// </summary>
        /// <param name="CreatedFrom"></param>
        /// <param name="Createdto"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedToId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(CitationsListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var citations = ApplyCitationFilter(model);

            switch (model.SortOrder)
            {
                case "Status":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Status);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Status);
                    }
                    break;
                case "Violation":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Violation.Name);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Violation.Name);
                    }
                    break;
                case "AssignedTo":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.AssignedTo);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.AssignedTo);
                    }
                    break;
                case "Created":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
                case "License":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.LicensePlate);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.LicensePlate);
                    }
                    break;
                case "Number":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CitationNumber);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CitationNumber);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
            }

            #region dropdown lists

            var assignList = await _accountCtx.Citations.ForAccount(CommonAccount.Id).Where(c => c.AssignedToId != null).Select( m => new {Text = m.AssignedTo.Email,Value = m.AssignedToId.ToString()}).Distinct().ToListAsync();

            model.AssignedToList.AddRange(assignList.Distinct().Select(m => new SelectListItem {Text = m.Text, Value = m.Value  }));

            var violations = await _violationSvc.GetCachedAccountViolations(CommonAccount.Id);

            if (model.ViolationTypeId.HasValue)
            {
                violations = violations.Where(m => m.ViolationTypeId == model.ViolationTypeId).ToList();
            }

            var violationList = violations.Select(m => new SelectListItem
            {
                Text = m.ViolationName,
                Value = m.ViolationId.ToString()
            });
            model.Violations.AddRange(violationList);

            #endregion

            var totalQueryCount = await citations.CountAsync();
            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(await citations.Skip(offset).Take(model.PageSize).ToListAsync());


            foreach (var m in model.CitationsListItem)
            {
                m.CreatedHumanizerDate = m.Created.Humanize();
            }

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;
            model.IsVehicleRelatedType = await IsVehicleRelatedType(model.ViolationTypeId);

            return View(model);
        }

        private IQueryable<Citation> ApplyCitationFilter(CitationsListViewModel model)
        {
            var citations = _accountCtx.Citations.ForAccount(CommonAccount.Id)
                           .Include(x => x.Violation).ThenInclude(m => m.Category).ThenInclude(m => m.Type)
                           .Include(x => x.AssignedTo)
                           .OrderByDescending(x => x.CreateUtc)
                           .AsQueryable();


            if (model.CreatedFrom.HasValue)
            {
                citations = citations.Where(s => s.CreateUtc >= model.CreatedFrom.Value.Floor());
            }
            if (model.CreatedTo.HasValue)
            {
                citations = citations.Where(s => s.CreateUtc <= model.CreatedTo.Value.Ceiling());
            }
            if (model.StatusId.HasValue)
            {
                citations = citations.Where(s => s.Status == model.StatusId);
            }
            if (model.AssignedToId.HasValue)
            {
                citations = citations.Where(s => s.AssignedToId == model.AssignedToId);
            }
            if (!string.IsNullOrWhiteSpace(model.LicensePlate))
            {
                citations = citations.Where(s => s.LicensePlate.Contains(model.LicensePlate));
            }
            if (model.ViolationTypeId.HasValue)
            {
                citations = citations.Where(s => s.Violation.Category.TypeId == model.ViolationTypeId);
            }
            if (model.ViolationId.HasValue)
            {
                citations = citations.Where(s => s.ViolationId == model.ViolationId);
            }
            if (!string.IsNullOrEmpty(model.PostalCode))
            {
                citations = citations.Where(s => s.PostalCode.Contains(model.PostalCode));
            }
            if (!string.IsNullOrEmpty(model.Street))
            {
                citations = citations.Where(s => s.Street.Contains(model.Street));
            }



            return citations;
        }

        /// <summary>
        /// get citation details by citation id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Citation(Guid id, string message, bool e = false)
        {
            var model = new CitationViolationListItem() { IsEdit = e };

            if (!string.IsNullOrEmpty(message))
            {
                model.UploadMessage = Cryptography.Decrypt(message);
            }
            else
            {
                model.UploadMessage = string.Empty;
            }

            var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking()
                            .Include(m => m.CreateUser)
                            .Include(m => m.AssignedTo)
                            .Include(m => m.Violation)
                            .Include(m => m.Comments).ThenInclude(m => m.CreateUser)
                            .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                            .Include(m => m.Attachments).ThenInclude(m => m.CreateUser)
                            .Include(m => m.AuditLogs).ThenInclude(m => m.CreateUser)
                            .SingleOrDefaultAsync(s => s.Id == id);

            Mapper.Map(citation, model);

            ApplyVideoAndImageUrl(citation, model,_fileService);

            var assignList = await _commonContext.UserAccounts.Include(x => x.User).Where(m => m.AccountId == CommonAccount.Id).Select(m => new SelectListItem
            {
                Text = m.User.Email,
                Value = m.UserId.ToString()
            }).ToListAsync();
            model.AssignedToList.AddRange(assignList);

            var violationList = await _accountCtx.Violations.Where(m => m.AccountId == CommonAccount.Id).Select(m => new SelectListItem
            {
                Text = m.Code == null ? m.Name : m.Code + " " + m.Name,
                Value = m.Id.ToString()
            }).ToListAsync();

            model.ViolationList.AddRange(violationList);
            model.ViolationId = citation.ViolationId;


            model.DisplayDate = model.Date.UTCToAccountLocalTime(CommonAccount.CityTimeZone).ToString("MM/dd/yyyy HH:mm tt");


            var statesList = await _commonContext.States.OrderBy(s => s.Name).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Name
            }).ToListAsync();

            model.States.AddRange(statesList);
            return View(model);
        }

        [RequiresPermission(AccountPermissions.CitationEdit)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        [Route("{accountNum}/Citations/Citation/{id}")]
        public async Task<IActionResult> Citation(CitationViolationListItem model)
        {
            var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking()
                                              .Include(m => m.CreateUser)
                                              .Include(m => m.Violation)
                                              .Include(m => m.Comments).ThenInclude(m => m.CreateUser)
                                              .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                                              .Include(m => m.Attachments).ThenInclude(m => m.CreateUser)
                                              .Include(m => m.AuditLogs).ThenInclude(m => m.CreateUser)
                                              .SingleOrDefaultAsync(s => s.Id == model.Id);
            if (citation == null)
            {
                throw new Exception("Invalid Ticket");
            }
            else
            {
                if (model.ViolationCustomRequiredFields.HasFlag(ViolationRequiredFields.VehicleInformation))
                {
                    if (IsValidCitation(citation, model))
                    {
                        var onlyCitation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking().SingleOrDefaultAsync(s => s.Id == model.Id);
                        bool IsUpdated = await UpdateCitation(onlyCitation, model);


                        return RedirectToAction("Citation", new { accountNum = CommonAccount.Number, Id = citation.Id, Message = Cryptography.Encrypt("Updated") });
                    }
                    else
                    {

                        var originalModel = model;
                        Mapper.Map(citation, model);

                        ApplyVideoAndImageUrl(citation, model,_fileService);

                        var assignList = await _commonContext.UserAccounts.Include(x => x.User).Where(m => m.AccountId == CommonAccount.Id).Select(m => new SelectListItem
                        {
                            Text = m.User.Email,
                            Value = m.UserId.ToString()
                        }).ToListAsync();
                        model.AssignedToList.AddRange(assignList);

                        var violationList = await _accountCtx.Violations.Where(m => m.AccountId == CommonAccount.Id).Select(m => new SelectListItem
                        {
                            Text = m.Code == null ? m.Name : m.Code + " " + m.Name,
                            Value = m.Id.ToString()
                        }).ToListAsync();

                        model.ViolationList.AddRange(violationList);
                        model.ViolationId = citation.ViolationId;
                        model.IsEdit = true;
                        model.LicensePlate = originalModel.LicensePlate;


                        return View(model);
                    }

                }
                else
                {
                    var onlyCitation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).AsNoTracking().SingleOrDefaultAsync(s => s.Id == model.Id);
                    bool IsUpdated = await UpdateCitation(onlyCitation, model);


                    return RedirectToAction("Citation", new { accountNum = CommonAccount.Number, Id = citation.Id, Message = Cryptography.Encrypt("Updated") });

                }


            }
        }


        /// <summary>
        /// Update Citation
        /// </summary>
        /// <param name="citation"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<bool> UpdateCitation(Citation citation, CitationViolationListItem model)
        {
            citation.LicensePlate = model.LicensePlate;
            citation.VehicleMake = model.VehicleMake;
            citation.VehicleModel = model.VehicleModel;
            citation.VehicleType = model.VehicleType;
            citation.VinNumber = model.VinNumber;
            citation.VehicleColor = model.VehicleColor;
            citation.ViolationId = model.ViolationId;
            citation.Description = model.Description;
            citation.LocationDescription = model.LocationDescription;
            citation.UpdateUserId = LoggedInUser.Id;
            citation.UpdateUtc = DateTime.UtcNow;
            citation.LicenseState = model.LicenseState;
            citation.FineAmount = model.FineAmount;
           // citation.CreateUtc = (Convert.ToDateTime(model.DisplayDate)).ToUniversalTime();

            _accountCtx.Citations.Update(citation);
            await _accountCtx.SaveChangesAsync();

            //Create Audit Log
            await _citationSvc.CreateAuditEvent(CommonAccount.Id, citation.Id, $"Ticket details have been updated.", User.GetLoggedInUserId().Value, CitationAuditEvent.DetailsUpdated);


            return true;
        }



        [RequiresPermission(AccountPermissions.CitationEdit)]
        [HttpPost]
        public async Task<IActionResult> DeleteCitation(Guid id)
        {

            var result = new ServiceResponse<Citation>() { Success = true };
            try
            {

                var citation = await _accountCtx.Citations
                                             .Include(m => m.CreateUser)
                                             .Include(m => m.Violation)
                                             .Include(m => m.Comments).ThenInclude(m => m.CreateUser)
                                             .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                                             .Include(m => m.Attachments).ThenInclude(m => m.CreateUser)
                                             .Include(m => m.AuditLogs).ThenInclude(m => m.CreateUser)
                                             .SingleOrDefaultAsync(s => s.Id == id);

                if (citation != null)
                {
                    foreach (var attach in citation.Attachments)
                    {
                        var cityAppFile = await _fileService.DeleteFile(attach.Attachment.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                    }

                    result = await _citationSvc.DeleteCitation(citation, id);

                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = true;

            }
            return Json(result);

        }


        private bool IsValidCitation(Citation citation, CitationViolationListItem model)
        {
            if (citation.Violation.RequiredFields.HasFlag(ViolationRequiredFields.VehicleInformation))
            {
                if (string.IsNullOrWhiteSpace(model.LicensePlate) && string.IsNullOrWhiteSpace(model.VinNumber))
                {
                    ModelState.AddModelError(nameof(model.LicensePlate), "License Plate or Vin is required");
                    ModelState.AddModelError(nameof(model.VinNumber), "License Plate or Vin is required");
                }
            }

            return ModelState.ErrorCount == 0;
        }


        /// <summary>
        /// update citation status
        /// </summary>
        /// <param name="accountNum"></param>
        /// <param name="statusId"></param>
        /// <param name="citationId"></param>
        /// <returns></returns>
        [HttpPost]
        [RequiresPermission(AccountPermissions.CitationEdit)]
        public async Task<IActionResult> UpdateCitationStatus(long accountNum, CitationStatus statusId, Guid citationId, string Reason)
        {
            var result = new ServiceResponse<Citation>();
            try
            {
                var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).Include(x => x.Violation).SingleOrDefaultAsync(m => m.Id == citationId);

                var fromStatus = citation.Status.GetName();
                citation.Status = statusId;
                var toStatus = citation.Status.GetName();

                await _accountCtx.SaveChangesAsync();
                result.Success = true;
                result.Data = null;

                //Create Audit Log
                await _citationSvc.CreateAuditEvent(CommonAccount.Id, citationId, $"Update Ticket Status from {fromStatus} to {toStatus} .", User.GetLoggedInUserId().Value, CitationAuditEvent.StatusChange);

                if (citation.CreateUserId != User.GetLoggedInUserId().Value)
                {
                    //Get Assigned User Email
                    var email = await _accountCtx.AccountUsers.Where(x => x.Id == citation.CreateUserId).Select(x => x.Email).SingleOrDefaultAsync();

                    var callbackUrl = Url.Action("Citation", "Citations", new { Id = citationId });
                    bool mailResult = await _mailSvc.SendStatusChangedEmail(email, email, callbackUrl, Reason, statusId.GetName(), citation.CitationNumber, citation.Violation.Name, citation.CreateUtc.UTCToAccountLocalTime(CommonAccount.CityTimeZone));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);

        }

        /// <summary>
        /// update citation assigned user
        /// </summary>
        /// <param name="assignToId"></param>
        /// <param name="citationId"></param>
        /// <returns></returns>
        [HttpPost]
        [RequiresPermission(AccountPermissions.CitationEdit)]
        public async Task<IActionResult> UpdateCitationAssignTo(Guid assignToId, Guid citationId)
        {
            var result = new ServiceResponse<Citation>();
            try
            {

                var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).Include(x => x.Violation).Include(x => x.AssignedTo).SingleOrDefaultAsync(m => m.Id == citationId);
                citation.AssignedToId = assignToId;
                //var email = citation.AssignedTo.Email;
                await _accountCtx.SaveChangesAsync();
                result.Success = true;
                result.Data = null;

                //Get Assigned User Email
                var email = await _accountCtx.AccountUsers.Where(x => x.Id == assignToId).Select(x => x.Email).SingleOrDefaultAsync();

                //Create Audit Log                
                await _citationSvc.CreateAuditEvent(CommonAccount.Id, citationId, $"Ticket Assigned to {email}.", User.GetLoggedInUserId().Value, CitationAuditEvent.Assignment);

                //TODO:  Please do not use Uppercase variable names.  Variables should be lower camel case.
                //It gets confusing when some are upper and others are lower.

                var callbackUrl = Url.Action("Citation", "Citations", new { Id = citationId });
                bool mailResult = await _mailSvc.SendAssignUserInvitationEmail(email, email, callbackUrl, citation.CitationNumber, citation.Violation.Name, citation.CreateUtc.UTCToAccountLocalTime(CommonAccount.CityTimeZone));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);

        }

        /// <summary>
        /// unassign the assigned user
        /// </summary>
        /// <param name="citationId"></param>
        /// <returns></returns>
        [HttpPost]
        [RequiresPermission(AccountPermissions.CitationEdit)]
        public async Task<IActionResult> UnAssignUser(Guid citationId)
        {
            var result = new ServiceResponse<Citation>();
            try
            {

                var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id).SingleOrDefaultAsync(m => m.Id == citationId);
                citation.AssignedToId = null;

                await _accountCtx.SaveChangesAsync();
                result.Success = true;
                result.Data = citation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);

        }

        /// <summary>
        /// create heat map
        /// </summary>
        /// <param name="CreatedFrom"></param>
        /// <param name="Createdto"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedTo"></param>
        /// <returns></returns>
        public async Task<IActionResult> HeatMaps(CitationsListViewModel model)
        {

            var citations = ApplyCitationFilter(model);

            var assignList = await _accountCtx.Citations.Where(c => c.AssignedToId != null).Select(m => new SelectListItem
            {
                Text = m.AssignedTo.Email,
                Value = m.AssignedToId.ToString()
            }).ToListAsync();

            model.AssignedToList.AddRange(assignList);
            model.AssignedToId = model.AssignedToId;
            model.IsVehicleRelatedType = await IsVehicleRelatedType(model.ViolationTypeId);

            var violations = await _violationSvc.GetCachedAccountViolations(CommonAccount.Id);

            if (model.ViolationTypeId.HasValue)
            {
                violations = violations.Where(m => m.ViolationTypeId == model.ViolationTypeId).ToList();
            }

            var violationList = violations.Select(m => new SelectListItem
            {
                Text = m.ViolationName,
                Value = m.ViolationId.ToString()
            });
            model.Violations.AddRange(violationList);

            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(await citations.ToListAsync());

            return View(model);
        }

        /// <summary>
        /// get lattitude and longitude 
        /// </summary>
        /// <param name="CreatedFrom"></param>
        /// <param name="Createdto"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedTo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetLatLng(CitationsListViewModel model)
        {
            var citations = ApplyCitationFilter(model);

            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(await citations.ToListAsync());

            return Json(model);
        }

        /// <summary>
        /// add new citation comment
        /// </summary>
        /// <param name="CitationId"></param>
        /// <param name="CommentID"></param>
        /// <param name="Comment"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveComment(Guid citationId, Guid commentId, string comment, bool isPublic)
        {
            var result = new ServiceResponse<Citation>();
            try
            {

                result = await _citationSvc.SaveComment(CommonAccount.Id, citationId, commentId, comment, isPublic, User.GetLoggedInUserId().Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> SaveVideoAttachment(float Time, string Src, Guid VideoId, long CitationNumber, Guid CitationId)
        {
            var result = new ServiceResponse<Citation>() { Success = false };
            try
            {

                var key = $"accounts/{CommonAccount.Number}/citations/{CitationNumber}";
                //var awsUrl = AWSHelper.GetS3Url(filePath, _appSettings.AmazonS3Url);
                Guid thumbnailId = Guid.NewGuid();
                var fileName = thumbnailId + "_thumbnail.png";
                var folderPath = $"{key}/{fileName}";

                bool check = await _ticketWorkFlowService.SaveVideoAttachment(Time, Src, fileName, folderPath, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey);
                // put dude in the database
                if (check)
                {
                    Attachment attach = new Attachment()
                    {
                        AccountId = CommonAccount.Id,
                        MimeType = "image/jpeg",
                        FileName = fileName,
                        Key = folderPath,
                        CreateUserId = User.GetLoggedInUserId().Value,
                        UpdateUserId = User.GetLoggedInUserId().Value,
                        AttachmentType = fileName.GetAttachmentType()
                    };
                    await SaveAttachments(attach, CitationId);
                    result.Success = true;
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;

            }
            return Json(result);
        }





        /// <summary>
        /// edit an existing citation comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditComment(Guid commentId)
        {
            var result = new CitationCommentListItem();
            if (commentId != Guid.Empty)
            {
                try
                {
                    var commentDetails = await _accountCtx.CitationComment.SingleOrDefaultAsync(a => a.Id == commentId);
                    if (commentDetails.CreateUserId == User.GetLoggedInUserId().Value)
                    {
                        result = new CitationCommentListItem
                        {
                            Comment = commentDetails.Comment,
                            CommentID = commentDetails.Id,
                            IsPublic = commentDetails.IsPublic

                        };
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json(result);
        }

        /// <summary>
        /// delete an existing comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            if (commentId != Guid.Empty)
            {
                try
                {

                    var commentDetails = await _accountCtx.CitationComment.SingleOrDefaultAsync(a => a.Id == commentId);
                    if (commentDetails.CreateUserId == User.GetLoggedInUserId().Value)
                    {
                        _accountCtx.Remove(commentDetails);
                        await _accountCtx.SaveChangesAsync();
                        return Json("1");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json("");
        }

        /// <summary>
        /// get comments by citation id
        /// </summary>
        /// <param name="citationId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCommentsByCitationId(Guid citationId)
        {
            var commentsList = new List<CitationCommentListItem>();
            try
            {
                commentsList = await _accountCtx.CitationComment.Where(m => m.CitationId == citationId).Select(m => new CitationCommentListItem
                {
                    Comment = m.Comment,
                    CommentID = m.Id,
                    CreatedById = m.CreateUserId,
                    CreatedBy = string.IsNullOrWhiteSpace(m.CreateUser.FullName) ? m.CreateUser.Email.ToString() : m.CreateUser.FullName.ToString(),
                    CreatedDate = m.CreateUtc,
                    IsPublic = m.IsPublic,
                    EnableEdit = (m.CreateUserId == User.GetLoggedInUserId().Value)
                }).ToListAsync();

                if (LoggedInUser.Permission == Data.Enums.SystemPermissions.None)
                {
                    commentsList = commentsList.Where(x => x.IsPublic == true).OrderByDescending(s => s.CreatedDate).ToList();
                }
                else
                {
                    commentsList = commentsList.OrderByDescending(s => s.CreatedDate).ToList();

                }

                foreach (var m in commentsList)
                {

                    m.CreatedHumanizerDate = m.CreatedDate.Humanize();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(commentsList);
        }

        /// <summary>
        /// Download all attachment and zip all then upload on FTP
        /// </summary>
        /// <param name="citationId"></param>
        /// <param name="citationNumber"></param>
        /// <returns></returns>
        public async Task<bool> SendEvidencePackage(Guid citationId, long citationNumber)
        {
            var citation = await _accountCtx.Citations.ForAccount(CommonAccount.Id)
                            .Include(m => m.CreateUser)
                            .Include(m => m.Violation)
                            .Include(m => m.Comments).ThenInclude(m => m.CreateUser)
                            .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                            .Include(m => m.Attachments).ThenInclude(m => m.CreateUser)
                            .Where(x => x.Id == citationId).SingleOrDefaultAsync();

            if (citation != null)
            {

                var s3Key = await _ticketWorkFlowService.SendEvidencePackage(CommonAccount, citationNumber, _appSettings.FTPUrl, _appSettings.FTPFolderName, _appSettings.FTPUserName, _appSettings.FTPPassWord, citation, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                if (!string.IsNullOrWhiteSpace(s3Key))
                {
                    //Mark that this citation already has an evidence pacakge created. 
                    citation.EvidencePackageCreated = DateTime.UtcNow;
                    citation.EvidencePackageKey = s3Key;

                    await _accountCtx.SaveChangesAsync();

                }
                return true;
            }

            return false;


        }

        /// <summary>
        /// Download Attachment By AttachmentKey
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<FileResult> DownloadAttachment(string attachmentKey, string fileName)
        {
            if (attachmentKey != null)
            {

                //Read the file from AWS Bucket
                var cityAppFile = await _fileService.ReadFile(attachmentKey, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                cityAppFile.FileStream.Position = 0;
                //return downloaded file
                return File(cityAppFile.FileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files, Guid id)
        {
            string message = string.Empty;
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
                                if (formFile.Length > _appSettings.UploadFileSize)
                                {
                                    var size = (_appSettings.UploadFileSize / 1024f) / 1024f;
                                    message = "File size  not be greater than {" + size + "} MB.";
                                }
                                else
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
                                        var result = await _fileService.UploadFile(fileByte1s, fileNameInFolder, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey);
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
                                                CreateUserId = User.GetLoggedInUserId().Value,
                                                UpdateUserId = User.GetLoggedInUserId().Value,
                                                AttachmentType = formFile.FileName.GetAttachmentType()
                                            };
                                            await SaveAttachments(attach, id);

                                            //Create Audit Log
                                            await _citationSvc.CreateAuditEvent(CommonAccount.Id, id, $"Ticket attachment has been uploaded.", User.GetLoggedInUserId().Value, CitationAuditEvent.Other);


                                            message = "File Uploaded Successfully.";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            message = "Invalid File Formate " + formFile.FileName;
                        }
                    }
                }
                else
                {
                    message = "Invalid Ticket Id.";
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return RedirectToAction("Citation", "Citations", new
            {
                id = id,
                message = Cryptography.Encrypt(message)
            });
        }

        public async Task<CitationAttachment> SaveAttachments(Attachment attach, Guid CitationId)
        {
            _accountCtx.Attachments.Add(attach);

            CitationAttachment citationAttach = new CitationAttachment()
            {
                AccountId = CommonAccount.Id,
                AttachmentId = attach.Id,
                CitationId = CitationId,
                CreateUserId = User.GetLoggedInUserId().Value,
                UpdateUserId = User.GetLoggedInUserId().Value
            };

            _accountCtx.CitationAttachments.Add(citationAttach);

            await _accountCtx.SaveChangesAsync();
            return citationAttach;
        }

        public string GetAttachementType(string MimeType)
        {
            string[] words = MimeType.Split('/');
            string Type = words[0];

            Type = char.ToUpper(Type[0]) + Type.Substring(1);
            return Type;
        }


        public async Task<IActionResult> DeleteAttachment(Guid AttachmentId)
        {
            var result = new ServiceResponse<Citation>();
            var attachment = await _accountCtx.Attachments
                                .Include(x => x.Citations)
                                .SingleOrDefaultAsync(s => s.Id == AttachmentId);
            Guid id = attachment.Citations[0].CitationId;
            string message = string.Empty;
            if (attachment != null)
            {
                //Delete the file from AWS Bucket and Database
                var cityAppFile = await _fileService.DeleteFile(attachment.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                _accountCtx.CitationAttachments.RemoveRange(attachment.Citations);
                _accountCtx.Attachments.RemoveRange(attachment);
                _accountCtx.Entry(attachment).State = EntityState.Deleted;
                await _accountCtx.SaveChangesAsync();
                if (cityAppFile == true)
                {
                    result.Success = true;
                    result.Message = "File Deleted Successfully.";
                }
                else
                {
                    result.Success = false;
                    result.Message = "File Deleted Failed.";
                }
            }
            return Json(result);
        }


        private async Task<bool> IsVehicleRelatedType(Guid? violationTypeId)
        {
            var result = false;

            if (violationTypeId.HasValue)
            {
                var violations = await _violationSvc.GetCachedAccountViolations(CommonAccount.Id);
                var violationType = violations.Where(m => m.ViolationTypeId == violationTypeId).FirstOrDefault();

                if (violationType != null)
                {
                    if (violationType.ViolationTypeName.ToLower().Contains("texting") || violationType.ViolationTypeName.ToLower().Contains("parking"))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        #region General Status

        /// <summary>
        /// getting all citation list
        /// </summary>
        /// <param name="CreatedFrom"></param>
        /// <param name="Createdto"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedToId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Status(CitationsListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var citations = ApplyCitationFilter(model);

            switch (model.SortOrder)
            {
                case "Status":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Status);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Status);
                    }
                    break;
                case "Violation":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Violation.Name);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Violation.Name);
                    }
                    break;
                case "AssignedTo":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.AssignedTo);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.AssignedTo);
                    }
                    break;
                case "Created":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
                case "License":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.LicensePlate);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.LicensePlate);
                    }
                    break;
                case "Number":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CitationNumber);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CitationNumber);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
            }

            #region dropdown lists

            var assignList = await _accountCtx.Citations.ForAccount(CommonAccount.Id).Where(c => c.AssignedToId != null).Select(m => new { Text = m.AssignedTo.Email, Value = m.AssignedToId.ToString() }).Distinct().ToListAsync();

            model.AssignedToList.AddRange(assignList.Distinct().Select(m => new SelectListItem { Text = m.Text, Value = m.Value }));

            var violations = await _violationSvc.GetCachedAccountViolations(CommonAccount.Id);

            if (model.ViolationTypeId.HasValue)
            {
                violations = violations.Where(m => m.ViolationTypeId == model.ViolationTypeId).ToList();
            }

            var violationList = violations.Select(m => new SelectListItem
            {
                Text = m.ViolationName,
                Value = m.ViolationId.ToString()
            });
            model.Violations.AddRange(violationList);

            #endregion

            var totalQueryCount = await citations.CountAsync();
            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(await citations.Skip(offset).Take(model.PageSize).ToListAsync());


            foreach (var m in model.CitationsListItem)
            {
                m.CreatedHumanizerDate = m.Created.Humanize();
            }

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;
            model.IsVehicleRelatedType = await IsVehicleRelatedType(model.ViolationTypeId);

            return View(model);
        }


        #endregion

        #region printing     





        public IActionResult CitationCsv(CitationsListViewModel model)
        {

            var citations = ApplyCitationFilter(model);

            switch (model.SortOrder)
            {
                case "Status":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Status);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Status);
                    }
                    break;
                case "Violation":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Violation.Name);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Violation.Name);
                    }
                    break;
                case "AssignedTo":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.AssignedTo);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.AssignedTo);
                    }
                    break;
                case "Created":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
                case "License":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.LicensePlate);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.LicensePlate);
                    }
                    break;
                case "Number":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CitationNumber);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CitationNumber);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
            }



            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(citations);          


            MemoryStream stream = new MemoryStream();
            StreamWriter csvWriter = new StreamWriter(stream, Encoding.UTF8);

            csvWriter.WriteLine("Violation Name,Violation Code,Ticket Number,Status,Location,Assigned To,Created");


            foreach (var contact in model.CitationsListItem)
            {

                var status = contact.Status.GetEnumName();
                var location = $"{contact.Street} {contact.Postalcode}";
                var createdDate = contact.Created.UTCToAccountLocalTime(CommonAccount.CityTimeZone);

                csvWriter.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                contact.Violation.Name,
                contact.Violation.Code,
                contact.CitationNumber,
                 status,
                location,
                contact.Email != null ? contact.Email : " ",
                createdDate));
            }

            csvWriter.Flush();

            stream.Position = 0;
            return File(stream, "text/csv", "Tickets.csv");

        }

        [HttpPost]
        public async Task<IActionResult> CitationPrinting(CitationsListViewModel model)
        {
            var result = new ServiceResponse<CitationPrintModel>() { Success = true };


            var citations = ApplyCitationFilter(model);

            switch (model.SortOrder)
            {
                case "Status":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Status);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Status);
                    }
                    break;
                case "Violation":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.Violation.Name);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.Violation.Name);
                    }
                    break;
                case "AssignedTo":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.AssignedTo);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.AssignedTo);
                    }
                    break;
                case "Created":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
                case "License":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.LicensePlate);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.LicensePlate);
                    }
                    break;
                case "Number":
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CitationNumber);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CitationNumber);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        citations = citations.OrderByDescending(s => s.CreateUtc);
                    }
                    else
                    {
                        citations = citations.OrderBy(s => s.CreateUtc);
                    }
                    break;
            }


            model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(citations);
           

            var printModel = new CitationPrintModel();
            var lstCitationCsv = new List<CitationCsv>();
            var objCitationCsv = new CitationCsv();



            foreach (var citation in model.CitationsListItem)
            {
                var status = citation.Status.GetEnumName();
                var createdDate = citation.Created.UTCToAccountLocalTime(CommonAccount.CityTimeZone);
                CitationCsv objcitation = new CitationCsv()
                {
                    ViolationName = citation.Violation.Name,
                    ViolationCode = citation.Violation.Code != null ? citation.Violation.Code : " ",
                    CitationNumber = citation.CitationNumber,
                    AssignedTo = citation.Email != null ? citation.Email : " ",
                    PostalCode = citation.Postalcode,
                    Street = citation.Street,
                    Status = status,
                    Created = Convert.ToString(createdDate)

                };

                lstCitationCsv.Add(objcitation);

            }
            printModel.CitationCsvItem = lstCitationCsv;
            result.Data = printModel;

            return Json(result);

        }

        #endregion
    }
}