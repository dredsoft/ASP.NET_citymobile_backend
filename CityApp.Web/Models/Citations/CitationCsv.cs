using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Citations
{
    public class CitationCsv
    {
        public string ViolationName { get; set; }


        public string ViolationCode { get; set; }

        public long CitationNumber { get; set; }

        public string Status { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }

        public string AssignedTo { get; set; }

        public string Created { get; set; }

    }

}
