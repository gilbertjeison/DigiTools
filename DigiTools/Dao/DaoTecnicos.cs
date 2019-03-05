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
                Debug.WriteLine("Error al obtener técnico: "+e.ToString());
            }

            return tec;
        }
    }
}