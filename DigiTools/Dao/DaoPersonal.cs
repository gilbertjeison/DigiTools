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
                Debug.WriteLine(e.ToString());
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
                Debug.WriteLine(e.ToString());
            }

            return listTecs.OrderBy(x => x.Nombre).ToList();
        }
    }
}