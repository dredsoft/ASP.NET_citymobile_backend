using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Citations
{
    public class CitationsListViewModel : ListBase
    {
        #region Constants

        public static readonly string NumberSortParam = "Number";
        public static readonly string StatusSortParam = "Status";
        public static readonly string DetailSortParam = "Violation";
        public static readonly string AssignedToSortParam = "AssignedTo";
        public static readonly string CreatedSortParam = "Created";
        public static readonly string LicenseSortParam = "License";

        #endregion


        public List<CitationsListItem> CitationsListItem { get; set; } = new List<Citations.CitationsListItem>();
        public List<SelectListItem> Status { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> AssignedToList { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Violations { get; private set; } = new List<SelectListItem>();


        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        public CitationStatus? StatusId { get; set; }
        public Guid? ViolationId { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? ViolationTypeId { get; set; }

        public bool IsVehicleRelatedType { get; set; }

        public string Title { get; set; } = "Tickets";

        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode  { get; set; }

        public string Street { get; set; }
    }
}
