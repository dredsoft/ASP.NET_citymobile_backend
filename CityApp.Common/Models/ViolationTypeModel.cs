using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    public class ViolationTypeModel
    {
        public Guid Id { get; set; }
        public Guid CommonViolationTypeId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string CustomName { get; set; }

        [IgnoreMap]
        public string DisplayName {
            get{
                if(!string.IsNullOrWhiteSpace(CustomName))
                {
                    return CustomName;
                }
                else
                {
                    return Name;
                }
            }
        }

    }
}
