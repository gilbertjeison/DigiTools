using DigiTools.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DigiTools.Dao
{
    public class DaoRoles
    {
        public List<AspNetRoles> GetRoles()
        {
            List<AspNetRoles> roles = new List<AspNetRoles>();

            try
            {
                using (var context = new MttoAppEntities())
                {
                    var query = from u in context.AspNetRoles
                                where !u.Id.Equals("2b0bf2ba-c2a3-4f19-84d7-9288c0bc2895")
                                select u;

                    roles = query.ToList();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return roles;
        }
    }
}