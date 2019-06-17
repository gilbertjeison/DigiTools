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
        public static async System.Threading.Tasks.Task<int> AddExceptionAsync(excepciones ex)
        {
            int regs = 0;
                        
            using (var context = new MttoAppEntities())
            {
                context.excepciones.Add(ex);
                regs = context.SaveChanges();
            }

            //ENVIAR CORREO ELECTRONICO A DESARROLLADOR DEL SISTEMA
            await Utils.SomeHelpers.SendGridAsync(4, "gilbertjeison@gmail.com", ex.descripcion);

            return regs;
        }
    }
}