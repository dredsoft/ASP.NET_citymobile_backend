using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CitationAttachment : AccountEntity
    {
        public Citation Citation { get; set; }
        public Guid CitationId { get; set; }

        public Attachment Attachment { get; set; }
        public Guid AttachmentId { get; set; }
    }

}
