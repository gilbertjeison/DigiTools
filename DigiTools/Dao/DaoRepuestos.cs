using DigiTools.Database;
using DigiTools.Utils;
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
                regs = -1;
                string err = "Error agregando Rep utilizados: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
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
                string err = "Excepción al momento de consultar repuestos utilizados: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }

            return list;
        }

        public async Task<int> EditRepuesto(repuestos_utilizados ru)
        {
            repuestos_utilizados rud;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    rud = context.repuestos_utilizados.Where(s => s.Id == ru.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (rud != null)
                {
                    rud.descripcion = ru.descripcion;
                    rud.codigo_sap = ru.codigo_sap;
                    rud.cantidad_ewo = ru.cantidad_ewo;
                    rud.costo = ru.costo;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(rud).State = EntityState.Modified;

                    //4. call SaveChanges
                    regs = await context.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                string err = "Excepción al editar repuesto utilizado: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }

            return regs;
        }

        public async Task<int> DeleteRepuestosFromEwo(int id_ewo)
        {
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    context.repuestos_utilizados.RemoveRange
                        (context.repuestos_utilizados.Where(x => x.id_ewo == id_ewo));

                    regs = context.SaveChangesAsync();
                    return await regs;
                }                
            }
            catch (Exception e)
            {
                string err = "Excepción al eliminar repuestos utilizados: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }
            return 0;
        }
    }
}