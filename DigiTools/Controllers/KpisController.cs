﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using DigiTools.Database;
using DigiTools.Dao;
using DigiTools.Models;
using System.Globalization;
using System.Diagnostics;

namespace DigiTools.Controllers
{
    [Authorize]
    public class KpisController : Controller
    {
        private MttoAppEntities db = new MttoAppEntities();
        DaoPlantas daoPla = new DaoPlantas();
        DaoLineas daoLin = new DaoLineas();
        DaoKpis daoTc = new DaoKpis();
        DaoEwo daoE = new DaoEwo();

        public enum EwoTimeOpt
        {
            EsperaTec,
            TiempoDiag,
            TiempoRepu,
            TiempoRepa,
            TiempoPru
        }
        
        // GET: Kpis
        public async Task<ActionResult> Index()
        {
            //LISTA DE PLANTAS
            var listPla = await daoPla.GetPlants();
            listPla.Insert(0, new plantas() { Id = 0, nombre = "Seleccione planta..." });
            ViewBag.PlantasList = new SelectList
                (listPla, "Id", "nombre");

            return View();
        }

        public async Task<JsonResult> GetLineAsync(int id)
        {
            SelectList List = null;

            //LISTA DE DE LÍNEAS
            var listL = await daoLin.GetLinesAsync(id);
            listL.Insert(0, new Database.lineas() { id = 0, nombre = "Seleccione línea..." });
            List = new SelectList(listL, "id", "nombre");

            return Json(new SelectList(List, "Value", "Text"));
        }

        public async Task<JsonResult> GetDistinctYearAsync(int id_linea)
        {
            //LISTA DE DE LÍNEAS
            var listL = await daoTc.GetDistinctYearAsync(id_linea);
            listL.Insert(0, "Seleccione año...");

            return Json(listL);
        }

        [HttpPost]
        public async Task<JsonResult> FilterTCAsync(int id_line, string year)
        {
            var tiempos_carga = daoTc.GetTiemposCargaAsync(id_line, year);

            return Json(await tiempos_carga);
        }

        [HttpPost]
        public async Task<JsonResult> CalculateMttrAsync(int id_line, string year)
        {
            int yr = int.Parse(year);
            var lnobja = await daoLin.GetLinesByIdAsync(id_line);
            var lnobj = lnobja.First();
            string lineName = lnobj.nombre;
            int plant = lnobj.id_planta.Value;
            List<MttrViewModel> mVM = new List<MttrViewModel>();

            await Task.Run(() => 
            { 
                for (int i = 1; i < 13; i++)
                {
                    mVM.Add(new MttrViewModel()
                    {
                        Line = id_line,
                        LineName = lineName,
                        Mes = i,
                        MesName = new DateTime(yr, i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")),
                        Year = yr,
                        Mttr = daoTc.GetMttrByLineMonthAsync(id_line, i, yr).Result
                    });
                }

                //MTTR DEL SITIO COMPLETO Y DE LA PLANTA
                Response.Headers["MttrSite"] = daoTc.GetMttrBySiteAsync(yr).Result.ToString("0.##");
                Response.Headers["MttrPlant"] = daoTc.GetMttrByPlantAsync(plant,yr).Result.ToString("0.##");
            });

            return Json(mVM);
        }

        [HttpPost]
        public async Task<JsonResult> CalculateMttrSiteAsync(string year)
        {
            List<MttrViewModel> mVM = new List<MttrViewModel>();

            await Task.Run(() =>
            {                
                mVM.Add(new MttrViewModel()
                {
                    Year = int.Parse(year),
                    Mttr = daoTc.GetMttrBySiteAsync(int.Parse(year)).Result
                });                
            });

            return Json(mVM);
        }

        [HttpPost]
        public async Task<JsonResult> CalculateCharDataAsync(int id_line, string year)
        {
            List<EwoTimesViewModel> mVM = new List<EwoTimesViewModel>();
                        
            try
            {
                await Task.Run(async () => 
                {
                    var valuesFromDB = await daoE.GetEwoTimeAsync(id_line, int.Parse(year));

                    for (int i = 1; i < 13; i++)
                    {
                        var reg = valuesFromDB.Where(x => x.Mes == i).ToList();
                        if (reg.Count == 0)
                        {
                            mVM.Add(new EwoTimesViewModel()
                            {
                                Line = id_line,
                                LineName = daoLin.GetLinesByIdAsync(id_line).Result.First().nombre,
                                Year = int.Parse(year),
                                Mes = i,
                                MesName = new DateTime(int.Parse(year), i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant()
                            });
                        }
                        else if (reg.Count > 0)
                        {
                            EwoTimesViewModel et = reg.ToList().First();
                            et.Line = id_line;
                            et.LineName = daoLin.GetLinesByIdAsync(id_line).Result.First().nombre;
                            mVM.Add(et);
                        }
                    }
                });                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al calcular CalculateCharDataAsync() " + e.ToString());                
            }
            
            return Json(mVM);
        }

        [HttpPost]
        public async Task<JsonResult> CalculateCharDataMAsync(int id_line, string year)
        {
            List<EwoTimesViewModelM> mVM = new List<EwoTimesViewModelM>();

            try
            {
                await Task.Run(async () =>
                {
                    var valuesFromDB = await daoE.GetEwoCCRAsync(id_line, int.Parse(year));

                    for (int i = 1; i < 13; i++)
                    {
                        var reg = valuesFromDB.Where(x => x.Mes == i).ToList();
                        if (reg.Count == 0)
                        {
                            mVM.Add(new EwoTimesViewModelM()
                            {
                                Line = id_line,
                                LineName = daoLin.GetLinesByIdAsync(id_line).Result.First().nombre,
                                Year = int.Parse(year),                              
                                Mes = i,
                                MesName = new DateTime(int.Parse(year), i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant()
                            });
                        }
                        else if (reg.Count > 0)
                        {
                            EwoTimesViewModelM et = reg.ToList().First();
                            et.Line = id_line;
                            et.LineName = daoLin.GetLinesByIdAsync(id_line).Result.First().nombre;
                            mVM.Add(et);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al calcular CalculateCharDataMAsync() " + e.ToString());
            }

            return Json(mVM);
        }


        //MTBF
        [HttpPost]
        public async Task<JsonResult> CalculateMtbfAsync(int id_line, string year)
        {
            int yr = int.Parse(year);
            var lnobj = daoLin.GetLinesByIdAsync(id_line).Result.First();
            string lineName = lnobj.nombre;
            int plant = lnobj.id_planta.Value;
            List<MtbfViewModel> mVM = new List<MtbfViewModel>();

            await Task.Run(() =>
            {
                for (int i = 1; i < 13; i++)
                {
                    mVM.Add(new MtbfViewModel()
                    {
                        Line = id_line,
                        LineName = lineName,
                        Mes = i,
                        MesName = new DateTime(yr, i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")),
                        Year = yr,
                        Mtbf = daoTc.GetMtbfByLineMonthAsync(id_line, i, yr).Result
                    });
                }

                //MTTR DEL SITIO COMPLETO Y DE LA PLANTA
                Response.Headers["MtbfSite"] = daoTc.GetMtbfBySiteAsync(yr).Result.ToString("0.##");
                Response.Headers["MtbfPlant"] = daoTc.GetMtbfByPlantAsync(plant, yr).Result.ToString("0.##");
            });

            return Json(mVM);
        }


        // GET: Kpis/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            kpis kpis = await db.kpis.FindAsync(id);
            if (kpis == null)
            {
                return HttpNotFound();
            }
            return View(kpis);
        }

        // GET: Kpis/Create
        public ActionResult Create()
        {
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre");
            return View();
        }

        // POST: Kpis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,id_linea,year,mes,tiempo_carga,mttr,mtbf")] kpis kpis)
        {
            if (ModelState.IsValid)
            {
                db.kpis.Add(kpis);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", kpis.id_linea);
            return View(kpis);
        }

        // GET: Kpis/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            kpis kpis = await db.kpis.FindAsync(id);
            if (kpis == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", kpis.id_linea);
            return View(kpis);
        }

        // POST: Kpis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,id_linea,year,mes,tiempo_carga,mttr,mtbf")] kpis kpis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kpis).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", kpis.id_linea);
            return View(kpis);
        }

        // GET: Kpis/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            kpis kpis = await db.kpis.FindAsync(id);
            if (kpis == null)
            {
                return HttpNotFound();
            }
            return View(kpis);
        }

        // POST: Kpis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            kpis kpis = await db.kpis.FindAsync(id);
            db.kpis.Remove(kpis);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
