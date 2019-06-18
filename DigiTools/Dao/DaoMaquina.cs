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
    public class DaoMaquina
    {
        public async Task<List<maquinas>> GetMachinesAsync(int line)
        {
            List<maquinas> listMachines = new List<maquinas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.maquinas
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.id_linea == line
                                select td;

                    listMachines = await query.OrderBy(x=>x.nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetMachinesAsync: " + e.ToString();
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

            return listMachines;
        }


        
    }
}