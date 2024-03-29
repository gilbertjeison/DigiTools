﻿using DigiTools.Database;
using DigiTools.Models;
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
    public class DaoLineas
    {
        
        DaoMaquina daoMaq = new DaoMaquina();
        DaoKpis daoTc = new DaoKpis();
        DaoSistemas daoSys = new DaoSistemas();

        public async Task<List<lineas>> GetLinesAsync(int plant,int type)
        {
            List<lineas> listLines = new List<lineas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.lineas
                                where td.id_planta == plant
                                && td.id_tipo_linea == type
                                select td;

                    listLines = await query.OrderBy(x => x.nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {                
                string err = "GetLinesAsync: " + e.ToString();
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

            return listLines;
        }

        public async Task<List<lineas>> GetLinesAsync(int plant)
        {
            List<lineas> listLines = new List<lineas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.lineas
                                where td.id_planta == plant
                                select td;

                    listLines = await query.OrderBy(x => x.nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetLinesAsync: " + e.ToString();
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

            return listLines;
        }

        public async Task<List<lineas>> GetLinesAsyncById(int id)
        {
            List<lineas> listLines = new List<lineas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.lineas
                                where td.id == id
                                select td;

                    listLines = await query.OrderBy(x => x.nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetLinesAsyncById: " + e.ToString();
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

            return listLines;
        }

        public async Task<List<lineas>> GetLinesByIdAsync(int id)
        {
            List<lineas> listLines = new List<lineas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.lineas
                                where td.id == id
                                select td;

                    listLines = query.OrderBy(x => x.nombre).ToList();
                }
            }
            catch (Exception e)
            {
                string err = "GetLinesById: " + e.ToString();
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

            return listLines;
        }

        public async Task<List<LineasViewModel>> GetCustomLinesAsync(int planta)
        {
            List<LineasViewModel> list = new List<LineasViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.lineas
                              where b.id_planta == planta
                              select b;

                    var l = await als.OrderBy(x => x.nombre).ToListAsync();

                    foreach (var item in l)
                    {
                        list.Add(new LineasViewModel()
                        {
                            Id = item.id,
                            Nombre = item.nombre,
                            Image = (item.image_path == null) ? SomeHelpers.IMG_PATH + "default2.jpg" : SomeHelpers.IMG_PATH + item.image_path,
                            IdPlanta = (int)item.id_planta,
                            TipoLinea = (int)item.id_tipo_linea,
                            ListMaquinas = await daoMaq.GetMachinesAsync(item.id),
                            TiempoCarga = await daoTc.GetTiempoCargaAsync(item.id)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Excepción al consultar custom lineas: " + e.ToString();
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
       
        public async Task<int> AddLineAsync(LineasViewModel lin)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.lineas.Add(new lineas()
                    {
                        id_planta = lin.IdPlanta,
                        nombre = lin.Nombre,
                        image_path = lin.Image,
                        id_tipo_linea = lin.TipoLinea,
                        tiempo_carga = 0
                    });

                    regs = context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                string err = "Excepción al agregar línea: " + e.ToString();
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

        public async Task<int> EditLineaAsync(LineasViewModel lin)
        {
            lineas line;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    line = context.lineas.Where(s => s.id == lin.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (line != null)
                {
                    line.nombre = lin.Nombre;

                    if (lin.Image != null)
                    {
                        line.image_path = lin.Image;
                    }
                    
                    line.id_planta = lin.IdPlanta;
                    line.id_tipo_linea = lin.TipoLinea;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(line).State = System.Data.Entity.EntityState.Modified;

                    //4. call SaveChanges
                    regs = context.SaveChanges();
                }

            }
            catch (Exception e)
            {
                string err = "Excepción al editar linea: " + e.ToString();
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
    }
}