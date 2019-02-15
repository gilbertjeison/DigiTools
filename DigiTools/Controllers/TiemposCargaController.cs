using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DigiTools.Database;
using DigiTools.Dao;
using System.Globalization;
using System.Threading;

namespace DigiTools.Controllers
{
    [Authorize]
    public class TiemposCargaController : Controller
    {
        DaoLineas daoLin = new DaoLineas();
        DaoPlantas daoPla = new DaoPlantas();
        DaoTiemposCarga daoTC = new DaoTiemposCarga();

        private MttoAppEntities db = new MttoAppEntities();

        // GET: TiemposCarga
        public async Task<ActionResult> Index(int? id)
        {     
            //int? idPlantSelected = daoLin.GetLinesById
            //    ((int)id).First().id_planta;

            //LISTA DE PLANTAS
            var listPla = await daoPla.GetPlants();
            listPla.Insert(0, new Database.plantas() { Id = 0, nombre = "Seleccione planta..." });
            ViewBag.PlantasList = new SelectList
                (listPla, "Id", "nombre");

            ////LISTA DE LÍNEAS
            //var listLin = await daoLin.GetLinesAsync((int)idPlantSelected);
            //listLin.Insert(0, new Database.lineas() { id = 0, nombre = "Seleccione línea..." });
            //ViewBag.LineasList = new SelectList
            //    (listLin, "id", "nombre", listLin.Select(x => x.id == id));                               

            //List<int> listA = await daoTC.GetDistinctYearAsync((int)id);
            //ViewBag.AnoList = listA;

            var tiempos_carga = daoTC.GetTiemposCargaAsync(id);                                          

            return View(await tiempos_carga);                           
        }

        [HttpPost]
        public async Task<JsonResult> FilterTCAsync(int id_line, string year)
        {
            var tiempos_carga = daoTC.GetTiemposCargaAsync(id_line,year);
            
            return Json(await tiempos_carga);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTcAsync(int id, string ntc)
        {
            var resUp = daoTC.EditTiempoCarga(id, decimal.Parse(ntc, new CultureInfo("en-US")));
            
            return Json(await resUp);
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
            var listL = await daoTC.GetDistinctYearAsync(id_linea);
            listL.Insert(0, "Seleccione año...");

            return Json(listL);
        }

        [HttpPost]
        public async Task<JsonResult> AddTcAsync(int IdLineaMod, int YearMod)
        {
            
            var tiempos_carga = await daoTC.GetTiemposCargaAsync(IdLineaMod, YearMod.ToString());
            int res = 0;

            //SI ESTÁ COMPLETO EL PERIODO
            if (tiempos_carga.Count == 12)
            {
                res = tiempos_carga.Count;
            }
            else if (tiempos_carga.Count == 0)
            {
                //CREAR TIEMPOS DE CARGA EN TODOS LOS MESES PARA EL AÑO SELECCIONADO
                var r = await daoTC.AddTcAsync(IdLineaMod, YearMod);
                if (r==12)
                {
                    res = 0;
                }
                else
                {
                    res = -1;
                }
            }

            return Json(res);
        }

        // GET: TiemposCarga/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tiempos_carga tiempos_carga = await db.tiempos_carga.FindAsync(id);
            if (tiempos_carga == null)
            {
                return HttpNotFound();
            }
            return View(tiempos_carga);
        }

        // GET: TiemposCarga/Create
        public ActionResult Create()
        {
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre");
            return View();
        }

        // POST: TiemposCarga/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_linea,year,mes")] tiempos_carga tiempos_carga)
        {
            if (ModelState.IsValid)
            {
                db.tiempos_carga.Add(tiempos_carga);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", tiempos_carga.id_linea);
            return View(tiempos_carga);
        }

        // GET: TiemposCarga/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tiempos_carga tiempos_carga = await db.tiempos_carga.FindAsync(id);
            if (tiempos_carga == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", tiempos_carga.id_linea);
            return View(tiempos_carga);
        }

        // POST: TiemposCarga/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,id_linea,year,mes")] tiempos_carga tiempos_carga)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tiempos_carga).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_linea = new SelectList(db.lineas, "id", "nombre", tiempos_carga.id_linea);
            return View(tiempos_carga);
        }

        // GET: TiemposCarga/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tiempos_carga tiempos_carga = await db.tiempos_carga.FindAsync(id);
            if (tiempos_carga == null)
            {
                return HttpNotFound();
            }
            return View(tiempos_carga);
        }

        // POST: TiemposCarga/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tiempos_carga tiempos_carga = await db.tiempos_carga.FindAsync(id);
            db.tiempos_carga.Remove(tiempos_carga);
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
