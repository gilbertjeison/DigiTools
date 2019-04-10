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
    public class DaoRepuestos
    {
        public async Task<int> AddRepUtil(List<repuestos_utilizados> ru)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.repuestos_utilizados.AddRange(ru);
                    regs = await context.SaveChangesAsync();                    
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error agregando Rep utilizados: "+e.ToString());
                regs = -1;
            }

            return regs;
        }

        public async Task<List<repuestos_utilizados>> GetRepUtils(int id_ewo)
        {
            List<repuestos_utilizados> list = new List<repuestos_utilizados>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from ru in context.repuestos_utilizados                                
                                where ru.id_ewo == id_ewo
                                select ru;

                    list = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al momento de consultar repuestos utilizados: " + e.ToString());
            }

            return list;
        }
    }
}