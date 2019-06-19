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
    public class DaoSistemas
    {
        public async Task<List<sistemas>> GetSystemsAsync(int maquina)
        {
            List<sistemas> listSystems = new List<sistemas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.sistemas
                                join mq in context.maquinas
                                on td.id_maquina equals mq.Id
                                where td.id_maquina == maquina
                                select td;

                    listSystems = await query.OrderBy(x => x.Nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetSystemsAsync: " + e.ToString();
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

            return listSystems;
        }
    }
}