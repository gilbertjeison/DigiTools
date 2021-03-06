﻿using DigiTools.Database;
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
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name == null ? "No definido" : HttpContext.Current.User.Identity.Name,
                        descripcion = "Dao Acciones " + e.ToString(),
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }
            
            return regs;
        }

        public async Task<List<lista_acciones>> GetActionsList(int id_ewo)
        {
            List<lista_acciones> list = new List<lista_acciones>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from ru in context.lista_acciones
                                where ru.id_ewo == id_ewo
                                select ru;

                    list = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al momento de consultar lista de acciones: " + e.ToString());
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name == null ? "No definido" : HttpContext.Current.User.Identity.Name,
                        descripcion = "Dao Acciones " + e.ToString(),
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }

            return list;
        }

        public async Task<int> EditAcciones(lista_acciones la)
        {
            lista_acciones lad;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    lad = context.lista_acciones.Where(s => s.Id == la.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (lad != null)
                {
                    lad.accion = la.accion;
                    lad.fecha = la.fecha;
                    lad.id_ewo = la.id_ewo;
                    lad.responsable = la.responsable;
                    lad.tipo_accion = la.tipo_accion;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(lad).State = EntityState.Modified;

                    //4. call SaveChanges
                    regs = await context.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al editar acción: " + e.ToString());
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name == null ? "No definido" : HttpContext.Current.User.Identity.Name,
                        descripcion = "Dao Acciones " + e.ToString(),
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }

            return regs;
        }

        public async Task<int> DeleteAccionFromEwo(int id_ewo)
        {
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    context.lista_acciones.RemoveRange
                        (context.lista_acciones.Where(x => x.id_ewo == id_ewo));

                    regs = context.SaveChangesAsync();
                    return await regs;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Excepción al eliminar acción de lista de acciones: " + e.ToString());
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name == null ? "No definido" : HttpContext.Current.User.Identity.Name,
                        descripcion = "Dao Acciones " + e.ToString(),
                        fecha = SomeHelpers.GetCurrentTime()
                    });
            }
            return 0;
        }
    }
}