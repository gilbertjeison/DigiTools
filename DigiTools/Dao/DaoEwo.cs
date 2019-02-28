using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoEwo
    {
        public int GetLastConsecutive()
        {
            int max = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    max = (int)context.ewos.OrderByDescending(u => u.Id).FirstOrDefault().consecutivo + 1;
                }
            }
            catch (Exception e)
            {
                max = -1;
            }

            return max;
        }

        public async Task<int> AddEwo(ewos ewo)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.ewos.Add(ewo);
                    await context.SaveChangesAsync();
                    regs = ewo.Id;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                regs = -1;
            }
            return regs;
        }
    }
}