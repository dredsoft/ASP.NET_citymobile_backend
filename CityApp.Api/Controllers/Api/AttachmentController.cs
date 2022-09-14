using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Services;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{accountNum:long}/Attachments")]

    public class AttachmentController : BaseAccountApiController
    {
        private AccountContext _accountCtx;
        private readonly FileService fileService;
        private readonly AppSettings _appSettings;

        private static readonly ILogger _logger = Log.Logger.ForContext<CitationsController>();


        public AttachmentController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, AccountContext accountCtx, FileService _fileService) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
            fileService = _fileService;
            _appSettings = appSettings.Value;
        }

        // GET: api/values

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid Id)
        {
            var response = new APIResponse<AttachmentModel>() { Success = false };
            var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == Id);
            if (attach != null)
            {
                var userResponse = Mapper.Map<AttachmentModel>(attach);
                response.Data = userResponse;
                response.Success = true;
            }
            return Ok(response);

        }

        [HttpGet("{id:guid}/Download")]
        public async Task<HttpResponseMessage> Download(Guid Id)
        {
            var response1 = new APIResponse<AttachmentModel>() { Success = false };
            var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == Id);
            if (attach != null)
            {
                //Read the file and copy it to the temp directory
                var cityAppFile = await fileService.ReadFile(attach.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);

                cityAppFile.FileStream.Position = 0;

                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(cityAppFile.FileStream)
                };

                // Set content headers
                message.Content.Headers.ContentLength = cityAppFile.FileStream.Length;
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = cityAppFile.FileName,
                    Size = cityAppFile.FileStream.Length
                };

                return message;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);

        }


    }
}
