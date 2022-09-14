using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using CityApp.Web.Areas.Admin.Models;
using CityApp.Web.Middleware;
using CityApp.Common.Utilities;
using Newtonsoft.Json;
using System.IO;
using AutoMapper;
using CityApp.Data.Models;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class CitationReceiptController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<CitationReceiptController>();

        private readonly AccountContext _accountCtx;
        private readonly AccountDbContextFactory _factoryCtx;
        private readonly CommonContext _commonContext;


        public CitationReceiptController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, AccountContext accountContext, AccountDbContextFactory factoryContext)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _accountCtx = accountContext;
            _factoryCtx = factoryContext;
            _commonContext = commonContext;
        }

        public IActionResult Index()
        {
            CitationReceiptViewModel model = new CitationReceiptViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CitationReceiptViewModel model)
        {
            verifiedFile lstVerified = new verifiedFile();
            if (ModelState.IsValid)
            {
                var receipt = _factoryCtx.GetAccountContext(model.AccountNumber);

                var commonAccount = _commonContext.CommonAccounts.Where(x => x.Number == model.AccountNumber).SingleOrDefault();
                if (commonAccount != null)
                {
                    var citation = receipt.CitationReceipts.Where(x => x.Citation.CitationNumber == model.CitationNumber && x.AccountId == commonAccount.Id).SingleOrDefault();
                    if (citation != null)
                    {
                        //JWT Token Method
                        model = await CitationReceipt(model, citation);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CitationNumber), "Invalid Citation Number.");
                        return View(model);
                    }

                }
                else
                {
                    ModelState.AddModelError(nameof(model.AccountNumber), "Invalid Account Number.");
                    return View(model);
                }
            }

            return View(model);
        }


        public async Task<CitationReceiptViewModel> CitationReceipt(CitationReceiptViewModel model, CitationReceipt citation)
        {

            var decodedJWTToken = Cryptography.DecodeJWTToken(citation.DevicePublicKey, citation.ReceiptPayload);
            CitationDeviceReceiptModel receiptModel = JsonConvert.DeserializeObject<CitationDeviceReceiptModel>(decodedJWTToken);
            model.Submitted = receiptModel.submittedUtc;
            model.Device = receiptModel.device;
            model.Email = receiptModel.useremail;
            model.Latitude = receiptModel.latitude;
            model.Longitude = receiptModel.longitude;
            if (model.File != null)
            {
                foreach (var formFile in model.File)
                {
                    //Get current file extension
                    var name = formFile.FileName;

                    // Check uploaded file exists in receiptModel
                    var IsExists = receiptModel.files.Where(x => x.filename == name).Any();
                    if (IsExists)
                    {
                        foreach (var receiptfile in receiptModel.files)
                        {
                            if (receiptfile.filename == name)
                            {
                                var exists = receiptfile.filename;

                                using (var fileStream = formFile.OpenReadStream())
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);


                                    //    using (var ms = new MemoryStream())
                                    //{
                                    //convert image into Bytes
                                    var fileByte1s = ms.ToArray();

                                    var hashfile = Cryptography.Hashfile(fileByte1s, receiptModel.identifier);

                                    if (receiptfile.sha256hash == hashfile)
                                    {
                                        model.VerifiedFiles.Add(new verifiedFile { FileName = name, IsValid = true });
                                    }
                                    else
                                    {
                                        model.VerifiedFiles.Add(new verifiedFile { FileName = name, IsValid = false });

                                    }
                                }
                            }
                            else
                            {
                                model.VerifiedFiles.Add(new verifiedFile { FileName = receiptfile.filename, IsValid = false, Hash = receiptfile.sha256hash });

                            }

                        }
                    }
                    else
                    {
                        foreach (var receiptfile in receiptModel.files)
                        {
                            model.VerifiedFiles.Add(new verifiedFile { FileName = receiptfile.filename, IsValid = false, Hash = receiptfile.sha256hash });
                        }

                    }


                }

            }
            else
            {
                foreach (var receiptfile in receiptModel.files)
                {
                    model.VerifiedFiles.Add(new verifiedFile { FileName = receiptfile.filename, IsValid = false, Hash = receiptfile.sha256hash });
                }

            }

            return model;
        }
    }



}