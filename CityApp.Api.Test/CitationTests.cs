using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CityApp.Api.Test.Models;
using System.Collections.Generic;
using CityApp.Api.Test.Models.Citation;
using CityApp.Data.Models;
using System.Linq;
using CityApp.Api.Test.Models.Violatoins;
using System.Threading;

namespace CityApp.Api.Test
{
    [TestClass]
    public class CitationTests : TestBase
    {
        private string username = "paul@texttoticket.com";
        private string password = "Password1,";
        private string deviceName = "iosPhone";
        private string deviceType = "ios";
        private string deviceToken = "206cb3e1-9acf-4a5e-a87f-7945c2eeb3f9";
        private string baseUrl = "http://localhost:55525/";
        [TestMethod]
        public async Task CreateCitation()
        {

            //Bearer token is needed for every request.  This token has the user information inside of it. 
            var userInformation = await GetUserLoginInformation(username, password, deviceName, deviceType, deviceToken);


            //Get a list of accounts
            var accounts = await GetAccounts(username, password, userInformation.Id, userInformation.Token);

            var numberOfAccounts = accounts.Count;

            var i = 0;
            //Create a violation in each accounts.
            foreach(var account in accounts)
            {
                i++;

                var uri = $"api/v1.0/{account.AccountNumber}/Citations";

                var parameters = new Dictionary<string, string>();

                parameters["status"] = "2";
                parameters["Latitude"] = "38.5815719";
                parameters["Longitude"] = "-121.49439960000001";
                parameters["LicensePlate"] = $"AAAAAA{i}";
                parameters["LocationDescription"] = $"LocationDescription";
                parameters["CitationNumber"] = $"LocationDescription";

                var citation = await PostAsync<CityAppResponse<CitationResponseModel>>(uri, parameters, userInformation.Token);

                Assert.IsTrue(citation.Success);
                Assert.IsTrue(citation.Data.Id.HasValue);

            }
        }



        [TestMethod]
        public async Task BatchCitationCreate()
        {

            //Bearer token is needed for every request.  This token has the user information inside of it. 
            var userInformation = await GetUserLoginInformation(username, password, deviceName, deviceType, deviceToken);


            //Get a list of accounts
            var accounts = await GetAccounts(username, password, userInformation.Id, userInformation.Token);

            var loadTestAccount = accounts.Where(m => m.Name == "City of Marysville").SingleOrDefault();

            if (loadTestAccount != null)
            {
                var violation = await GetViolation(loadTestAccount.AccountNumber, userInformation.Token);
                var numberOfAccounts = accounts.Count;

                //Batch
                var batch = 30;
                var createdCitations = new List<CitationResponseModel>();
                var uri = $"api/v1.0/{loadTestAccount.AccountNumber}/Citations";


                for (int i = 0; i < batch; i++)
                {
                    var parameters = BuildCitation(i, violation);
                    var citation = await PostAsync<CityAppResponse<CitationResponseModel>>(uri, parameters, userInformation.Token);
                    var errors = new List<string>();

                    if (citation != null && citation.Success && citation.Data.CitationNumber > 0)
                    {
                        createdCitations.Add(citation.Data);
                    }
                    else
                    {
                        errors.Add(citation?.Message);
                    }

                    Thread.Sleep(1000);
                }

                Assert.IsTrue(batch == createdCitations.Count());

  
            }
        }

        private async Task<ViolationResponseModel> GetViolation(long accountNumber, string token)
        {
            var uri = $"api/v1.0/{accountNumber}/Violations";

            var violations = await GetAsync<CityAppResponse<List<ViolationResponseModel>>>(uri, token);

            return violations.Data.First();
        }

        private async Task<List<CommonUserAccountModel>> GetAccounts(string username, string password, Guid userId, string token)
        {


            var uri = $"api/v1.0/Users/{userId}/Accounts";

            var result = await GetAsync<CityAppResponse<List<CommonUserAccountModel>>>(uri, token);

            return result.Data;
        }


        private Dictionary<string, string> BuildCitation(int counter, ViolationResponseModel violatoin)
        {

            counter = counter == 0 ? 1 : counter;
            var parameters = new Dictionary<string, string>();

            parameters["status"] = "2";
            parameters["Latitude"] = "38.581822";
            parameters["Longitude"] = "-121.486859";
            parameters["LicensePlate"] = $"{DateTime.UtcNow.Second}AAAAD{counter}";
            parameters["ViolationId"] = violatoin.id.ToString();

            return parameters;
        }
    }
}
