using DigiTools.Database;
using System;
using System.Collections.Generic;
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
    }
}