using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class AttachmentModel
    {

        public Guid Id { get; set; }


        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MimeType { get; set; }

        /// <summary>
        /// Where the file is located in storage
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        public CitationAttachmentType AttachmentType { get; set; }

        [Required]
        public long ContentLength { get; set; }
        public int? Duration { get; set; }

        public string DisplayDuration { get; set; }

        public List<CitationAttachment> Citations { get; private set; } = new List<CitationAttachment>();
    }
}
