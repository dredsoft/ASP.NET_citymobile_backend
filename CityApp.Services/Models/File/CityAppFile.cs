using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services.Models
{
    public class CityAppFile
    {
        public System.IO.Stream FileStream { get; set; }
        public string FileName { get; set; }

        public byte[] FileBytes { get; set; }


    }
}
