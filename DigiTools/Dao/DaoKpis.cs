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
using DigiTools.Utils;

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
                string err = "GetTiemposCargaAsync " + e.ToString();
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
                string err = "GetTiemposCargaAsync " + e.ToString();
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
                string err = "GetTiempoCargaAsync " + e.ToString();
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

            return TC;
        }

        public async Task<decimal> GetTiempoCargaAsync(int line, int month, int year)
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
                                && td.year == year
                                && td.mes == month
                                select new { td, ln };

                    var res = query.ToList();

                    if (res.Count > 0)
                    {
                        var first = res.First();
                        TC = (decimal)first.td.tiempo_carga;
                    }
                }
            }
            catch (Exception e)
            {
                string err = "GetTiempoCarga " + e.ToString();
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

            return TC;
        }

        public async Task<decimal> GetTiempoCargaAsync(int plant, int year)
        {
            decimal TC = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where ln.id_planta == plant
                                && td.year == year                              
                                select new { td, ln };

                    var res = query.ToList();

                    if (res.Count > 0)
                    {
                        var first = res.Sum(x=>x.td.tiempo_carga);
                        TC = (decimal)first;
                    }
                }
            }
            catch (Exception e)
            {
                string err = "GetTiempoCarga " + e.ToString();
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

            return TC;
        }

        public async Task<decimal> GetTiempoCargaAsyncYear(int year)
        {
            decimal TC = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from td in context.kpis
                                join ln in context.lineas
                                on td.id_linea equals ln.id
                                where td.year == year
                                select new { td, ln };

                    var res = query.ToList();

                    if (res.Count > 0)
                    {
                        var first = res.Sum(x => x.td.tiempo_carga);
                        TC = (decimal)first;
                    }
                }
            }
            catch (Exception e)
            {
                string err = "GetTiempoCarga " + e.ToString();
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

            return TC;
        }

        public async Task<List<string>> GetDistinctYearAsync(int? line)
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
                string err = "GetDistinctYearAsync " + e.ToString();
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
                string err = "Excepción al editar tiempos de carga: " + e.ToString();
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

        public async Task<decimal> GetMttrByLineMonthAsync(int line, int month, int year)
        {
            decimal mttr = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                where e.id_area_linea == line
                                && e.notificacion_averia.Value.Month == (month)
                                && e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        if (avrs > 0)
                        {
                            mttr = (decimal)minT / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averias por líneas y mes: " + e.ToString();
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

            return mttr;
        }

        public async Task<decimal> GetMttrByPlantAsync(int plant, int year)
        {
            decimal mttr = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                join p in context.plantas
                                on ln.id_planta equals p.Id
                                where ln.id_planta == plant
                                && e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        if (avrs > 0)
                        {
                            mttr = (decimal)minT / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averías por planta: " + e.ToString();
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

            return mttr;
        }

        public async Task<decimal> GetMttrBySiteAsync(int year)
        {
            decimal mttr = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                where e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        if (avrs > 0)
                        {
                            mttr = (decimal)minT / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averías por site: " + e.ToString();
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

            return mttr;
        }

        public async Task<decimal> GetMtbfByLineMonthAsync(int line, int month, int year)
        {
            decimal mtbf = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                where e.id_area_linea == line
                                && e.notificacion_averia.Value.Month == (month)
                                && e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        var tc = GetTiempoCargaAsync(line, month, year).Result;

                        if (avrs > 0)
                        {
                            mtbf = (tc - ((decimal)minT / 60)) / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averias por líneas y mes (MTBF): " + e.ToString();
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

            return mtbf;
        }

        public async Task<decimal> GetMtbfByPlantAsync(int plant, int year)
        {
            decimal mtbf = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                join p in context.plantas
                                on ln.id_planta equals p.Id
                                where ln.id_planta == plant
                                && e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        var tc = GetTiempoCargaAsync(plant, year).Result;

                        if (avrs > 0)
                        {
                            mtbf = (tc - ((decimal)minT / 60)) / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averias por planta (MTBF): " + e.ToString();
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

            return mtbf;
        }

        public async Task<decimal> GetMtbfBySiteAsync(int year)
        {
            decimal mtbf = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join ln in context.lineas
                                on e.id_area_linea equals ln.id
                                where e.notificacion_averia.Value.Year == (year)
                                select new { e, ln };

                    if (query != null)
                    {
                        var minT = query.ToList().Sum(x => x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        var tc = GetTiempoCargaAsyncYear(year).Result;

                        if (avrs > 0)
                        {
                            mtbf = (tc - ((decimal)minT / 60)) / avrs;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string err = "Error al consultar averias por site (MTBF): " + e.ToString();
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

            return mtbf;
        }
    }
}