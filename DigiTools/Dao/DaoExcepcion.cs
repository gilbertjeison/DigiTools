using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DigiTools.Dao
{
    public static class DaoExcepcion
    {
        public static int AddException(excepciones ex)
        {
            int regs = 0;
                        
            using (var context = new MttoAppEntities())
            {
                context.excepciones.Add(ex);
                regs = context.SaveChanges();
            }
            
            return regs;
        }
    }
}