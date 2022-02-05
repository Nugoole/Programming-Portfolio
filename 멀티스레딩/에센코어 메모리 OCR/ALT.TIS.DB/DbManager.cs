using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.TIS.DB
{
    public static class DbManager
    {
        static DbManager()
        {

        }

        public static bool Insert<T>(DbContext context, T _object) where T : class
        {
            context.Set<T>().Add(_object);

            return CheckChanges(context);
        }


        public static bool Delete<T>(DbContext context, T _object) where T : class
        {
            context.Set<T>().Remove(_object);

            return CheckChanges(context);
        }

        public static bool Update<T>(DbContext context, T _object) where T : class
        {
            context.Entry(_object).State = EntityState.Modified;

            return CheckChanges(context);
        }

        public static List<T> GetAll<T>(DbContext context) where T : class
        {
            return context.Set<T>().ToList();
        }

        private static bool CheckChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex != null)
                    return false;
            }

            return true;
        }

        public static int GetID<T>(DbContext context, T _Object) where T : class
        {
            return (int)context.Entry(_Object).Property("Id").CurrentValue;
        }
    }
}
