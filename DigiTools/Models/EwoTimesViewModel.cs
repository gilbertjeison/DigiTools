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

    public class EwoTimesViewModelM
    {
        public int Mes { get; set; }
        public string MesName { get; set; }
        public int Line { get; set; }
        public string LineName { get; set; }
        public int Year { get; set; }

        //VALOR EN LA BASE DE DATOS
        //0
        public int FactoresExt { get; set; }
        //1
        public int FaltaCono { get; set; }
        //2
        public int FaltaDis { get; set; }
        //3
        public int FaltaMtto { get; set; }
        //4
        public int CondSubEstOpe { get; set; }
        //5
        public int FaltaConBas { get; set; }
       
    }
}