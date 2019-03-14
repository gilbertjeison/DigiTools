using DigiTools.Dao;
using DigiTools.Database;
using DigiTools.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ExcelApp = Microsoft.Office.Interop.Excel ;

namespace DigiTools.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        DaoPlantas daoPlantas = new DaoPlantas();
        DaoTipoData daoTD = new DaoTipoData();
        DaoLineas daoLin = new DaoLineas();
        DaoMaquina daoMaq = new DaoMaquina();
        DaoPersonal daoPer = new DaoPersonal();
        DaoUsuarios daoUser = new DaoUsuarios();
        DaoRepuestos daoRep = new DaoRepuestos();
        DaoPorque daoPor = new DaoPorque();
        DaoAcciones daoAcc = new DaoAcciones();
        DaoEwo daoEwo = new DaoEwo();
        DaoTecnicos daoTec = new DaoTecnicos();
        int Consecutivo = 0;
        AspNetUsers aspNetUsers;
        static string ewo_images = "~/Content/images/ewo_images/";
        
        private static readonly int LINES = 21;
        private static readonly int BREAKDOWNSTYPES = 13;

        public async Task<ActionResult> Index()
        {
            aspNetUsers = daoUser.GetUser(User.Identity.GetUserId());

            var req = Request;
            if (aspNetUsers.IdRol.Equals("65b01f2a-0b46-4d0c-a227-304dc22e2f9d"))
            {
                var kpiWiewModel = new KpiViewModel();

                //LISTA DE PLANTAS
                var list = await daoPlantas.GetPlants();
                list.Insert(0, new plantas() { Id = 0, nombre = "Seleccione planta..." });
                kpiWiewModel.PlantaList = new SelectList(list, "Id", "nombre");

                //LISTA DE TIPOS DE LÍNEA
                var listTL = await daoTD.GetTypesAsync(LINES);
                listTL.Insert(0, new tipos_data() { Id = 0, descripcion = "Seleccione tipo de línea..." });
                kpiWiewModel.TipoLineaList = new SelectList(listTL, "Id", "descripcion");

                //LISTA DE TIPOS DE AVERÍAS
                var listA = await daoTD.GetTypesAsync(BREAKDOWNSTYPES);
                listA.Insert(0, new tipos_data() { Id = 0, descripcion = "Seleccione tipo de avería..." });
                kpiWiewModel.TipoAveriaList = new SelectList(listA, "Id", "descripcion");

                kpiWiewModel.IdDiligenciado = aspNetUsers.Nombres +" " + aspNetUsers.Apellidos;
                Consecutivo = await daoEwo.GetLastConsecutive();
                ViewBag.Cons = "00" + Consecutivo;                

                return View(kpiWiewModel);
            }
            else
            {
                return View("IndexAdmin");
            }            
        }

        [HttpPost]
        public async Task<JsonResult> CreateKpi(KpiViewModel kvm)
        {
            int r;
            string message;

            try
            {
                ewos ewo = new ewos();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<repuestos_utilizados> rep_Utils = ser.Deserialize<List<repuestos_utilizados>>(kvm.RepUtil);
                List<porques> porques = ser.Deserialize<List<porques>>(kvm.Porques);
                List<lista_acciones> lista_acc = ser.Deserialize<List<lista_acciones>>(kvm.Cmd);

                ewo.consecutivo = await daoEwo.GetLastConsecutive();
                ewo.id_area_linea = kvm.IdLinea;
                ewo.id_equipo = kvm.IdMaquina;
                ewo.fecha_ewo = DateTime.Now;
                ewo.aviso_numero = kvm.NumAviso;
                ewo.id_tecnico = daoUser.GetUser(User.Identity.GetUserId()).Id;
                ewo.id_tipo_averia = kvm.IdTipoAveria;
                ewo.id_turno = kvm.Turno;

                //CONVERTIR FECHAS A DATETIME
                ewo.notificacion_averia = kvm.HrNotAve == null? DateTime.Now :DateTime.ParseExact(kvm.HrNotAve, "dd-MM-yyyy HH:mm",CultureInfo.InvariantCulture);
                ewo.inicio_reparacion = kvm.HrIniRep == null ? DateTime.Now : DateTime.ParseExact(kvm.HrIniRep, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                ewo.fin_reparacion = kvm.HrFinRepEnt == null ? DateTime.Now : DateTime.ParseExact(kvm.HrFinRepEnt, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                ewo.fecha_ultimo_mtto = kvm.FchUltimoMtto == null ? DateTime.Now : DateTime.ParseExact(kvm.FchUltimoMtto, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_proximo_mtto = kvm.FchProxMtto == null ? DateTime.Now : DateTime.ParseExact(kvm.FchProxMtto, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_analisis = kvm.FchAnaElab == null ? DateTime.Now : DateTime.ParseExact(kvm.FchAnaElab, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_validacion = kvm.FchEjeVal == null ? DateTime.Now : DateTime.ParseExact(kvm.FchEjeVal, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_contramedida = kvm.FchDefConMed == null ? DateTime.Now : DateTime.ParseExact(kvm.FchDefConMed, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                ewo.tiempo_espera_tecnico = kvm.TEspIniTec;
                ewo.tiempo_diagnostico = kvm.TDiagn;
                ewo.tiempo_espera_repuestos = kvm.TEspRep;
                ewo.tiempo_reparacion = kvm.TRepCamP;
                ewo.tiempo_pruebas = kvm.PruTieArr;
                ewo.tiempo_total = kvm.TiempoTotal;

                //GUARDAR RUTA DE LAS IMAGENES                
                ewo.imagen_1 = kvm.Image1 != null ? kvm.Image1.FileName : "";
                ewo.imagen_2 = kvm.Image2 != null ? kvm.Image2.FileName : "";
                ewo.imagen_3 = kvm.ImagePQ1 != null ? kvm.ImagePQ1.FileName : "";
                ewo.imagen_4 = kvm.ImagePQ2 != null ? kvm.ImagePQ2.FileName : "";

                ewo.desc_imagen_1 = kvm.DescImg1;
                ewo.desc_imagen_2 = kvm.DescImg2;
                ewo.desc_imagen_3 = kvm.DescImgPQ1;
                ewo.desc_imagen_4 = kvm.DescImgPQ2;
                ewo.desc_averia = kvm.DescripcionAveria;
                ewo.cambio_componente = kvm.Accion == 1 ? true : false;
                ewo.ajuste = kvm.Accion == 0 ? true : false;
                ewo.what = kvm.QueDesc;
                ewo.where = kvm.DondeDesc;
                ewo.when = kvm.CuandoDesc;
                ewo.who = kvm.QuienDesc;
                ewo.wich = kvm.CualDesc;
                ewo.how = kvm.ComoDesc;
                ewo.fenomeno = kvm.FenomenoDesc;
                ewo.gemba = kvm.GembaDesc;
                ewo.gembutsu = kvm.GembutsuDesc;
                ewo.genjitsu = kvm.GenjitsuDesc;
                ewo.genri = kvm.GenriDesc;
                ewo.gensoku = kvm.GensokuDesc;
                ewo.falla_index = kvm.CausaRaiz;
                ewo.causa_raiz_index = kvm.CicloRaiz;
                ewo.tecnicos_man_involucrados = kvm.IdTecMattInv;
                ewo.operarios_involucrados = kvm.IdOpersInv;
                ewo.elaborador_analisis = kvm.IdAnaElab;
                ewo.definidor_contramedidas = kvm.IdContMedDef;
                ewo.validador_ejecucion = kvm.IdEjeValPor;
                
                //to do
                //EVALUAR EL RESULTADO DE CADA CHECK Y CONVERTIRLO A BOOL                
                ewo.gemba_ok = kvm.GembaOk != null ? true:false;
                ewo.genjitsu_ok = kvm.GenjitsuOk != null ? true : false;
                ewo.genri_ok = kvm.GenriOk != null ? true : false;
                ewo.gensoku_ok = kvm.GensokuOk != null ? true : false;
                ewo.gembutsu_ok = kvm.GembutsuOk != null ? true : false;

                int res = await daoEwo.AddEwo(ewo);

                //INTERACTUAR CON CADA UNA DE LAS LISTAS DE ITEMS PARA INGRESAR EN SU DETERMINADA TABLA
                if (res > 0)
                {
                    foreach (var item in rep_Utils)
                    {
                        item.id_ewo = res;
                    }

                    foreach (var item in porques)
                    {
                        item.id_ewo = res;
                    }

                    foreach (var item in lista_acc)
                    {
                        item.id_ewo = res;
                    }

                    await daoAcc.AddAcciones(lista_acc);
                    await daoRep.AddRepUtil(rep_Utils);
                    await daoPor.AddPorque(porques);

                    var descData = await daoEwo.GetEwoDesc(res);

                    //QUEBRAR PROCESO DE EXCEL
                    //foreach (Process clsProcess in Process.GetProcesses())
                    //{
                    //    if (clsProcess.ProcessName.Equals("EXCEL"))
                    //    {
                    //        clsProcess.Kill();
                    //        break;
                    //    }
                    //}

                    string filename = Server.MapPath("~/Content/formats/FEU.XLSX");
                    string nfilename = Server.MapPath("~/Content/formats/FORMATO_EWO.XLSX");

                    //ESCRIBIR EN ARCHIVO ECXEL
                    FileInfo file = new FileInfo(filename);
                    FileInfo fileN = new FileInfo(nfilename);

                    if (fileN.Exists)
                    {
                        fileN.Delete();                        
                    }

                    file.CopyTo(nfilename);

                    using (ExcelPackage excelPackage = new ExcelPackage(fileN))
                    {
                        var ws = excelPackage.Workbook.Worksheets["EWO"];

                        ws.Cells["A3"].Value = ewo.consecutivo;
                        ws.Cells["B3"].Value = descData.AreaLinea;
                        ws.Cells["C3"].Value = descData.Equipo;
                        ws.Cells["E3"].Value = ewo.fecha_ewo.Value.ToShortDateString();
                        ws.Cells["F3"].Value = ewo.aviso_numero;
                        ws.Cells["G3"].Value = descData.DiligenciadoPor;
                        ws.Cells["H3"].Value = descData.TipoAveria;
                        ws.Cells["J3"].Value = ewo.id_turno;

                        ws.Cells["A6"].Value = ewo.notificacion_averia;
                        ws.Cells["B6"].Value = ewo.inicio_reparacion;
                        ws.Cells["C6"].Value = ewo.tiempo_espera_tecnico;
                        ws.Cells["D6"].Value = ewo.tiempo_diagnostico;
                        ws.Cells["E6"].Value = ewo.tiempo_espera_repuestos;
                        ws.Cells["F6"].Value = ewo.tiempo_reparacion;
                        ws.Cells["G6"].Value = ewo.tiempo_pruebas;
                        ws.Cells["H6"].Value = ewo.fin_reparacion;
                        ws.Cells["I6"].Value = ewo.tiempo_total;

                        //IMAGEN 1
                        var eImg = ws.Drawings.AddPicture("image1", Image.FromStream(kvm.Image1.InputStream, true, true));
                        eImg.From.Column = 0;
                        eImg.From.Row = 7;
                        eImg.SetSize(170, 220);

                        // 2x2 px space for better alignment
                        eImg.From.ColumnOff = Pixel2MTU(2);
                        eImg.From.RowOff = Pixel2MTU(60);

                        //GUARDAR IMAGEN EN SERVIDOR
                        SaveImageEwoServer(kvm.Image1);
                        SaveImageEwoServer(kvm.ImagePQ1);

                        //IMAGEN 2
                        if (kvm.Image2 != null)
                        {
                            var eImg2 = ws.Drawings.AddPicture("image2", Image.FromStream(kvm.Image2.InputStream, true, true));
                            eImg2.From.Column = 2;
                            eImg2.From.Row = 7;
                            eImg2.SetSize(220, 220);

                            // 2x2 px space for better alignment
                            eImg2.From.ColumnOff = Pixel2MTU(20);
                            eImg2.From.RowOff = Pixel2MTU(60);

                            //GUARDAR IMAGEN EN SERVIDOR
                            SaveImageEwoServer(kvm.Image2);
                        }


                        ws.Cells["A16"].Value = ewo.desc_imagen_1;
                        ws.Cells["C16"].Value = ewo.desc_imagen_2;
                        ws.Cells["F8"].Value = ewo.desc_averia;

                        //CAMBIO DE REPUESTO O AJUSTE
                        string a = "", c = "";
                        if (ewo.ajuste == true) { a = "X"; } else { c = "X"; }
                        ws.Cells["F12"].Value = "( " + c + "  ) Cambio de Componente                    (  " + a + "  ) Ajuste";

                        if (rep_Utils.Count > 0)
                        {
                            for (int row = 15; row < rep_Utils.Count + 15; row++)
                            {
                                for (int cols = 6; cols < 10; cols++)
                                {
                                    if (cols == 6)
                                    {
                                        ws.SetValue(row, cols, rep_Utils[row - 15].codigo_sap);
                                    }
                                    if (cols == 7)
                                    {
                                        ws.SetValue(row, cols, rep_Utils[row - 15].descripcion);
                                    }
                                    if (cols == 8)
                                    {
                                        ws.SetValue(row, cols + 1, rep_Utils[row - 15].cantidad);
                                    }
                                    if (cols == 9)
                                    {
                                        ws.SetValue(row, cols + 1, "$" + rep_Utils[row - 15].costo);
                                    }
                                }
                            }
                        }

                        //5G CHECK
                        if (ewo.gemba_ok == true)
                        {
                            ws.Cells["J22"].Value = "X";
                        }
                        else
                        {
                            ws.Cells["I22"].Value = "X";
                        }

                        if (ewo.gembutsu_ok == true)
                        {
                            ws.Cells["J23"].Value = "X";
                        }
                        else
                        {
                            ws.Cells["I23"].Value = "X";
                        }

                        if (ewo.genjitsu_ok == true)
                        {
                            ws.Cells["J24"].Value = "X";
                        }
                        else
                        {
                            ws.Cells["I24"].Value = "X";
                        }

                        if (ewo.genri_ok == true)
                        {
                            ws.Cells["J25"].Value = "X";
                        }
                        else
                        {
                            ws.Cells["I25"].Value = "X";
                        }

                        if (ewo.gensoku_ok == true)
                        {
                            ws.Cells["J26"].Value = "X";
                        }
                        else
                        {
                            ws.Cells["I26"].Value = "X";
                        }

                        //5G TEXT
                        ws.Cells["C22"].Value = ewo.gemba;
                        ws.Cells["C23"].Value = ewo.gembutsu;
                        ws.Cells["C24"].Value = ewo.genjitsu;
                        ws.Cells["C25"].Value = ewo.genri;
                        ws.Cells["C26"].Value = ewo.gensoku;

                        //5W + 1H
                        ws.Cells["C28"].Value = ewo.what;
                        ws.Cells["C29"].Value = ewo.where;
                        ws.Cells["C30"].Value = ewo.when;
                        ws.Cells["C31"].Value = ewo.who;
                        ws.Cells["C32"].Value = ewo.wich;
                        ws.Cells["C33"].Value = ewo.how;

                        ws.Cells["A35"].Value = ewo.fenomeno;

                        //CUADRO PORQUE PORQUE
                        if (porques.Count > 0)
                        {
                            for (int row = 38; row < porques.Count + 38; row++)
                            {
                                for (int cols = 1; cols <= 10; cols += 2)
                                {
                                    if (cols == 1)
                                    {
                                        ws.SetValue(row, cols, porques[row - 38].porque_1);
                                    }
                                    if (cols == 3)
                                    {
                                        ws.SetValue(row, cols, porques[row - 38].porque_2);
                                    }
                                    if (cols == 5)
                                    {
                                        ws.SetValue(row, cols, porques[row - 38].porque_3);
                                    }
                                    if (cols == 7)
                                    {
                                        ws.SetValue(row, cols, porques[row - 38].porque_4);
                                    }
                                    if (cols == 9)
                                    {
                                        ws.SetValue(row, cols, porques[row - 38].porque_5);
                                    }
                                }
                            }
                        }

                        //IMAGEN 3
                        var eImg3 = ws.Drawings.AddPicture("image3", Image.FromStream(kvm.ImagePQ1.InputStream, true, true));
                        eImg3.From.Column = 0;
                        eImg3.From.Row = 42;
                        eImg3.SetSize(320, 235);

                        // 2x2 px space for better alignment
                        eImg3.From.ColumnOff = Pixel2MTU(60);
                        eImg3.From.RowOff = Pixel2MTU(30);

                        //IMAGEN 4
                        if (kvm.ImagePQ2 != null)
                        {
                            var eImg4 = ws.Drawings.AddPicture("image4", Image.FromStream(kvm.ImagePQ2.InputStream, true, true));
                            eImg4.From.Column = 5;
                            eImg4.From.Row = 42;
                            eImg4.SetSize(320, 235);

                            // 2x2 px space for better alignment
                            eImg4.From.ColumnOff = Pixel2MTU(60);
                            eImg4.From.RowOff = Pixel2MTU(30);

                            //GUARDAR IMAGEN EN SERVIDOR
                            SaveImageEwoServer(kvm.ImagePQ2);
                        }
                            

                        //DESC IMAGE XQ
                        ws.Cells["A55"].Value = ewo.desc_imagen_3;
                        ws.Cells["F55"].Value = ewo.desc_imagen_4;

                        ws.Cells["B64"].Value = ewo.fecha_ultimo_mtto.Value.ToShortDateString();
                        ws.Cells["H64"].Value = ewo.fecha_proximo_mtto.Value.ToShortDateString();
                        
                        
                        //CUADRO PORQUE PORQUE
                        if (lista_acc.Count > 0)
                        {
                            for (int row = 88; row < lista_acc.Count + 88; row++)
                            {
                                for (int cols = 1; cols <= 4; cols++)
                                {
                                    if (cols == 1)
                                    {
                                        ws.SetValue(row, cols+1, lista_acc[row - 88].accion);
                                    }
                                    if (cols == 2)
                                    {
                                        ws.SetValue(row, cols+5, lista_acc[row - 88].tipo_accion);
                                    }
                                    if (cols == 3)
                                    {
                                        ws.SetValue(row, cols+5,daoTec.GetTecnico(int.Parse(lista_acc[row - 88].responsable)).Nombre);
                                    }
                                    if (cols == 4)
                                    {
                                        ws.SetValue(row, cols+6, lista_acc[row - 88].fecha);
                                    }
                                }
                            }
                        }

                        //RESPONSABLES
                        ws.Cells["A103"].Value = ewo.tecnicos_man_involucrados == null ? "" : ewo.tecnicos_man_involucrados.Replace(",", " - ");
                        ws.Cells["F103"].Value = ewo.operarios_involucrados == null ? "" :ewo.operarios_involucrados.Replace(",", " - ");
                        ws.Cells["A105"].Value = ewo.elaborador_analisis == null ? "" : ewo.elaborador_analisis.Replace(",", " - ");
                        ws.Cells["C105"].Value = ewo.fecha_analisis == null ? "" : ewo.fecha_analisis.Value.ToShortDateString();
                        ws.Cells["D105"].Value = ewo.definidor_contramedidas == null ? "" : ewo.definidor_contramedidas.Replace(",", " - ");
                        ws.Cells["G105"].Value = ewo.fecha_contramedida == null ? "" : ewo.fecha_contramedida.Value.ToShortDateString();
                        ws.Cells["J105"].Value = ewo.fecha_validacion == null ? "" : ewo.fecha_validacion.Value.ToShortDateString();
                        ws.Cells["H105"].Value = ewo.validador_ejecucion == null ? "" : ewo.validador_ejecucion.Replace(",", " - ");

                        excelPackage.SaveAs(fileN);
                    }

                    //ABRIR CON INTEROP PARA DILIGENCIAR CAMPOS PENDIENTES
                    ExcelApp.Application xlApp = new ExcelApp.Application();
                    xlApp.Visible = false;
                    Workbook wb = xlApp.Workbooks.Open(nfilename);
                    Worksheet ws2 = (Worksheet)wb.Worksheets[1];

                    OptionButton opt =
                           (OptionButton)ws2.OptionButtons(ewo.falla_index);
                    opt.Value = true;

                    //CAUSA RAIZ
                    Range rango1 = (Range)ws2.Cells[69, 1];
                    Range rango2 = (Range)ws2.Cells[69, 2];
                    Range rango3 = (Range)ws2.Cells[69, 4];
                    Range rango4 = (Range)ws2.Cells[69, 5];
                    Range rango5 = (Range)ws2.Cells[69, 7];
                    Range rango6 = (Range)ws2.Cells[69, 9];

                    float pri = (float)rango1.Left;
                    float seg = (float)rango2.Left + 28f;
                    float ter = (float)rango3.Left;
                    float cua = (float)rango4.Left + 37f;
                    float qui = (float)rango5.Left + 8f;
                    float sex = (float)rango6.Left;

                    switch (ewo.causa_raiz_index)
                    {
                        case 1:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                pri, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                        case 2:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                seg, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                        case 3:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                ter, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                        case 4:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                cua, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                        case 5:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                qui, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                        case 6:
                            ws2.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval,
                                sex, 1700, 80, 200).Fill.Transparency = 0.89f;
                            break;
                    }
                    

                    wb.Save();
                    wb.Close();

                    r = 1;
                    message = "OK";
                }
                else
                {
                    r = 0;
                    message = "ERROR";
                }      
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                r = -1;
                message = e.ToString();
            }

            GC.Collect();

            return Json(new { code = r, message = message });
        }

        public ActionResult DownloadEwoFile()
        {
            string nfilename = Server.MapPath("~/Content/formats/FORMATO_EWO.XLSX");                       

            return File(nfilename,"application/vnd.ms-excel","Ewo Web Format.xlsx");
        }

        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }

        private void SaveImageEwoServer(HttpPostedFileBase file)
        {
            string nameAndLocation = ewo_images + file.FileName;
            file.SaveAs(Server.MapPath(nameAndLocation));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void CreateRoles()
        {
            //var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));


            //if (!roleManager.RoleExists("SUPER-ADMIN"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "SUPER-ADMIN";
            //    roleManager.Create(role);
            //}

            //if (!roleManager.RoleExists("ADMINISTRADOR"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "ADMINISTRADOR";
            //    roleManager.Create(role);
            //}

            //if (!roleManager.RoleExists("MECÁNICO"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "MECÁNICO";
            //    roleManager.Create(role);
            //}

            //LISTA DE TIPOS DE LÍNEAS
            //kpiWiewModel.TipoLineaList = new SelectList(
            //    new List<SelectListItem>
            //    { new SelectListItem() {Value = "0", Text = "Seleccione tipo de linea" },
            //          new SelectListItem() { Value = "1", Text = "Procesos" },
            //          new SelectListItem() { Value = "1", Text = "Terminales" }
            //    }, "Value", "Text");
        }

        #region DataMethods
        public async Task<JsonResult> GetDropDownListAsync(int from, int id, int id2)
        {
            //FROM 1 - DESDE TIPOS DE LÍNEAS A LINEAS 
            //FROM 2 - DESDE LÍNEAS A MÁQUINAS 
            
            SelectList List = null;

            switch (from)
            {
                case 1:
                    //LISTA DE DE LÍNEAS
                    var listL = await daoLin.GetLinesAsync(id,id2);
                    listL.Insert(0, new Database.lineas() { id = 0, nombre = "Seleccione línea..." });
                    List = new SelectList(listL, "id", "nombre");
                    break;
                case 2:
                    //LISTA DE DE MÁQUINAS
                    var listM = await daoMaq.GetMachinesAsync(id);
                    listM.Insert(0, new Database.maquinas() { Id = 0, nombre = "Seleccione máquina..." });
                    List = new SelectList(listM, "Id", "nombre");
                    break;
            }
            return Json(new SelectList(List, "Value", "Text"));
        }

        public async Task<JsonResult> GetTechnicians()
        {        
            //LISTA DE DE LÍNEAS
            var list = await daoPer.GetPersonalAsync(1);            
                   
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAdmins()
        {
            //LISTA DE DE LÍNEAS
            var list = await daoPer.GetPersonalAsync(2);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllUsersJsonAsync()
        {
            var users = await daoPer.GetAllPersonalAsync();
            return Json(users);
        }
        #endregion
    }
}