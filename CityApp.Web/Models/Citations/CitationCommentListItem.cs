using AutoMapper;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CityApp.Web.Models
{
    public class CitationCommentListItem
    {
        public Guid CommentID { get; set; }
        public string Comment { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EnableEdit { get; set; }
        public string CreatedHumanizerDate { get; set; }

        public bool IsPublic { get; set; }
    }
}