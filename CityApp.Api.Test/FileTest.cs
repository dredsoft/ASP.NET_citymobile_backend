using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using CityApp.Api.Test.Models.Authentication;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using CityApp.Api.Test.Models;
using System.Collections.Generic;
using CityApp.Api.Test.Models.Citation;
using System.IO;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;

namespace CityApp.Api.Test
{
    [TestClass]
    public class FileTest : TestBase
    {
        private IOptions<AppSettings> appSettings { get; set; }

        public FileTest()
        {
            var appSetting = new AppSettings {AWSAccessKeyID = "AKIAI34B42MPVNTLLR6A", AWSSecretKey= "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", AmazonS3Bucket = "cityapp.dev.west" };
            appSettings = Options.Create(appSetting);
        }

        [TestMethod]
        public async Task UploadAndDownloadTest()
        {
           
            //Get a sample file from your local path.
            var filePath = @"C:\Temp\bird.jpg";
            var fileBytes = File.ReadAllBytes(filePath);

            var fileService = new Services.FileService(appSettings);

            //Add this crazy folder structure to file name
            var fileNameInFolder = $"accounts/1007/citations/1000002/8b568379-4d35-4a5c-9717-65bfc1d09a4c.jpg";

            //Upload the file
            var result = await fileService.UploadFile(fileBytes, fileNameInFolder, appSettings.Value.AWSAccessKeyID, appSettings.Value.AWSSecretKey);

            //Read the file and copy it to the temp directory
            var cityAppFile = await fileService.ReadFile(fileNameInFolder, appSettings.Value.AWSAccessKeyID, appSettings.Value.AWSSecretKey, appSettings.Value.AmazonS3Bucket);

            //Now test the downloading of this file.
            var fileName = "TestUpload" + new Random().Next(1, 99) + ".png";
            var newFile = $"c:\\temp\\{fileName}";
            using (var fileStream = File.Create(newFile))
            {
                cityAppFile.FileStream.Position = 0;
                cityAppFile.FileStream.CopyTo(fileStream);
            }

            Assert.IsTrue(File.ReadAllBytes(newFile).Length > 0);
        }


        private async Task<List<CommonUserAccountModel>> GetAccounts(string username, string password, Guid userId, string token)
        {
            var uri = $"api/v1.0/Users/{userId}/Accounts";

            var result = await GetAsync<CityAppResponse<List<CommonUserAccountModel>>>(uri, token);

            return result.Data;
        }
    }
}
