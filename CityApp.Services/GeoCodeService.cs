using CityApp.Common.Models;
using CityApp.Services.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services
{
    public class GeoCodeService : ICustomService
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<GeoCodeService>();
        private readonly AppSettings _appSettings;

        private string _locationEndPoint = "http://locationiq.org/v1/reverse.php?format=json&key={0}&lat={1}&lon={2}";
        //Use Google instead "https://maps.googleapis.com/maps/api/geocode/json?latlng={1},{2}&key={0}";

        public GeoCodeService()
        {
        }

        public GeoCodeService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public async Task<GeoLocation> ReverseCode(decimal latitude, decimal longitude)
        {
            GeoLocation result = null;

            var url = string.Format(_locationEndPoint, _appSettings.LocationIQKey, latitude, longitude);
            //_logger.Error("Begin Geocode");
            result = await GetSingleAsync<GeoLocation>(url);
            //_logger.Error("Geo Gode retrieved");

            return result;
        }

        public async Task<T> GetSingleAsync<T>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);

                    var contents = await response.Content.ReadAsAsync<T>();

                    //Rety once
                    if (contents == null)
                    {
                        response = await httpClient.GetAsync(url);

                        contents = await response.Content.ReadAsAsync<T>();
                    }

                    return contents;
                }
                catch(Exception ex)
                {
                    _logger.Error("url get request " + url);

                    return default(T);
                }
            }
        }

    }
}
