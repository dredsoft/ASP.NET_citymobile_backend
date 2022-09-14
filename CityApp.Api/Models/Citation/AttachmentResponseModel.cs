using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class AttachmentResponseModel
    {
        public List<AttachmentResponse> Response { get; set; } = new List<AttachmentResponse>();
    }

    public class AttachmentResponse
    {
        public Guid AttachmentId { get; set; }
        public Guid CitationId { get; set; }



    }
}
