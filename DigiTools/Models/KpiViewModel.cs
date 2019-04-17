using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigiTools.Models
{
    public class KpiViewModel
    {
        public int Edit { get; set; }

        public int Id { get; set; }
        public int Consecutivo { get; set; }

        [Display(Name = "Línea")]
        public int IdLinea { get; set; }
        public SelectList LineaList { get; set; }
        
        [Display(Name = "Máquina")]
        public int IdMaquina { get; set; }
        public SelectList MaquinaList { get; set; }

        public DateTime Fecha { get; set; }

        [Display(Name = "Número de aviso")]
        public string NumAviso { get; set; }

        [Display(Name = "Tipo de avería")]
        public int IdTipoAveria { get; set; }
        public SelectList TipoAveriaList { get; set; }

        [Display(Name = "Turno")]
        public int Turno { get; set; }


        [DisplayName("Notificación Avería (Hora)")]
        public string HrNotAve { get; set; }
        public DateTime HrNotAveD { get; set; }

        [DisplayName("Inicio de Reparación (Hora)")]
        public string HrIniRep { get; set; }
        public DateTime HrIniRepD { get; set; }

        [DisplayName("Tiempo de espera inicial del técnico (min)")]
        public int TEspIniTec { get; set; }

        [DisplayName("Tiempo de Diagnóstico (min)")]
        public int TDiagn { get; set; }

        [DisplayName("Tiempo de espera por repuestos (min)")]
        public int TEspRep { get; set; }

        [DisplayName("T. de Reparación/cambio de piezas (min)")]
        public int TRepCamP { get; set; }

        [DisplayName("Pruebas y Tiempo de arranque (min)")]
        public int PruTieArr { get; set; }

        [DisplayName("Fin reparación y entrega a AM (Hora)")]
        public string HrFinRepEnt { get; set; }
        public DateTime HrFinRepEntD { get; set; }

        [DisplayName("Tiempo Total (min)")]
        public int TiempoTotal { get; set; }

        public string PathImage1 { get; set; }
        public string DescImg1 { get; set; }
        public HttpPostedFileBase Image1 { get; set; }

        
        public string PathImage2 { get; set; }
        public string DescImg2 { get; set; }
        public HttpPostedFileBase Image2 { get; set; }

        public string PathImagePQ1 { get; set; }
        public string DescImgPQ1 { get; set; }
        public HttpPostedFileBase ImagePQ1 { get; set; }

        public string PathImagePQ2 { get; set; }
        public string DescImgPQ2 { get; set; }
        public HttpPostedFileBase ImagePQ2 { get; set; }

        [Display(Name = "Seleccione una acción")]
        public int Accion { get; set; }
       
        public string RepUtil { get; set; }

        public string GembaDesc { get; set; }
        public string GembutsuDesc { get; set; }
        public string GenjitsuDesc { get; set; }
        public string GenriDesc { get; set; }
        public string GensokuDesc { get; set; }

        public string GembaOk { get; set; }
        public string GembutsuOk { get; set; }
        public string GenjitsuOk { get; set; }
        public string GenriOk { get; set; }
        public string GensokuOk { get; set; }

        public bool GembaOkB { get; set; }
        public bool GembutsuOkB { get; set; }
        public bool GenjitsuOkB { get; set; }
        public bool GenriOkB { get; set; }
        public bool GensokuOkB { get; set; }

        public string QueDesc { get; set; }
        public string DondeDesc { get; set; }
        public string CuandoDesc { get; set; }
        public string QuienDesc { get; set; }
        public string CualDesc { get; set; }
        public string ComoDesc { get; set; }
        public string FenomenoDesc { get; set; }

        public string Porques { get; set; }

        public int CausaRaiz { get; set; }
        public int CicloRaiz { get; set; }
        public string DescCicloRaiz { get; set; }
        public string FchUltimoMtto { get; set; }
        public string FchProxMtto { get; set; }
        
        public string Cmd { get; set; }

        [DisplayName("Diligenciada por")]
        public string IdDiligenciado { get; set; }

        [DisplayName("Técnicos de mantenimiento involucrados")]
        public string IdTecMattInv { get; set; }

        [DisplayName("Operarios involucrados")]
        public string IdOpersInv { get; set; }

        [DisplayName("Análisis elaborado por")]
        public string IdAnaElab { get; set; }

        [DisplayName("Fecha")]
        public string FchAnaElab { get; set; }

        [DisplayName("Contramedidas definidas por")]
        public string IdContMedDef { get; set; }

        [DisplayName("Fecha")]
        public string FchDefConMed { get; set; }

        [DisplayName("Ejecución validada por")]
        public string IdEjeValPor { get; set; }

        [DisplayName("Fecha")]
        public string FchEjeVal { get; set; }


        [Display(Name ="Planta")]        
        public int IdPlanta { get; set; }
        public SelectList PlantaList { get; set; }

        [Display(Name = "Tipo de línea")]
        public int IdTipoLinea { get; set; }
        public SelectList TipoLineaList { get; set; }


        public string FormattedDate => Fecha.ToShortDateString();


        [Display(Name = "Descripción de la avería")]
        public string DescripcionAveria { get; set; }


        #region CAMPOS DESCRIPTIVOS
        public string AreaLinea { get; set; }
        public string Equipo { get; set; }
        public string DiligenciadoPor { get; set; }
        public string TipoAveria { get; set; }
        public DateTime FchUltimoMttoD { get;  set; }
        public DateTime FchProxMttoD { get;  set; }

        #endregion
    }

    public class rep_util
    {
        public string codigo_sap { get; set; }
        public string descripcion { get; set; }
        public string cantidad { get; set; }
        public string  costo { get; set; }
    }

    public class Klista_accion
    {        
        public int Id { get; set; }
        public string accion { get; set; }
        public string tipo_accion { get; set; }
        public string responsable { get; set; }
        public string fecha { get; set; }
        public Nullable<int> id_ewo { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
        
    }
}