using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigiTools.Models
{
    public class MlModelView
    {       
        public List<PlantasViewModel> PlantasModelList { get; set; }
        public List<LineasViewModel> LineasModelList { get; set; }
    }
    public class PlantasViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Image { get; set; }

        public List<lineas> ListLineas { get; set; }
    }


    public class LineasViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public string Nombre { get; set; }

        [Display(Name = "Imágen")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public string Image { get; set; }

       
        public HttpPostedFileBase Images { get; set; }

        [Display(Name = "Tipo de línea")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public int TipoLinea { get; set; }

        [Display(Name = "Planta")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public int IdPlanta { get; set; }

        [Display(Name = "Tiempo de carga")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public decimal TiempoCarga { get; set; }

        public List<maquinas> ListMaquinas { get; set; }
    }

    public class MaquinasViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public string Nombre { get; set; }

        [Display(Name = "Imágen")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public string Image { get; set; }
        
        public HttpPostedFileBase Images { get; set; }

        [Display(Name = "Línea")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public int IdLinea { get; set; }

        [Display(Name = "Planta")]
        [Required(ErrorMessage = "Este campo es obligatorio...")]
        public int IdPlanta { get; set; }

        public string DescPlanta { get; set; }

        public List<sistemas> ListSistemas { get; set; }
    }
}