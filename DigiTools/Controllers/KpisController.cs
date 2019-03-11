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

namespace DigiTools.Controllers
{
    public class KpisController : Controller
    {
        private MttoAppEntities db = new MttoAppEntities();

        // GET: Kpis
        public async Task<ActionResult> Index()
        {
            var kpis = db.kpis.Include(k => k.lineas);
            return View(await kpis.ToListAsync());
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
