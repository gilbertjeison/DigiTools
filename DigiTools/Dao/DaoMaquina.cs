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

                    listMachines = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return listMachines;
        }


        
    }
}