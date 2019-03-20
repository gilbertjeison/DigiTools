using DigiTools.Database;
using DigiTools.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MoreLinq.Extensions;
using System.Data.Entity;

namespace DigiTools.Dao
{
    public class DaoKpis
    {
        DaoEwo daoE = new DaoEwo();
        public async Task<List<TiemposCargaViewModel>> GetTiemposCargaAsync(Nullable<int> line)
        {
            List<TiemposCargaViewModel> listTC = new List<TiemposCargaViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.id_linea == line       
                                orderby td.mes ascending
                                select new {td,ln};

                    var res =await query.ToListAsync();
                    var res2 = res.MaxBy(x=> x.td.year);                                     

                    foreach (var s in res2)
                    {
                        listTC.Add(new TiemposCargaViewModel()
                        {
                            Id = s.td.id,
                            IdLinea = (int)s.td.id_linea,
                            Mes = (int)s.td.mes,
                            TiempoCarga = (decimal)s.td.tiempo_carga,
                            Year = (int)s.td.year,
                            LineName = s.ln.nombre,
                            MesName = new DateTime((int)s.td.year, (int)s.td.mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return listTC;
        }

        public async Task<List<TiemposCargaViewModel>> GetTiemposCargaAsync(int line, string year)
        {
            List<TiemposCargaViewModel> listTC = new List<TiemposCargaViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.id_linea == line
                                && td.year.ToString().Equals(year)
                                select new { td, ln };

                    var res = await query.ToListAsync();

                    
                    foreach (var s in res)
                    {
                        listTC.Add(new TiemposCargaViewModel()
                        {
                            Id = s.td.id,
                            IdLinea = (int)s.td.id_linea,
                            Mes = (int)s.td.mes,
                            TiempoCarga = (decimal)s.td.tiempo_carga,
                            Year = (int)s.td.year,
                            LineName = s.ln.nombre,
                            MesName = new DateTime((int)s.td.year, (int)s.td.mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))
                        });
                    }     
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return listTC.OrderBy(x => x.Mes).ToList();
        }
              
        public async Task<decimal> GetTiempoCargaAsync(int line)
        {
            decimal TC = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.id_linea == line
                                && td.year.ToString().Equals(DateTime.Now.Year.ToString())
                                && td.mes == DateTime.Now.Month
                                select new { td, ln };

                    var res = await query.ToListAsync();

                    if (res.Count > 0)
                    {
                        var first = res.First();
                        TC = (decimal)first.td.tiempo_carga;
                    }
                                 
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return TC;
        }

        public async Task<List<string>> GetDistinctYearAsync(Nullable<int> line)
        {
            List<string> listTC = new List<string>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis  
                                where td.id_linea == line
                                select td.year;

                    var res = query.Distinct().ToList();

                    await Task.Run(() => Parallel.ForEach(res, s =>
                    {
                        listTC.Add(s.ToString());
                    }));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return listTC;
        }
       
        public async Task<int> EditTiempoCarga(int id, decimal ntc)
        {
            kpis tc;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    tc = context.kpis.Where(s => s.id == id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (tc != null)
                {
                    tc.tiempo_carga = ntc;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(tc).State = System.Data.Entity.EntityState.Modified;

                    //4. call SaveChanges
                    regs = await context.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al editar tiempos de carga: " + e.ToString());
            }

            return regs;
        }

        public async Task<int> AddTcAsync(int IdLinea, int Year)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    for (int i = 0; i < 12; i++)
                    {
                        context.kpis.Add(new kpis()
                        {
                            id_linea = IdLinea,
                            year = Year,
                            mes = i + 1,
                            tiempo_carga = 0
                        });

                        await context.SaveChangesAsync();
                        regs++;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al agregar línea: " + e.ToString());
            }
            return regs;
        }
    }
}