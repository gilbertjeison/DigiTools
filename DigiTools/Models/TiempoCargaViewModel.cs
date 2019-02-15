using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiTools.Models
{
    public class TiemposCargaViewModel
    {
        public int Id { get; set; }
        public int IdLinea { get; set; }
        public int Year { get; set; }
        public int Mes { get; set; }
        public decimal TiempoCarga { get; set; }
        public string MesName { get; set; }
        public string LineName { get; set; }
    }
}