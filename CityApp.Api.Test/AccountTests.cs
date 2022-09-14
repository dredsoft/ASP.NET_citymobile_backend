using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using CityApp.Api.Test.Models.Authentication;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using CityApp.Api.Test.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace CityApp.Api.Test
{
    [TestClass]
    public class AccountTests : TestBase
    {
        public string username = "paul@texttoticket.com";
        public string password = "Password1,";
        public string deviceName = "iosPhone";
        public string deviceType = "ios";
        public string deviceToken = "206cb3e1-9acf-4a5e-a87f-7945c2eeb3f9";

        [TestMethod]
        public async Task UserAccountInformation()
        {
       
            var username = "paul@texttoticket.com";
            var password = "Password1,";
            var deviceName = "iosPhone";
            var deviceType = "ios";
            var deviceToken = "206cb3e1-9acf-4a5e-a87f-7945c2eeb3f9";

            //Bearer token is needed for every request.  This token has the user information inside of it. 
            var userInformation = await GetUserLoginInformation(username, password, deviceName, deviceType, deviceToken);

            var uri = $"api/v1.0/Users/{userInformation.Id}/Accounts";

            var result = await GetAsync<CityAppResponse<List<CommonUserAccountModel>>>(uri, userInformation.Token);

            Assert.IsTrue(result.Data.Any());

        }

        [TestMethod]
        public async Task GetAccountViolations()
        {

            //Bearer token is needed for every request.  This token has the user information inside of it. 
            var userInformation = await GetUserLoginInformation(username, password, deviceName, deviceType, deviceToken);

            var uri = $"api/v1.0/Users/{userInformation.Id}/Accounts";

            var result = await GetAsync<CityAppResponse<List<CommonUserAccountModel>>>(uri, userInformation.Token);

            Assert.IsTrue(result.Data.Any());

        }
    }
}
