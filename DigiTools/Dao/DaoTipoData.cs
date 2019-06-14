using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoTipoData
    {
        public async Task<List<tipos_data>> GetTypesAsync(int type)
        {
            List<tipos_data> tiposData = new List<tipos_data>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.tipos_data
                                where td.id_tipo == type
                                select td;

                    tiposData = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetTypesAsync: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                DaoExcepcion.AddException(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = DateTime.Now
                    });
            }

            return tiposData;
        }
    }
}