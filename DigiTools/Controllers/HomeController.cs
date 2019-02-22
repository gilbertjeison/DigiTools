using DigiTools.Dao;
using DigiTools.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        DaoEwo daoEwo = new DaoEwo();
        
        private static readonly int LINES = 21;
        private static readonly int BREAKDOWNSTYPES = 13;

        public async Task<ActionResult> Index()
        {
            var Uss = daoUser.GetUser(User.Identity.GetUserId());

            
            if (Uss.IdRol.Equals("65b01f2a-0b46-4d0c-a227-304dc22e2f9d"))
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

                ViewBag.Cons = "00" +daoEwo.GetLastConsecutive();

                return View(kpiWiewModel);
            }
            else
            {
                return View("IndexAdmin");
            }            
        }

        [HttpPost]
        public int CreateKpi(KpiViewModel collection)
        {
            var fc = collection;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<rep_util> rep_Utils = ser.Deserialize<List<rep_util>>(collection.RepUtil);
            //to do
            //EVALUAR EL RESULTADO DE CADA CHECK Y CONVERTIRLO A BOOL
            //INTERACTUAR CON CADA UNA DE LAS LISTAS DE ITEMS PARA INGRESAR EN SU DETERMINADA TABLA
            //CONVERTIR FECHAS A DATETIME


            var images = Request.Files;
            return 0;
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