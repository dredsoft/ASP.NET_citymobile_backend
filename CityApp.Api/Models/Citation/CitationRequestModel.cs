using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    /// <summary>
    /// Request Models are sent from the Consumser
    /// </summary>
    public class CitationRequestModel : BaseCitation
    {

        [MaxLength(25)]
        public string LicensePlate { get; set; }
        [MaxLength(100)]
        public string LicenseState { get; set; }
        [MaxLength(50)]
        public string VehicleMake { get; set; }
        [MaxLength(50)]
        public string VehicleModel { get; set; }
        [MaxLength(50)]
        public string VehicleColor { get; set; }
        [MaxLength(50)]
        public string VehicleType { get; set; }
       
    }
}
