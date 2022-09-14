using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class CitationAttachmentListItem
    {
        public Guid AttachmentId { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    return Description;
                }
                else
                {
                    return FileName;
                }
            }
        }
        public CitationAttachmentType AttachmentType { get; set; }
    }
}
