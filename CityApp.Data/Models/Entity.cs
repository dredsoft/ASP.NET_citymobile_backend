using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CityApp.Data.Models
{
    public abstract class Entity
    {
        public Entity()
        {
            var now = DateTime.UtcNow;
            Id = SequentialGuid.GenerateComb();
            CreateUtc = now;
            UpdateUtc = now;
        }

        [Key]
        public Guid Id { get; set; }

        public Guid CreateUserId { get; set; }
        public DateTime CreateUtc { get; set; }

        public Guid UpdateUserId { get; set; }
        public DateTime UpdateUtc { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
