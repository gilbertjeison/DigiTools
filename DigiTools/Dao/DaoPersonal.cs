using DigiTools.Database;
using DigiTools.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoPersonal
    {
        public async Task<List<tecnicos>> GetPersonalAsync(int role)
        {
            List<tecnicos> listTecs = new List<tecnicos>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.tecnicos
                                where td.id_rol == role
                                && td.tipo_usuario == 101 //TECNICOS
                                select td;

                    listTecs = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetPersonalAsync: " + e.ToString();
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

            return listTecs;
        }

        public async Task<List<tecnicos>> GetAllPersonalAsync()
        {
            List<tecnicos> listTecs = new List<tecnicos>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.tecnicos
                                where td.tipo_usuario == 101 //TECNICOS
                                select td;

                   
                    foreach (var item in query)
                    {
                        item.Nombre = item.Nombre.Trim();
                    }

                    
                    listTecs = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetAllPersonalAsync: " + e.ToString();
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

            return listTecs.OrderBy(x => x.Nombre).ToList();
        }
    }
}