//using BaseLib;

using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataBaseConnectionCodeFirst
{
    /// <summary>
    /// Database connection with sqlite - code first approach
    /// Assign ConnectionString and DbModelBuilder (database schema) in the constroctor to create the database if doesn't exist at the first connection
    /// </summary>
    public class DataBaseConnection
    {
        private string ConnectionString { get; set; } = string.Empty;

        private Action<DbModelBuilder> ConfigureDbModelBuilder { get; set; }

        public DataBaseConnection(string connectionString, Action<DbModelBuilder> ConfigureDbModelBuilder = null)
        {
            this.ConnectionString = connectionString;
            this.ConfigureDbModelBuilder = ConfigureDbModelBuilder;
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        return expression == null ? context.Set<T>().Count() : context.Set<T>().Where(expression).Count();                       
                    }
                }               
            }
            catch (Exception Ex)
            {
                return 0;
            }
        }


        public bool Add<T>(T data) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        context.Set<T>().Add(data);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public List<T> Get<T>(Expression<Func<T, bool>> Expression = null) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        return Expression == null ? context.Set<T>().ToList() : context.Set<T>().Where(Expression).ToList();
                    }
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }


        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> Expression = null) where T : class
        {
            List<T> lstData = new List<T>();
            try
            {
                lstData = await Task.Factory.StartNew(() =>
               {
                   using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                   {
                       sqLiteConnection.Open();
                       using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                       {
                           return Expression == null ? context.Set<T>().ToList() : context.Set<T>().Where(Expression).ToList();
                       }
                   }
               });

            }
            catch (Exception Ex)
            {
                return lstData;
            }
            return lstData;
        }


        public T GetSingle<T>(Expression<Func<T, bool>> Expression) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        return context.Set<T>().FirstOrDefault(Expression);
                    }
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        public bool Remove<T>(T t) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        context.Entry<T>(t).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();
                    }
                    sqLiteConnection.Close();
                }
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public bool Update<T>(T t) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CommonDbContext(sqLiteConnection, false, this.ConfigureDbModelBuilder))
                    {
                        context.Entry<T>(t).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    sqLiteConnection.Close();
                }
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

    }
}
