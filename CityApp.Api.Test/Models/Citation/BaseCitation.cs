using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Test.Models.Citation
{
    public class BaseCitation
    {
        public Guid? Id { get; set; }
        public Guid? ViolationId { get; set; }

        [Required]
        public CitationStatus Status { get; set; }

        public Guid? AssignedToId { get; set; }


        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public long CitationNumber { get; set; }
    }
}
