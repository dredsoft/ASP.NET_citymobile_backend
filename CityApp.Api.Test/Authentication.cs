using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using CityApp.Api.Test.Models.Authentication;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using CityApp.Api.Test.Models;

namespace CityApp.Api.Test
{
    [TestClass]
    public class Authentication : TestBase
    {
        [TestMethod]
        public async Task ValidLogin()
        {
            var uri = "api/v1.0/Authentication/Login";

            var username = "paul@texttoticket.com";
            var password = "Password1,";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                var loginModel = new LoginModel() { Email = username, Password = password};

                var data = JsonConvert.SerializeObject(loginModel);
                HttpResponseMessage response = await client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.  
                    var responseData = await response.Content.ReadAsAsync<CityAppResponse<TokenResponseModel>>();

                    Assert.IsTrue(responseData.Success);
                    Assert.IsTrue(responseData.Data != null && !string.IsNullOrWhiteSpace(responseData.Data.Token));
                }
            }
        }
    }
}
