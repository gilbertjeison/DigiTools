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
        public int GetLastConsecutive()
        {
            int max = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    max = (int)context.ewos.OrderByDescending(u => u.Id).FirstOrDefault().consecutivo + 1;
                }
            }
            catch (Exception e)
            {
                max = -1;
            }

            return max;
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