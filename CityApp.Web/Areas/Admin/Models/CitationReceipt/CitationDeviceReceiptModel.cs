using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class File
    {
        public string createdUtc { get; set; }
        public string filename { get; set; }
        public string sha256hash { get; set; }
    }

    public class CitationDeviceReceiptModel
    {
        public string identifier { get; set; }
        public string submittedUtc { get; set; }
        public string device { get; set; }
        public string useremail { get; set; }
        public string userId { get; set; }
        public string citationNumber { get; set; }
        public string accountNumber { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string publicKey { get; set; }
        public List<File> files { get; set; }
    }
}
