using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiTools.Models
{
    public class MttrViewModel
    {
        public int Line { get; set; }
        public string LineName { get; set; }
        public int Mes { get; set; }
        public string MesName { get; set; }
        public int Year { get; set; }
        public float Mttr { get; set; }
    }
}