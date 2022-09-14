using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Common
{
    public class MailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        [MaxLength(150)]
        public string Subject { get; set; }
        [MaxLength(300)]
        public string Message { get; set; }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public long? AccountNumber { get; set; }

        public string BuildVersion { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
