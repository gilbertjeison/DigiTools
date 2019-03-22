using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiTools.Models
{
    public class EwoTimesViewModel
    {
        public int Mes { get; set; }
        public string MesName { get; set; }
        public int Line { get; set; }
        public string LineName { get; set; }       
        public int Year { get; set; }
        public int EsperaTecnico { get; set; }
        public int TiempoDiagnostico { get; set; }
        public int EsperaRepuestos { get; set; }
        public int TiempoReparacion { get; set; }
        public int TiempoPruebas { get; set; }
    }
}