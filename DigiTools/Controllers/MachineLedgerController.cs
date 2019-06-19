using DigiTools.Dao;
using DigiTools.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigiTools.Controllers
{
    [Authorize]
    public class MachineLedgerController : Controller
    {
        private static readonly int LINES = 21;

        DaoTipoData daoTD = new DaoTipoData();
        DaoPlantas daoPlantas = new DaoPlantas();
        DaoLineas daoLin = new DaoLineas();
        DaoMaquina daoMaq = new DaoMaquina();
        // GET: MachineLedger
        public async Task<ActionResult> Index()
        {
            ViewBag.CurrentPlant = 1;
            

            //LISTA DE TIPOS DE LÍNEA
            var listTL = await daoTD.GetTypesAsync(LINES);
            listTL.Insert(0, new Database.tipos_data() { Id = 0, descripcion = "Seleccione tipo de línea..." });
            ViewBag.TipoLineaList = new SelectList(listTL, "Id", "descripcion");

            return View();
        }

        #region QUERY
        public async Task<ActionResult> GetPlantsView()
        {
            var plList = await daoPlantas.GetCustomPlantsAsync();
            
            ViewBag.Planta = "MACHINE LEDGER";            
            
            return PartialView("_Plantas", plList);
        }

        //[HttpGet]
        public async Task<ActionResult> GetLinesView(int planta, string nombre)
        {
            var linList = await daoLin.GetCustomLinesAsync(planta);
            
            ViewBag.Planta = nombre;            

            Session["id_planta"] = planta;
            Trace.WriteLine("Nombre de la planta: " + nombre);
            
            return PartialView("_Lineas", linList);
        }


        public async Task<ActionResult> GetMachinesView(int linea, string nombre)
        {
            var maqList = await daoMaq.GetCustomMachinesAsync(linea);
            ViewBag.Linea = nombre;
            Session["id_linea"] = linea;
                      

            return PartialView("_Maquinas", maqList);
        }
        #endregion

        #region EXECUTIONS
        [HttpPost]
        public async Task<JsonResult> AddLine(LineasViewModel lineasViewModel)
        {
            try
            {
                string imageName = string.Empty;

                if (lineasViewModel.Images != null)
                {
                    imageName = Save_File(lineasViewModel.Images.FileName);
                    lineasViewModel.Image = imageName;
                }                                

                lineasViewModel.IdPlanta = int.Parse(Session["id_planta"].ToString());
                
                int res = await daoLin.AddLineAsync(lineasViewModel);
                if (res == 1)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("No se pudo guardar línea, intente de nuevo...");
                }
                
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Error al agregar línea" + e.Message;
                return Json(e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddMachine(MaquinasViewModel mVM)
        {
            try
            {
                string imageName = string.Empty;

                if (mVM.Images != null)
                {
                    imageName = Save_File(mVM.Images.FileName);
                    mVM.Image = imageName;
                }

                mVM.IdPlanta = int.Parse(Session["id_planta"].ToString());
                mVM.IdLinea = int.Parse(Session["id_linea"].ToString());

                int res = await daoMaq.AddMachineAsync(mVM);
                if (res == 1)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("No se pudo guardar máquina, intente de nuevo...");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Error al agregar máquina" + e.Message;
                return Json(e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditLine(LineasViewModel lineasViewModel)
        {
            try
            {
                string imageName = string.Empty;

                if (lineasViewModel.Images != null)
                {
                    imageName = Save_File(lineasViewModel.Images.FileName);
                    lineasViewModel.Image = imageName;
                }                

                lineasViewModel.IdPlanta = int.Parse(Session["id_planta"].ToString());
                

                int res = await daoLin.EditLineaAsync(lineasViewModel);
                if (res == 1)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("No se pudo editar la línea, intente de nuevo...");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Error al agregar línea" + e.ToString();
                return Json(e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditMachine(MaquinasViewModel maqsViewModel)
        {
            try
            {
                string imageName = string.Empty;

                if (maqsViewModel.Images != null)
                {
                    imageName = Save_File(maqsViewModel.Images.FileName);
                    maqsViewModel.Image = imageName;
                }                

                int res = await daoMaq.EditMachineAsync(maqsViewModel);
                if (res == 1)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("No se pudo editar la máquina, intente de nuevo...");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Error al editar máquina" + e.ToString();
                return Json(e.Message);
            }
        }

        public string Save_File(string data_file)
        {
            Guid g =  Guid.NewGuid();
           
            string[] nombre = data_file.Replace(" ", "").Split("\\".ToCharArray());

            string oriName = nombre[nombre.Length - 1];

            string nameFile = g.ToString() + "_" + nombre[nombre.Length - 1];


            var uploadedFile = Request.Files[0];
            
            string uploadsFilesPath = Server.MapPath("~/Content/images/v1/");

            if (!Directory.Exists(uploadsFilesPath))
                Directory.CreateDirectory(uploadsFilesPath);

            var filePath = Path.Combine(uploadsFilesPath, nameFile);

            uploadedFile.SaveAs(filePath);

            return nameFile;
        }
        #endregion
        // GET: MachineLedger/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MachineLedger/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MachineLedger/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MachineLedger/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MachineLedger/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MachineLedger/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MachineLedger/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
