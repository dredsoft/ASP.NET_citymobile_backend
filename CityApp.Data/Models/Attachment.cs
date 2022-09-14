using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class Attachment : AccountEntity
    {
        /// <summary>
        /// What the file is called
        /// </summary>
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

        public CitationAttachmentType AttachmentType { get; set; }

        /// <summary>
        /// Where the file is located in storage
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        [Required]
        public long ContentLength { get; set; }

        public int? Duration { get; set; }

        [NotMapped]
        public string DisplayDuration
        {
            get
            {
                if (Duration.HasValue)
                {
                    TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(Duration));
                    return $"{time.Minutes} min {time.Seconds} sec";
                }
                else
                {
                    return "0 sec";
                }
            }
        }        

        public List<CitationAttachment> Citations { get; private set; } = new List<CitationAttachment>();

    }

}
