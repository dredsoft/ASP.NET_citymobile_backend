using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Citations
{
    public class CitationPrintModel
    {
        public List<CitationCsv> CitationCsvItem { get; set; } = new List<CitationCsv>();
    }
}
