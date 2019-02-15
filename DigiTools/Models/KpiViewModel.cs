using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiTools.Models
{
    public class KpiViewModel
    {
        [Display(Name ="Planta")]
        [Required(ErrorMessage ="Por favor seleccione planta")]
        public int IdPlanta { get; set; }
        public SelectList PlantaList { get; set; }

        [Display(Name = "Tipo de línea")]
        [Required(ErrorMessage = "Por favor seleccione un tipo de línea")]
        public int IdTipoLinea { get; set; }
        public SelectList TipoLineaList { get; set; }

        [Display(Name = "Línea")]
        [Required(ErrorMessage = "Por favor seleccione una línea")]
        public int IdLinea { get; set; }
        public SelectList LineaList { get; set; }
        

        [Display(Name = "Máquina")]
        [Required(ErrorMessage = "Por favor seleccione una línea")]
        public int IdMaquina { get; set; }
        public SelectList MaquinaList { get; set; }


        [Display(Name = "Número de aviso")]
        public string NumAviso { get; set; }


        [Display(Name = "Tipo de avería")]
        public int IdTipoAveria { get; set; }
        public SelectList TipoAveriaList { get; set; }


        [Display(Name = "Descripción de la avería")]
        public string DescripcionAveria { get; set; }

        
        public string GembaDesc { get; set; }
        public string GembutsuDesc { get; set; }
        public string GenjitsuDesc { get; set; }
        public string GenriDesc { get; set; }
        public string GensokuDesc { get; set; }

        public string QueDesc { get; set; }
        public string DondeDesc { get; set; }
        public string CuandoDesc { get; set; }
        public string QuienDesc { get; set; }
        public string CualDesc { get; set; }
        public string ComoDesc { get; set; }
        public string FenomenoDesc { get; set; }


        [Display(Name = "Seleccione una acción")]
        public string Accion { get; set; }


        [DisplayName("Notificación Avería (Hora)")]
        [Required(ErrorMessage = "Se debe seleccionar una hora...")]
        public string HrNotAve { get; set; }

        [DisplayName("Inicio de Reparación (Hora)")]
        [Required(ErrorMessage = "Se debe seleccionar una hora...")]
        public string HrIniRep{ get; set; }

        [DisplayName("Tiempo de espera inicial del técnico (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int TEspIniTec { get; set; }

        [DisplayName("Tiempo de Diagnóstico (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int TDiagn { get; set; }

        [DisplayName("Tiempo de espera por repuestos (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int TEspRep { get; set; }

        [DisplayName("T. de Reparación/cambio de piezas (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int TRepCamP { get; set; }

        [DisplayName("Pruebas y Tiempo de arranque (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int PruTieArr { get; set; }

        [DisplayName("Fin reparación y entrega a AM (Hora)")]
        [Required(ErrorMessage = "Se debe seleccionar una hora...")]
        public string HrFinRepEnt { get; set; }
        
        [DisplayName("Tiempo Total (min)")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public int TiempoTotal { get; set; }

        [DisplayName("Diligenciada por")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdDiligenciado { get; set; }

        [DisplayName("Técnicos de mantenimiento involucrados")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdTecMattInv { get; set; }

        [DisplayName("Operarios involucrados")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdOpersInv { get; set; }

        [DisplayName("Análisis elaborado por")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdAnaElab { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string FchAnaElab { get; set; }

        [DisplayName("Contramedidas definidas por")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdContMedDef { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string FchDefConMed { get; set; }

        [DisplayName("Ejecución validada por")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string IdEjeValPor { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "Este campo es requerido...")]
        public string FchEjeVal { get; set; }

        public string FchUltimoMtto { get; set; }
        public string FchProxMtto { get; set; }
    }
}