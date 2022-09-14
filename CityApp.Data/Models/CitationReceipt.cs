using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CitationReceipt : AccountEntity
    {
        public Guid CitationId { get; set; }
        public Citation Citation { get; set; }

        [MaxLength(int.MaxValue)]
        public string ReceiptPayload { get; set; }

        [MaxLength(500)]
        public string DevicePublicKey { get; set; }
    }
}
