﻿using DigiTools.Dao;
using DigiTools.Models;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult> GetLinesView(int planta)
        {
            var linList = await daoLin.GetCustomLinesAsync(planta);
            if (planta == 1)
            {
                ViewBag.Planta = "planta de jabones";
            }
            else
            {
                ViewBag.Planta = "planta de detergentes";
            }

            Session["id_planta"] = planta;


            return PartialView("_Lineas", linList);
        }
        #endregion

        #region EXECUTIONS
        [HttpPost]
        public JsonResult AddLine(LineasViewModel lineasViewModel)
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
                
                int res = daoLin.AddLine(lineasViewModel);
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
        public JsonResult EditLine(LineasViewModel lineasViewModel)
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
                

                int res = daoLin.EditLinea(lineasViewModel);
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
