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
            List<MttrViewModel> mVM = new List<MttrViewModel>();

            for (int i = 1; i < 13; i++)
            {
                mVM.Add(new MttrViewModel()
                {
                    Line = id_line,
                    LineName = daoLin.GetLinesById(id_line).First().nombre,
                    Mes = i,
                    MesName = new DateTime(int.Parse(year), i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant(),
                    Year = int.Parse(year),
                    Mttr = daoE.GetMttrByLineMonth(id_line, i, int.Parse(year))
                });
            }

            await Task.Delay(1000);

            return Json(mVM);
        }

        [HttpPost]
        public async Task<JsonResult> CalculateCharDataAsync(int id_line, string year)
        {
            List<EwoTimesViewModel> mVM = new List<EwoTimesViewModel>();

            try
            {                
                var valuesFromDB = daoE.GetEwoTime(id_line, int.Parse(year));

                for (int i = 1; i < 13; i++)
                {
                    var reg = valuesFromDB.Where(x => x.Mes == i).ToList();
                    if (reg.Count == 0)
                    {
                        mVM.Add(new EwoTimesViewModel()
                        {
                            Line = id_line,
                            LineName = daoLin.GetLinesById(id_line).First().nombre,
                            Year = int.Parse(year),
                            EsperaTecnico = 0,
                            Mes = i,
                            MesName = new DateTime(int.Parse(year), i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant()
                        });
                    }
                    else if (reg.Count > 0)
                    {
                        EwoTimesViewModel et = reg.ToList().First();
                        et.Line = id_line;
                        et.LineName = daoLin.GetLinesById(id_line).First().nombre;
                        mVM.Add(et);
                    }
                }

                await Task.Delay(1000);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al calcular CalculateCharDataAsync() " + e.ToString());                
            }
            
            return Json(mVM);
        }

        

        public List<int> GetEwoTimes(EwoTimeOpt opt, int line, string year)
        {
            List<int> values = new List<int>();

            for (int i = 1; i < 13; i++)
            {
                
            }

                return null;
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
