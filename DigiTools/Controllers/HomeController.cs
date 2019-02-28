using DigiTools.Dao;
using DigiTools.Database;
using DigiTools.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
        int Consecutivo = 0;
        AspNetUsers aspNetUsers;
        
        private static readonly int LINES = 21;
        private static readonly int BREAKDOWNSTYPES = 13;

        public async Task<ActionResult> Index()
        {
            aspNetUsers = daoUser.GetUser(User.Identity.GetUserId());

            
            if (aspNetUsers.IdRol.Equals("65b01f2a-0b46-4d0c-a227-304dc22e2f9d"))
            {
                var kpiWiewModel = new KpiViewModel();

                //LISTA DE PLANTAS
                var list = await daoPlantas.GetPlants();
                list.Insert(0, new Database.plantas() { Id = 0, nombre = "Seleccione planta..." });
                kpiWiewModel.PlantaList = new SelectList(list, "Id", "nombre");

                //LISTA DE TIPOS DE LÍNEA
                var listTL = await daoTD.GetTypesAsync(LINES);
                listTL.Insert(0, new Database.tipos_data() { Id = 0, descripcion = "Seleccione tipo de línea..." });
                kpiWiewModel.TipoLineaList = new SelectList(listTL, "Id", "descripcion");

                //LISTA DE TIPOS DE AVERÍAS
                var listA = await daoTD.GetTypesAsync(BREAKDOWNSTYPES);
                listA.Insert(0, new Database.tipos_data() { Id = 0, descripcion = "Seleccione tipo de avería..." });
                kpiWiewModel.TipoAveriaList = new SelectList(listA, "Id", "descripcion");

                kpiWiewModel.IdDiligenciado = aspNetUsers.Nombres +" " + aspNetUsers.Apellidos;
                Consecutivo = daoEwo.GetLastConsecutive();
                ViewBag.Cons = "00" +Consecutivo;

                return View(kpiWiewModel);
            }
            else
            {
                return View("IndexAdmin");
            }            
        }

        [HttpPost]
        public async Task<int> CreateKpi(KpiViewModel kvm)
        {
            try
            {
                ewos ewo = new ewos();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<repuestos_utilizados> rep_Utils = ser.Deserialize<List<repuestos_utilizados>>(kvm.RepUtil);
                List<porques> porques = ser.Deserialize<List<porques>>(kvm.Porques);
                List<lista_acciones> lista_acc = ser.Deserialize<List<lista_acciones>>(kvm.Cmd);

                ewo.consecutivo = daoEwo.GetLastConsecutive();
                ewo.id_area_linea = kvm.IdLinea;
                ewo.id_equipo = kvm.IdMaquina;
                ewo.fecha_ewo = DateTime.Now;
                ewo.aviso_numero = kvm.NumAviso;
                ewo.id_tecnico = daoUser.GetUser(User.Identity.GetUserId()).Id;
                ewo.id_tipo_averia = kvm.IdTipoAveria;
                ewo.id_turno = kvm.Turno;

                //CONVERTIR FECHAS A DATETIME
                ewo.notificacion_averia = DateTime.ParseExact(kvm.HrNotAve, "dd-MM-yyyy HH:mm",CultureInfo.InvariantCulture);
                ewo.inicio_reparacion = DateTime.ParseExact(kvm.HrIniRep, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                ewo.fin_reparacion = DateTime.ParseExact(kvm.HrFinRepEnt, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                ewo.fecha_ultimo_mtto = DateTime.ParseExact(kvm.FchUltimoMtto, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_proximo_mtto = DateTime.ParseExact(kvm.FchProxMtto, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_analisis = DateTime.ParseExact(kvm.FchAnaElab, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_validacion = DateTime.ParseExact(kvm.FchEjeVal, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ewo.fecha_contramedida = DateTime.ParseExact(kvm.FchDefConMed, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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
                ewo.genjitsu_ok = kvm != null ? true : false;
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
                }
                                

                
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                //return -1;
            }
            
            return 1;
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