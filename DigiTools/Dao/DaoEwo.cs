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
                //REPORTAR ERROR EN LA BASE DE DATOS
                DaoExcepcion.AddException(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name == null ? "No definido" : HttpContext.Current.User.Identity.Name,
                        descripcion = "Dao Ewo " + e.ToString(),
                        fecha = DateTime.Now
                    });
            }

            return max;
        }

        public int GetConsecutive(int id_ewo)
        {
            int max = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var maxv = context.ewos.Where(x=>x.Id == id_ewo)
                        .FirstOrDefault().consecutivo;
                    if (maxv != null)
                    {
                        max = maxv.Value;
                    }
                    else
                    {
                        max = 0;
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

        public string[] GetEwoImages(int id_ewo)
        {
            string[] images = new string[4];

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from e in context.ewos                                
                                where e.Id == id_ewo
                                select new { e.imagen_1, e.imagen_2,e.imagen_3,e.imagen_4 };
                 
                    images[0] = query.First().imagen_1;
                    images[1] = query.First().imagen_2;
                    images[2] = query.First().imagen_3;
                    images[3] = query.First().imagen_4;                    
                    
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar imagenes de ewo: " + e.ToString());
            }

            return images;
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

                    foreach (var i in data.ToList())
                    {
                        lDecs = new KpiViewModel()
                        {
                            Id = i.e.Id,
                            AreaLinea = i.l.nombre,
                            Equipo = i.m.nombre,
                            DiligenciadoPor = i.t.Nombres + " " + i.t.Apellidos,
                            TipoAveria = i.ta.descripcion,
                            Consecutivo = i.e.consecutivo.Value,
                            Fecha = i.e.fecha_ewo.Value,
                            NumAviso = i.e.aviso_numero,
                            Turno = i.e.id_turno.Value,
                            HrNotAveD = i.e.notificacion_averia.Value,
                            HrNotAve = i.e.notificacion_averia.Value.ToString("dd-MM-yyyy HH:mm"),
                            HrIniRepD = i.e.inicio_reparacion.Value,
                            HrIniRep = i.e.inicio_reparacion.Value.ToString("dd-MM-yyyy HH:mm"),
                            TEspIniTec = i.e.tiempo_espera_tecnico.Value,
                            TDiagn = i.e.tiempo_diagnostico.Value,
                            TEspRep = i.e.tiempo_espera_repuestos.Value,
                            TRepCamP = i.e.tiempo_reparacion.Value,
                            PruTieArr = i.e.tiempo_pruebas.Value,
                            HrFinRepEntD = i.e.fin_reparacion.Value,
                            HrFinRepEnt = i.e.fin_reparacion.Value.ToString("dd-MM-yyyy HH:mm"),
                            TiempoTotal = i.e.tiempo_total.Value,
                            PathImage1 = i.e.imagen_1,
                            PathImage2 = i.e.imagen_2,
                            PathImagePQ1 = i.e.imagen_3,
                            PathImagePQ2 = i.e.imagen_4,
                            DescImg1 = i.e.desc_imagen_1,
                            DescImg2 = i.e.desc_imagen_2,
                            DescImgPQ1 = i.e.desc_imagen_3,
                            DescImgPQ2 = i.e.desc_imagen_4,
                            DescripcionAveria = i.e.desc_averia,
                            Accion = i.e.ajuste == true ? 0:1,
                            GembaOkB = i.e.gemba_ok.Value,
                            GembutsuOkB = i.e.gembutsu_ok.Value,
                            GensokuOkB = i.e.gensoku_ok.Value,
                            GenriOkB = i.e.genri_ok.Value,
                            GenjitsuOkB = i.e.genjitsu_ok.Value,
                            GembaDesc = i.e.gemba,
                            GembutsuDesc = i.e.gembutsu,
                            GenjitsuDesc = i.e.genjitsu,
                            GenriDesc = i.e.genri,
                            GensokuDesc = i.e.gensoku,
                            QueDesc = i.e.what,
                            DondeDesc = i.e.where,
                            CuandoDesc = i.e.when,
                            QuienDesc = i.e.who,
                            CualDesc = i.e.wich,
                            ComoDesc = i.e.how,
                            FenomenoDesc = i.e.fenomeno,
                            FchUltimoMtto = i.e.fecha_ultimo_mtto.Value.ToString("dd-MM-yyyy"),
                            FchProxMtto = i.e.fecha_proximo_mtto.Value.ToString("dd-MM-yyyy"),
                            FchUltimoMttoD = i.e.fecha_ultimo_mtto.Value,
                            FchProxMttoD = i.e.fecha_proximo_mtto.Value,
                            IdTecMattInv = i.e.tecnicos_man_involucrados,
                            IdOpersInv = i.e.operarios_involucrados,
                            IdAnaElab = i.e.elaborador_analisis,
                            FchAnaElab = i.e.fecha_analisis.Value.ToString("dd-MM-yyyy"),
                            IdContMedDef = i.e.definidor_contramedidas,
                            FchDefConMed = i.e.fecha_contramedida.Value.ToString("dd-MM-yyyy"),
                            FchEjeVal = i.e.fecha_validacion.Value.ToString("dd-MM-yyyy"),
                            IdEjeValPor = i.e.validador_ejecucion,
                            CausaRaiz = i.e.falla_index.Value,
                            CicloRaiz = i.e.causa_raiz_index.Value,
                            IdPlanta = i.l.id_planta.Value,
                            IdTipoLinea = i.l.id_tipo_linea.Value,
                            IdLinea = i.e.id_area_linea.Value,
                            IdMaquina = i.e.id_equipo.Value,
                            IdTipoAveria = i.e.id_tipo_averia.Value
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al generar formato ewo (SOMEHELPERS): "+e.ToString());
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
                            Id = item.e.Id,
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

        public async Task<List<KpiViewModel>> GetEwoList(string id)
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
                                where e.id_tecnico == id
                                select new { e, m, l, t, ta };

                    var data = await query.ToListAsync();

                    foreach (var item in data.ToList())
                    {
                        list.Add(new KpiViewModel()
                        {
                            Id = item.e.Id,
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
                Debug.WriteLine("Excepción al momento de consultar consolodado de Ewos: " + e.ToString());
            }

            return list;
        }

        private string DesCausaRaiz(int? index)
        {
            string r = "";

            switch (index)
            {
                case 1:
                    r = "Factores externos [FI]";
                    break;
                case 2:
                    r = "Falta de Conocimiento [PD]";
                    break;
                case 3:
                    r = "Falta de Diseño [FI]";
                    break;
                case 4:
                    r = "Falta de Mantenimiento [PM]";
                    break;
                case 5:
                    r = "Condiciones Sub estandar de operación [PD]";
                    break;
                case 6:
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

        public async Task<int> EditEwo(ewos ewo)
        {
            ewos ed;

            int regs = 0;

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    ed = context.ewos.Where(s => s.Id == ewo.Id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)                
                if (ed != null)
                {
                    ed.id_area_linea = ewo.id_area_linea;
                    ed.id_equipo = ewo.id_equipo;
                    ed.fecha_ewo = ewo.fecha_ewo;
                    ed.aviso_numero = ewo.aviso_numero;
                    ed.id_tecnico = ewo.id_tecnico;
                    ed.id_tipo_averia = ewo.id_tipo_averia;
                    ed.id_turno = ewo.id_turno;
                    ed.notificacion_averia = ewo.notificacion_averia;
                    ed.inicio_reparacion = ewo.inicio_reparacion;
                    ed.tiempo_espera_tecnico = ewo.tiempo_espera_tecnico;
                    ed.tiempo_diagnostico = ewo.tiempo_diagnostico;
                    ed.tiempo_espera_repuestos = ewo.tiempo_espera_repuestos;
                    ed.tiempo_reparacion = ewo.tiempo_reparacion;
                    ed.tiempo_pruebas = ewo.tiempo_pruebas;
                    ed.fin_reparacion = ewo.fin_reparacion;
                    ed.tiempo_total = ewo.tiempo_total;
                    ed.imagen_1 = ewo.imagen_1;
                    ed.imagen_2 = ewo.imagen_2;
                    ed.imagen_3 = ewo.imagen_3;
                    ed.imagen_4 = ewo.imagen_4;
                    ed.desc_imagen_1 = ewo.desc_imagen_1;
                    ed.desc_imagen_2 = ewo.desc_imagen_2;
                    ed.desc_imagen_3 = ewo.desc_imagen_3;
                    ed.desc_imagen_4 = ewo.desc_imagen_4;
                    ed.desc_averia = ewo.desc_averia;
                    ed.cambio_componente = ewo.cambio_componente;
                    ed.ajuste = ewo.ajuste;
                    ed.what = ewo.what;
                    ed.where = ewo.where;
                    ed.when = ewo.when;
                    ed.who = ewo.who;
                    ed.wich = ewo.wich;
                    ed.how = ewo.how;
                    ed.fenomeno = ewo.fenomeno;
                    ed.gemba = ewo.gemba;
                    ed.gemba_ok = ewo.gemba_ok;
                    ed.gembutsu = ewo.gembutsu;
                    ed.gembutsu_ok = ewo.gembutsu_ok;
                    ed.genjitsu = ewo.genjitsu;
                    ed.genjitsu_ok = ewo.genjitsu_ok;
                    ed.genri = ewo.genri;
                    ed.genri_ok = ewo.genri_ok;
                    ed.gensoku = ewo.gensoku;
                    ed.gensoku_ok = ewo.gensoku_ok;
                    ed.fecha_ultimo_mtto = ewo.fecha_ultimo_mtto;
                    ed.fecha_proximo_mtto = ewo.fecha_proximo_mtto;
                    ed.falla_index = ewo.falla_index;
                    ed.causa_raiz_index = ewo.causa_raiz_index;
                    ed.tecnicos_man_involucrados = ewo.tecnicos_man_involucrados;
                    ed.operarios_involucrados = ewo.operarios_involucrados;
                    ed.elaborador_analisis = ewo.elaborador_analisis;
                    ed.fecha_analisis = ewo.fecha_analisis;
                    ed.definidor_contramedidas = ewo.definidor_contramedidas;
                    ed.fecha_contramedida = ewo.fecha_contramedida;
                    ed.validador_ejecucion = ewo.validador_ejecucion;
                    ed.fecha_validacion = ewo.fecha_validacion;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(ed).State = EntityState.Modified;

                    //4. call SaveChanges
                    await context.SaveChangesAsync();
                    regs = ed.Id;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al editar ewo: " + e.ToString());
            }

            return regs;
        }

        public async Task<int> DeleteEwo(int id)
        {
            ewos ewod;
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    ewod = context.ewos.Where(s => s.Id.Equals(id)).FirstOrDefault();
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as deleted
                    context.Entry(ewod).State = EntityState.Deleted;

                    //4. call SaveChanges
                    regs = context.SaveChangesAsync();

                    return await regs;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Excepción al eliminar ewo: " + e.ToString());
            }
            return 0;
        }

        public async Task<IndexAdminViewModel> GetIndexData()
        {
            IndexAdminViewModel iavm = new IndexAdminViewModel();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    await Task.Run(() =>
                    {
                        iavm.IncidentesReportados = (from e in context.ewos select e).Count();                        
                        iavm.TiempoLinParada = (from e in context.ewos
                                                select e).Sum(x => x.tiempo_total.Value);
                        iavm.UsuariosRegistrados = (from e in context.AspNetUsers
                                                    select e).Count();

                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar porcentajes de tipos de incidentes: " + e.ToString());
            }

            return iavm;
        }

        public List<DonutViewModel> GetEwoPercents()
        {
            List<DonutViewModel> list = new List<DonutViewModel>();

            try
            {
                using (var context = new MttoAppEntities())
                {

                    var query = (from e in context.ewos
                                 group e by new { e.causa_raiz_index } into g
                                 select new
                                 {
                                     g.Key,
                                     Count = g.Count()
                                 }).AsEnumerable()
                                .Select(g => new
                                {
                                    des = DesCausaRaiz(g.Key.causa_raiz_index),
                                    g.Count
                                });

                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            list.Add(new DonutViewModel()
                            {
                                label = item.des,
                                value = ((double)item.Count / GetCount() * 100).ToString("F2")
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar porcentajes de tipos de incidentes: " + e.ToString());
            }

            return list;
        }

        public int GetCount()
        {
            int max = 0;

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var maxv = context.ewos.Count();
                    
                    max = maxv;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error al consultar cantidad de ewos: " + e.ToString());
                max = -1;
            }

            return max;
        }
    }
}