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
    public class DaoUsuarios
    {
        public AspNetUsers GetUser(string id)
        {
            AspNetUsers user = new AspNetUsers();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = (from u in context.AspNetUsers
                                 where u.Id.Equals(id)
                                 select u).FirstOrDefault();

                    user = query;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return user;
        }

        public async Task<List<UserToApprove>> GetUserWOApprv()
        {
            List<UserToApprove> list = new List<UserToApprove>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    // Query for all
                    var als = from b in context.AspNetUsers
                              join r in context.AspNetRoles
                              on b.IdRol equals r.Id
                              where b.EmailConfirmed == false
                              select new { b, r };

                    var lista = als.ToList();

                    await Task.Run(() => Parallel.ForEach(lista, s =>
                    {
                        list.Add(new UserToApprove()
                        {
                            Id = s.b.Id,
                            Nombres = s.b.Nombres,
                            Apellidos = s.b.Apellidos,
                            Email = s.b.Email,
                            Usuario = s.b.UserName,
                            Registro = (DateTime)s.b.Registrado,
                            IdRol = s.r.Id,
                            DesRol = s.r.Name
                        });

                    }));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error al seleccionar usuarios por aprobar " + e.ToString());
            }

            return list;
        }

        public async Task<int> ApproveUser(string id, string role)
        {
            AspNetUsers usere;
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);

            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    usere = context.AspNetUsers.Where(s => s.Id == id).FirstOrDefault();
                }

                //2. change data in disconnected mode (out of ctx scope)
                if (usere != null)
                {
                    usere.EmailConfirmed = true;
                    usere.IdRol = role;
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as modified
                    context.Entry(usere).State = EntityState.Modified;

                    //4. call SaveChanges
                    regs = context.SaveChangesAsync();

                    return await regs;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al aprobar usuario: " + e.ToString());
            }

            return 0;
        }

        public async Task<int> DeleteUser(string id)
        {
            AspNetUsers userd;
            Task<int> regs = Task<int>.Factory.StartNew(() => 0);
            
            try
            {
                //1. Get row from DB
                using (var context = new MttoAppEntities())
                {
                    userd = context.AspNetUsers.Where(s => s.Id.Equals(id)).FirstOrDefault();
                }

                //save modified entity using new Context
                using (var context = new MttoAppEntities())
                {
                    //3. Mark entity as deleted
                    context.Entry(userd).State = EntityState.Deleted;

                    //4. call SaveChanges
                    regs = context.SaveChangesAsync();

                    return await regs;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Excepción al eliminar usuario: " + e.ToString());
            }
            return 0;
        }

        public AspNetUsers GetUserByMail(string email)
        {
            AspNetUsers user = new AspNetUsers();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = (from u in context.AspNetUsers
                                 where u.Email.Equals(email)
                                 select u).FirstOrDefault();

                    user = query;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return user;
        }
    }
}