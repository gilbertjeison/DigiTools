using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoPorque
    {
        public async Task<int> AddPorque(List<porques> pqs)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.porques.AddRange(pqs);
                    regs = await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error agregando porques: " + e.ToString());
                regs = -1;
            }

            return regs;
        }
    }
}