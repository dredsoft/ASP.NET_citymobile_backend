using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    public enum CitationStatus
    {
        Open = 1,
        Approved =2,

        [Display(Name = "In Processing")]
        InReview = 3,
        Closed = 4,
        Rejected = 5,
        Contested = 6
    }
}
