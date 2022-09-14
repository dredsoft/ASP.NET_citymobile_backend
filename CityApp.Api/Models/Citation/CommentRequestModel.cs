using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class CommentRequestModel
    {

        [Required]
        public Guid CreatedUserId { get; set; }
        public Guid CommentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }

        public bool IsPublic { get; set; }
    }
}
