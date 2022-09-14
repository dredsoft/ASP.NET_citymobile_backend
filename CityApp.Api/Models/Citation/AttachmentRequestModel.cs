using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class AttachmentRequestModel
    {
        [Required]
        public Guid CitationId { get; set; }

        public string DeviceReceipt { get; set; }

        public string DevicePublicKey { get; set; }

        public List<AttachmentListModel> Attachments { get; set; }

    }
}
