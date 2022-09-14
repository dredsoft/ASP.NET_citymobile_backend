using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class CitationAttachmentModel
    {
        public Attachment Attachment { get; set; }
        public Guid AttachmentId { get; set; }
    }
}
