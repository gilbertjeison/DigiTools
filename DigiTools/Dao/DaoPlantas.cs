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
    public class DaoPlantas
    {
        DaoLineas daoLin = new DaoLineas();
        public async Task<List<plantas>> GetPlants()
        {
            List<plantas> list = new List<plantas>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.plantas
                              select b;
                    
                    list = await als.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al consultar plantas: " + e);
            }

            return list;
        }


        public async Task<List<PlantasViewModel>> GetCustomPlantsAsync()
        {
            List<PlantasViewModel> list = new List<PlantasViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.plantas
                              select b;

                    var l = await als.ToListAsync();

                    foreach (var item in l)
                    {
                        list.Add(new PlantasViewModel()
                        {
                            Id = item.Id,
                            Nombre = item.nombre,
                            Image = "~/Content/images/slides/"+item.image_path,
                            ListLineas = await daoLin.GetLinesAsync(item.Id)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al consultar custom plantas: " + e);
            }

            return list;
        }
    }
}