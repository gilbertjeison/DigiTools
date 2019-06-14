using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoTecnicos
    {
        public tecnicos GetTecnico(int id)
        {
            tecnicos tec = new tecnicos();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = (from u in context.tecnicos
                                 where u.Id.Equals(id)
                                 select u).FirstOrDefault();

                    tec = query;
                }
            }
            catch (Exception e)
            {
                string err = "Error al obtener técnico: " + e.ToString();
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

            return tec;
        }
    }
}