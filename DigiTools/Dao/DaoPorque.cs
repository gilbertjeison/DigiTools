using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoPorque
    {
        public async Task<int> AddPorque(List<porques> pqs)
        {
            int regs = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    context.porques.AddRange(pqs);
                    regs = await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error agregando porques: " + e.ToString());
                regs = -1;
            }

            return regs;
        }

        public async Task<List<porques>> GetPorques(int id_ewo)
        {
            List<porques> list = new List<porques>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from ru in context.porques
                                where ru.id_ewo == id_ewo
                                select ru;

                    list = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al momento de consultar porques: " + e.ToString());
            }

            return list;
        }

        public async Task<int> EditPorque(porques pq)
        {
            porques pqd;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    pqd = context.porques.Where(s => s.Id == pq.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (pqd != null)
                {
                    pqd.porque_1 = pq.porque_1;
                    pqd.porque_2 = pq.porque_2;
                    pqd.porque_3 = pq.porque_3;
                    pqd.porque_4 = pq.porque_4;
                    pqd.porque_5 = pq.porque_5;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(pqd).State = EntityState.Modified;

                    //4. call SaveChanges
                    regs = await context.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al editar porques: " + e.ToString());
            }

            return regs;
        }

        public async Task<int> DeletePorqueFromEwo(int id_ewo)
        {
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    context.porques.RemoveRange
                        (context.porques.Where(x => x.id_ewo == id_ewo));

                    regs = context.SaveChangesAsync();
                    return await regs;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Excepción al eliminar porques: " + e.ToString());
            }
            return 0;
        }
    }
}