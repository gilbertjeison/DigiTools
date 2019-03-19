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

namespace DigiTools.Controllers
{
    [Authorize]
    public class KpisController : Controller
    {
        private MttoAppEntities db = new MttoAppEntities();
        DaoPlantas daoPla = new DaoPlantas();
        DaoLineas daoLin = new DaoLineas();
        DaoKpis daoTc = new DaoKpis();

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
