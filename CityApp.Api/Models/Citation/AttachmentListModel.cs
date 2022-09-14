using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class AttachmentListModel
    {
        public string Description { get; set; }

        [Required]
        public long Length { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        [MaxLength(100)]
        public string MimeType { get; set; }

        public int? Duration { get; set; }

        [Required]
        public CitationAttachmentType AttachmentType { get; set; }
    }
}
