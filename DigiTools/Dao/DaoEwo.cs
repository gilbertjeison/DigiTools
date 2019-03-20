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

        public float GetMttrByLineMonth(int line, int month, int year)
        {
            float mttr = 0;

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

                    if (query!= null)
                    {
                        //int sum = 0;
                        //foreach (var item in query.ToList())
                        //{
                        //    sum += int.Parse(item.e.tiempo_total.ToString());
                        //}

                        var minT = query.ToList().Sum(x=>x.e.tiempo_total);
                        var avrs = query.ToList().Count;
                        if (avrs>0)
                        {
                            mttr = (float)minT / avrs;
                        }
                        
                    }
                    
                }                               
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar averias por líneas y mes: " + e.ToString());                
            }

            return mttr;
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