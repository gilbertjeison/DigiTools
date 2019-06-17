using DigiTools.Database;
using DigiTools.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoRoles
    {
        public async System.Threading.Tasks.Task<List<AspNetRoles>> GetRolesAsync()
        {
            List<AspNetRoles> roles = new List<AspNetRoles>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from u in context.AspNetRoles
                                where !u.Id.Equals("2b0bf2ba-c2a3-4f19-84d7-9288c0bc2895")
                                select u;

                    roles = query.ToList();
                }

            }
            catch (Exception e)
            {
                string err = "GetRoles: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }

            return roles;
        }
    }
}