using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class CitationModel: ListBase
    {

        public Guid? CreatedById { get; set; }

        public string CreatedFrom { get; set; }

        public string CreatedTo { get; set; }

        public string LicensePlate { get; set; }

        public List<CitationListModel> CitationList = new List<CitationListModel>();

    }
}
