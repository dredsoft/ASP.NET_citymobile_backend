using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class CitationCommentModel
    {
        public Guid CitationId { get; set; }      

        public Guid Id { get; set; }

        public string CreateUserDisplayName { get; set; }
        public string CreateUserProfileImageKey { get; set; }
        public Guid CreateUserId { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        public bool IsPublic { get; set; }

        public List<CitationComment> Citations { get; private set; } = new List<CitationComment>();
    }
}
