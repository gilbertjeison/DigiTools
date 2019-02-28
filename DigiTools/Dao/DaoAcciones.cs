using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoAcciones
    {
        public async Task<int> AddAcciones(List<lista_acciones> la)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.lista_acciones.AddRange(la);
                    regs = await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error agregando lista de acciones: " + e.ToString());
                regs = -1;
            }

            return regs;
        }
    }
}