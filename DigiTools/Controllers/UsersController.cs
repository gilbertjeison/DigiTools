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
using System.Linq.Dynamic;
using DigiTools.Models;

namespace DigiTools.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private MttoAppEntities db = new MttoAppEntities();
        DaoUsuarios daoUser = new DaoUsuarios();
        DaoRoles daoRol = new DaoRoles();

        // GET: Users
        public ActionResult Index()
        {
            var viewModel = new RoleViewModel();
            viewModel.RoleList = new List<SelectListItem>();
            var roles = daoRol.GetRoles();

            var RoleList = new List<SelectListItem>();

            //RoleList.Add(new SelectListItem() { Text = "Seleccione un rol...", Value = string.Empty });

            foreach (var item in roles)
            {
                RoleList.Add(new SelectListItem() { Text = item.Name, Value = item.Id });
            }

            viewModel.RoleList = RoleList;

            return View(viewModel);
        }

        public async Task<ActionResult> LoadDataAsync()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"];
                // Skiping number of Rows count
                var start = Request.Form["start"];
                // Paging Length 10,20
                var length = Request.Form["length"];
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"];
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"];

                //Paging Size (10,20,50,100)
                int pageSize = length.ToString() != null ? Convert.ToInt32(length) : 0;
                int skip = start.ToString() != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all user data
                var userData = daoUser.GetUserWOApprv();

                var data1 = await userData;

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    data1 = data1.OrderBy(sortColumn + " " + sortColumnDirection).ToList();
                }

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    data1 = data1.Where(m => m.Nombres.Contains(searchValue) || m.Apellidos.Contains(searchValue)).ToList();
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
        public async Task<ActionResult> Approve(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Users", "Index");
                }

                int result = await daoUser.ApproveUser(id);

                if (result > 0)
                {
                    //ENVIAR CORREO ELECTRÓNICO DE NOTIFICACIÓN
                    var user = daoUser.GetUser(id);
                    await Utils.SomeHelpers.SendGridAsync(2, user.Email, user.Nombres + " " + user.Apellidos);
                    return Json(data: true);
                }
                else
                {
                    return Json(data: false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {            
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Users", "Index");
                }

                var user = daoUser.GetUser(id);

                //ENVIAR CORREO ELECTRÓNICO DE NOTIFICACIÓN DE RECHAZO
                await Utils.SomeHelpers.SendGridAsync(3, user.Email, user.Nombres + " " + user.Apellidos);
                int result = await daoUser.DeleteUser(id);

                if (result > 0)
                {
                    return Json(data: true);
                }
                else
                {
                    return Json(data: false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = await db.AspNetUsers.FindAsync(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nombres,Apellidos,IdRol,TipoUsuario,Registrado,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUsers);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(aspNetUsers);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = await db.AspNetUsers.FindAsync(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nombres,Apellidos,IdRol,TipoUsuario,Registrado,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUsers).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(aspNetUsers);
        }

        // GET: Users/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AspNetUsers aspNetUsers = await db.AspNetUsers.FindAsync(id);
        //    if (aspNetUsers == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aspNetUsers);
        //}

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = await db.AspNetUsers.FindAsync(id);
            db.AspNetUsers.Remove(aspNetUsers);
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
