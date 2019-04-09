using DigiTools.Database;
using DigiTools.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoEwo
    {

        public async Task<int> GetLastConsecutive()
        {
            int max = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var maxv = await context.ewos.OrderByDescending(u => u.Id).FirstOrDefaultAsync();
                    if (maxv != null)
                    {
                        max = (int)maxv.consecutivo + 1;
                    }
                    else
                    {
                        max = 1;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar consecutivo de ewo: " + e.ToString());
                max = -1;
            }

            return max;
        }
        
        public List<EwoTimesViewModel> GetEwoTime(int line, int year)
        {
            List<EwoTimesViewModel> list = new List<EwoTimesViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {

                    var query = (from e in context.ewos
                                 join ln in context.lineas
                                 on e.id_area_linea equals ln.id
                                 where e.id_area_linea == line
                                 && e.notificacion_averia.Value.Year == (year)
                                 group e by new { e.notificacion_averia.Value.Month } into g
                                 select new
                                 {
                                     g.Key.Month,
                                     EsperaTec = g.Sum(x => x.tiempo_espera_tecnico),
                                     TiempoDiag = g.Sum(x => x.tiempo_diagnostico),
                                     TiempoEspRep = g.Sum(x => x.tiempo_espera_repuestos),
                                     TiempoRep = g.Sum(x => x.tiempo_reparacion),
                                     TiempoPru = g.Sum(x => x.tiempo_pruebas)
                                 }).AsEnumerable()
                                .Select(g => new
                                {
                                    g.Month,
                                    g.EsperaTec,
                                    g.TiempoDiag,
                                    g.TiempoEspRep,
                                    g.TiempoRep,
                                    g.TiempoPru
                                }); 

                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            list.Add(new EwoTimesViewModel()
                            {
                                Mes = item.Month,
                                MesName = new DateTime(year, item.Month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant(),
                                EsperaTecnico = (int)item.EsperaTec,
                                TiempoDiagnostico = (int)item.TiempoDiag,
                                EsperaRepuestos = (int)item.TiempoEspRep,
                                TiempoReparacion = (int)item.TiempoRep,
                                TiempoPruebas = (int)item.TiempoPru
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar averias por líneas y mes: " + e.ToString());
            }

            return list;
        }

        public List<EwoTimesViewModelM> GetEwoCCR(int line, int year)
        {
            List<EwoTimesViewModelM> list = new List<EwoTimesViewModelM>();

            try
            {
                using (var context = new MttoAppEntities())
                {

                    var query = (from e in context.ewos
                                 join ln in context.lineas
                                 on e.id_area_linea equals ln.id
                                 where e.id_area_linea == line
                                 && e.notificacion_averia.Value.Year == (year)
                                 group e by new { e.notificacion_averia.Value.Month } into g
                                 select new
                                 {
                                     g.Key.Month,
                                     FactoresExt = g.Where(x => x.causa_raiz_index == 0).Count(),
                                     FaltaCono = g.Where(x => x.causa_raiz_index == 1).Count(),
                                     FaltaDis = g.Where(x => x.causa_raiz_index == 2).Count(),
                                     FaltaMtto = g.Where(x => x.causa_raiz_index == 3).Count(),
                                     CondSubEstOpe = g.Where(x => x.causa_raiz_index == 4).Count(),
                                     FaltaConBas = g.Where(x => x.causa_raiz_index == 5).Count()

                                 }).AsEnumerable()
                                .Select(g => new
                                {
                                    g.Month,
                                    g.FactoresExt,
                                    g.FaltaCono,
                                    g.FaltaDis,
                                    g.FaltaMtto,
                                    g.CondSubEstOpe,
                                    g.FaltaConBas
                                });

                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            list.Add(new EwoTimesViewModelM()
                            {
                                Mes = item.Month,
                                MesName = new DateTime(year, item.Month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")).ToUpperInvariant(),
                                FactoresExt = item.FactoresExt,
                                FaltaCono = item.FaltaCono,
                                FaltaDis = item.FaltaDis,
                                FaltaMtto = item.FaltaMtto,
                                CondSubEstOpe = item.CondSubEstOpe,
                                FaltaConBas = item.FaltaConBas
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar causa ciclo raiz por líneas y mes MTBF: " + e.ToString());
            }

            return list;
        }

        public async Task<KpiViewModel> GetEwoDesc(int id)
        {
            KpiViewModel lDecs = null;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join l in context.lineas
                                on e.id_area_linea equals l.id
                                join m in context.maquinas
                                on e.id_equipo equals m.Id
                                join t in context.AspNetUsers
                                on e.id_tecnico equals t.Id
                                join ta in context.tipos_data
                                on e.id_tipo_averia equals ta.Id
                                where e.Id == id
                                select new { e, m,l,t,ta };

                    var data = await query.ToListAsync();

                    foreach (var item in data.ToList())
                    {
                        lDecs= new KpiViewModel()
                        {
                            AreaLinea = item.l.nombre,
                            Equipo = item.m.nombre,
                            DiligenciadoPor = item.t.Nombres +" "+ item.t.Apellidos,
                            TipoAveria = item.ta.descripcion
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return lDecs;
        }

        public async Task<List<KpiViewModel>> GetEwoList()
        {
            List<KpiViewModel> list = new List<KpiViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos
                                join l in context.lineas
                                on e.id_area_linea equals l.id
                                join m in context.maquinas
                                on e.id_equipo equals m.Id
                                join t in context.AspNetUsers
                                on e.id_tecnico equals t.Id
                                join ta in context.tipos_data
                                on e.id_tipo_averia equals ta.Id                               
                                select new { e, m, l, t, ta };

                    var data = await query.ToListAsync();

                    foreach (var item in data.ToList())
                    {
                        list.Add(new KpiViewModel()
                        {
                            AreaLinea = item.l.nombre,
                            Equipo = item.m.nombre,
                            DiligenciadoPor = item.t.Nombres + " " + item.t.Apellidos,
                            TipoAveria = item.ta.descripcion,
                            IdTipoAveria = (int)item.e.id_tipo_averia,
                            TiempoTotal = (int)item.e.tiempo_total,
                            Fecha = (DateTime)item.e.fecha_ewo,
                            CicloRaiz = (int)item.e.causa_raiz_index,
                            DescCicloRaiz = DesCausaRaiz(item.e.causa_raiz_index)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al momento de consultar consolodado de Ewos: "+e.ToString());
            }

            return list;
        }

        private string DesCausaRaiz(int? index)
        {
            string r = "";

            switch (index)
            {
                case 0:
                    r = "Factores externos [FI]";
                    break;
                case 1:
                    r = "Falta de Conocimiento [PD]";
                    break;
                case 2:
                    r = "Falta de Diseño [FI]";
                    break;
                case 3:
                    r = "Falta de Mantenimiento [PM]";
                    break;
                case 4:
                    r = "Condiciones Sub estandar de operación [PD]";
                    break;
                case 5:
                    r = "Falta de Condiciones básicas [AA]";
                    break;
                default:
                    r = "None!";
                    break;
            }

            return r;
        }

        public async Task<int> AddEwo(ewos ewo)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.ewos.Add(ewo);
                    await context.SaveChangesAsync();
                    regs = ewo.Id;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                regs = -1;
            }
            return regs;
        }
    }
}