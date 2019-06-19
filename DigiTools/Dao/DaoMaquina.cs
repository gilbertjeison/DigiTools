using DigiTools.Database;
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
    public class DaoMaquina
    {
        DaoSistemas daoSys = new DaoSistemas();

        public async Task<List<maquinas>> GetMachinesAsync(int line)
        {
            List<maquinas> listMachines = new List<maquinas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.maquinas
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.id_linea == line
                                select td;

                    listMachines = await query.OrderBy(x=>x.nombre).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string err = "GetMachinesAsync: " + e.ToString();
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

            return listMachines;
        }

        public async Task<List<MaquinasViewModel>> GetCustomMachinesAsync(int linea)
        {
            List<MaquinasViewModel> list = new List<MaquinasViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.maquinas
                              where b.id_linea == linea
                              select b;

                    var m = await als.OrderBy(x => x.nombre).ToListAsync();

                    foreach (var item in m)
                    {
                        list.Add(new MaquinasViewModel()
                        {
                            Id = item.Id,
                            Nombre = item.nombre,
                            Image = (item.foto_path == null || item.foto_path == "") ? SomeHelpers.IMG_PATH + "default2.jpg" : SomeHelpers.IMG_PATH + item.foto_path,
                            IdPlanta = (int)item.id_planta,
                            DescPlanta = GetPlantDesc(item.id_planta.Value),
                            IdLinea = (int)item.id_linea,
                            ListSistemas = await daoSys.GetSystemsAsync(item.Id)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Excepción al consultar custom máquinas: " + e.ToString();
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

        public async Task<int> AddMachineAsync(MaquinasViewModel mvm)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.maquinas.Add(new maquinas()
                    {
                        id_planta = mvm.IdPlanta,
                        nombre = mvm.Nombre,
                        foto_path = mvm.Image,
                        id_linea = mvm.IdLinea
                    });

                    regs = context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                string err = "Excepción al agregar máquina: " + e.ToString();
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

        public async Task<int> EditMachineAsync(MaquinasViewModel maq)
        {
            maquinas maqe;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    maqe = context.maquinas.Where(s => s.Id == maq.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (maqe != null)
                {
                    maqe.nombre = maq.Nombre;

                    if (maq.Image != null)
                    {
                        maqe.foto_path = maq.Image;
                    }                    
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(maqe).State = EntityState.Modified;

                    //4. call SaveChanges
                    regs = context.SaveChanges();
                }

            }
            catch (Exception e)
            {
                string err = "Excepción al editar máquina: " + e.ToString();
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

        public string GetPlantDesc(int id)
        {
            string desc = string.Empty;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.plantas
                              where b.Id == id
                              select b.nombre;

                    desc = als.Single();
                }
            }
            catch (Exception e)
            {
                string err = "Excepción al consultar planta desc: " + e.ToString();
                Trace.WriteLine(err);
                //REPORTAR ERROR EN LA BASE DE DATOS
               Task.Run(async () => await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    })
                );
            }

            return desc;
        }
    }
}