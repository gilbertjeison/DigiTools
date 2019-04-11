using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using DigiTools.Database;
using DigiTools.Dao;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using DigiTools.Models;
using DigiTools.Utils;

namespace DigiTools.Controllers
{
    [Authorize]
    public class EwosController : Controller
    {
        DaoUsuarios daoUser = new DaoUsuarios();
        DaoEwo daoE = new DaoEwo();
        private MttoAppEntities db = new MttoAppEntities();

        // GET: Ewos
        public async Task<ActionResult> Index()
        {
            return View(await db.ewos.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> LoadDataAsync()
        {
            Task<List<KpiViewModel>> kvm;

            try
            {
                AspNetUsers aspNetUsers = daoUser.GetUser(User.Identity.GetUserId());

                var draw = Request.Form["draw"];
                var start = Request.Form["start"];
                var length = Request.Form["length"];
                var sortColumn = Request.Form["columns["+Request.Form["order[0][column]"]+"][name]"];
                var sortColumnDirection = Request.Form["order[0][dir]"];
                var searchValue = Request.Form["search[value]"];

                //Paging Size (10,20,50,100)
                int pageSize = length.ToString() != null ? Convert.ToInt32(length) : 0;
                int skip = start.ToString() != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (aspNetUsers.IdRol.Equals(Utils.SomeHelpers.ROL_MEC))
                {
                    kvm = daoE.GetEwoList(aspNetUsers.Id);
                }
                else
                {
                    kvm = daoE.GetEwoList();
                }                               

                var data1 = await kvm;

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    data1 = data1.OrderBy(sortColumn + " " + sortColumnDirection).ToList();
                }

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    data1 = data1.Where(m => m.Equipo.Contains(searchValue) || m.AreaLinea.Contains(searchValue)).ToList();
                }

                //total number of rows count 
                recordsTotal = data1.Count();
                //Paging 
                var data = data1.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }            
        }

        [HttpPost]
        public async Task<ActionResult> GenerateEwoFile(int id)
        {
            string nfilename = await SomeHelpers.GenerateEwoFile(id);
            
            
            if (!nfilename.Equals("-1") && !nfilename.Equals("-2")) 
            {
                return Json(new { message = "1" });                
            }
            else
            {
                return Json(new { message = nfilename});
            }            
        }

        public ActionResult DownloadEwoFile()
        {
            string nfilename = Server.MapPath("~/Content/formats/FORMATO_EWO.XLSX");

            return File(nfilename, "application/vnd.ms-excel", "Ewo Web Format.xlsx");
        }
        // GET: Ewos/Details/5
        public ActionResult Details()
        {            
            return View();
        }

        // GET: Ewos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ewos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,consecutivo,id_area_linea,id_equipo,fecha_ewo,aviso_numero,id_tecnico,id_tipo_averia,id_turno,notificacion_averia,inicio_reparacion,tiempo_espera_tecnico,tiempo_diagnostico,tiempo_espera_repuestos,tiempo_reparacion,tiempo_pruebas,fin_reparacion,tiempo_total,imagen_1,imagen_2,desc_imagen_1,desc_imagen_2,desc_averia,cambio_componente,ajuste,what,where,when,who,wich,how,fenomeno,gemba,gemba_ok,gembutsu,gembutsu_ok,genjitsu,genjitsu_ok,genri,genri_ok,gensoku,gensoku_ok,imagen_3,imagen_4,desc_imagen_3,desc_imagen_4,fecha_ultimo_mtto,fecha_proximo_mtto,falla_index,causa_raiz_index,tecnicos_man_involucrados,operarios_involucrados,elaborador_analisis,fecha_analisis,definidor_contramedidas,fecha_contramedida,validador_ejecucion,fecha_validacion,user_id,kpi")] ewos ewos)
        {
            if (ModelState.IsValid)
            {
                db.ewos.Add(ewos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ewos);
        }

        // GET: Ewos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ewos ewos = await db.ewos.FindAsync(id);
            if (ewos == null)
            {
                return HttpNotFound();
            }
            return View(ewos);
        }

        // POST: Ewos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,consecutivo,id_area_linea,id_equipo,fecha_ewo,aviso_numero,id_tecnico,id_tipo_averia,id_turno,notificacion_averia,inicio_reparacion,tiempo_espera_tecnico,tiempo_diagnostico,tiempo_espera_repuestos,tiempo_reparacion,tiempo_pruebas,fin_reparacion,tiempo_total,imagen_1,imagen_2,desc_imagen_1,desc_imagen_2,desc_averia,cambio_componente,ajuste,what,where,when,who,wich,how,fenomeno,gemba,gemba_ok,gembutsu,gembutsu_ok,genjitsu,genjitsu_ok,genri,genri_ok,gensoku,gensoku_ok,imagen_3,imagen_4,desc_imagen_3,desc_imagen_4,fecha_ultimo_mtto,fecha_proximo_mtto,falla_index,causa_raiz_index,tecnicos_man_involucrados,operarios_involucrados,elaborador_analisis,fecha_analisis,definidor_contramedidas,fecha_contramedida,validador_ejecucion,fecha_validacion,user_id,kpi")] ewos ewos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ewos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ewos);
        }

        // GET: Ewos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ewos ewos = await db.ewos.FindAsync(id);
            if (ewos == null)
            {
                return HttpNotFound();
            }
            return View(ewos);
        }

        // POST: Ewos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ewos ewos = await db.ewos.FindAsync(id);
            db.ewos.Remove(ewos);
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
