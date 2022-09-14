using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CommonUserDevice : Entity
    {

        [MaxLength(100)]
        public string DeviceName { get; set; }

        [MaxLength(500)]
        public string DeviceToken { get; set; }

        [MaxLength(100)]
        public string DeviceType { get; set; }

        [MaxLength(500)]
        public string DevicePublicKey { get; set; }

        public Guid UserId { get; set; }
        public CommonUser User { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsLogin { get; set; }

    }
}
