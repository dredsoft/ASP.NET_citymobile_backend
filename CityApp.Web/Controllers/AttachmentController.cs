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
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace CityApp.Web.Controllers
{
    public class AttachmentController : AccountBaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<AttachmentController>();

        private readonly CommonUserService _commonUserSvc;
        private AccountContext _accountCtx;
        private readonly FileService _fileService;
        private readonly AppSettings _appSettings;

        public AttachmentController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CommonUserService commonUserSvc, FileService fileService)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonUserSvc = commonUserSvc;
            _accountCtx = accountCtx;
            _fileService = fileService;
            _appSettings = appSettings.Value;

        }
        /// <summary>
        /// Download Attachment By AttachmentId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<FileResult> Download(Guid Id)
        {
            var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == Id);
            if (attach != null)
            {
                //Read the file from AWS Bucket
                var cityAppFile = await _fileService.ReadFile(attach.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                cityAppFile.FileStream.Position = 0;
                //return downloaded file
                return File(cityAppFile.FileStream, System.Net.Mime.MediaTypeNames.Application.Octet, attach.FileName);
            }
            return null;
        }
    }
}