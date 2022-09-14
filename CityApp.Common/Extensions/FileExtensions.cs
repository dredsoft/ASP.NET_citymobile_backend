using CityApp.Common.Enums;
using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Extensions
{
    public static class FileExtensions
    {
        public static CitationAttachmentType GetAttachmentType(this string fileName)
        {
            var result = CitationAttachmentType.File;

            var images = ".jpg,.gif,.png,.jpeg".Split(',').ToList();
            var videos = ".mp4,.wav".Split(',').ToList();

            var ext = System.IO.Path.GetExtension(fileName).ToLower();
            
            if(images.Contains(ext))
            {
                return CitationAttachmentType.Image;
            }
            else if(videos.Contains(ext))
            {
                return CitationAttachmentType.Video;
            }
            else
            {
                return CitationAttachmentType.File;
            }
        }
    }
}
