using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    public enum CitationWorkflow
    {
        None = 1,
        [Display( Name ="Workflow 1")]
        Workflow1 = 2,
        [Display(Name = "Workflow 2")]
        Workflow2 = 3
    }
}
