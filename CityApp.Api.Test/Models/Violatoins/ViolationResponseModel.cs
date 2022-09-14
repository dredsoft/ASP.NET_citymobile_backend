using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Test.Models.Violatoins
{


    public class ViolationResponseModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public object customName { get; set; }
        public string categoryName { get; set; }
        public string typeName { get; set; }
        public object helpUrl { get; set; }
        public object customHelpUrl { get; set; }
        public string actions { get; set; }
        public string customActions { get; set; }
        public object description { get; set; }
        public object customDescription { get; set; }
        public object displayDescription { get; set; }
        public string displayName { get; set; }
        public string displayActions { get; set; }
        public object displayHelpUrl { get; set; }
    }

}
