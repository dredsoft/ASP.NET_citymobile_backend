using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models.Partitions
{
    public class PartitionListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public string Occupancy { get; set; }


    }
}
