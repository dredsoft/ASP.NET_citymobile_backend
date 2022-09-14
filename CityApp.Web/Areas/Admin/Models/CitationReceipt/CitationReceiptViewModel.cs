using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class CitationReceiptViewModel
    {



        [Required]
        [Display(Name = "Account Number")]
        public long AccountNumber { get; set; }

        [Required]
        [Display(Name = "Citation Number")]
        public long CitationNumber { get; set; }


        public List<IFormFile> File { get; set; }

        public string Submitted { get; set; }

        public string Device { get; set; }

        public string Email { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string FileName { get; set; }

        public List<verifiedFile> VerifiedFiles { get; set; } = new List<verifiedFile>();
    }

    public class verifiedFile
    {
        public string FileName { get; set; }
        public bool IsValid { get; set; }

        public string Hash { get; set; }
    }
}
