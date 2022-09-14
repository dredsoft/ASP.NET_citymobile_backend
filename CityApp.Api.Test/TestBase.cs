using CityApp.Api.Test.Models;
using CityApp.Api.Test.Models.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Test
{

    public class TestBase
    {
        public HttpClient httpClient = new HttpClient();

        //public string BaseUrl = "https://cityappapidev.azurewebsites.net/";

        public string BaseUrl = "http://localhost:55525/";

        public TestBase()
        {
            httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<TokenResponseModel> GetUserLoginInformation(string email, string password, string deviceName, string deviceType, string deviceToken)
        {
            var uri = "api/v1.0/Authentication/Login";

            var result = new TokenResponseModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.BaseUrl);

                var loginModel = new LoginModel() { Email = email, Password = password, DeviceName = deviceName, DeviceType =deviceType, DeviceToken = deviceToken };

                var data = JsonConvert.SerializeObject(loginModel);
                HttpResponseMessage response = await client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.  
                    var responseData = await response.Content.ReadAsAsync<CityAppResponse<TokenResponseModel>>();

                    if (responseData.Success)
                    {
                        result = responseData.Data;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Generic Post Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="bearer"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, Dictionary<string, string> parameters, string bearer)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var data = JsonConvert.SerializeObject(parameters);

            var response = await httpClient.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
            var contents = await response.Content.ReadAsAsync<T>();

            return contents;
        }

        /// <summary>
        /// Generic Get Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="bearer"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, string bearer)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var response = await httpClient.GetAsync(url);

            try
            {
                var contents = await response.Content.ReadAsAsync<T>();
                return contents;
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                return default(T);
            }


        }
    }

}
