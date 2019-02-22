using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}