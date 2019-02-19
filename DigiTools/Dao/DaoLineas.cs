using DigiTools.Database;
using DigiTools.Models;
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
        static string IMG_PATH = "~/Content/images/v1/";
        DaoMaquina daoMaq = new DaoMaquina();
        DaoKpis daoTc = new DaoKpis();

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

                    listLines = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
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

                    listLines = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
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

                    listLines = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return listLines;
        }

        public List<lineas> GetLinesById(int id)
        {
            List<lineas> listLines = new List<lineas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.lineas
                                where td.id == id
                                select td;

                    listLines = query.ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
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

                    var l = await als.ToListAsync();

                    foreach (var item in l)
                    {
                        list.Add(new LineasViewModel()
                        {
                            Id = item.id,
                            Nombre = item.nombre,
                            Image = (item.image_path == null) ? IMG_PATH + "default2.jpg" : IMG_PATH + item.image_path,
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
                Debug.WriteLine("Excepción al consultar custom lineas: " + e);
            }

            return list;
        }

        public int AddLine(LineasViewModel lin)
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
                        id_tipo_linea = lin.TipoLinea
                    });

                    regs = context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al agregar línea: " + e.ToString());
            }
            return regs;
        }

        public int EditLinea(LineasViewModel lin)
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
                Debug.WriteLine("Excepción al editar linea: " + e.ToString());
            }
            return regs;
        }
    }
}