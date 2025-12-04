using System;
using System.Data.Common;
using System.Data.Entity;

namespace SQLite.CodeFirst
{
    public class CommonDbContext : DbContext
    {

        Action<DbModelBuilder> ConfigureDbModelBuilder;

        Action<CommonDbContext> SeedDataBase;

        public CommonDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        public CommonDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        public CommonDbContext(DbConnection connection, bool contextOwnsConnection, Action<DbModelBuilder> ConfigureDbModelBuilder = null , Action<CommonDbContext> SeedDataBase = null)
          : base(connection, contextOwnsConnection)
        {
            Configure();
            this.ConfigureDbModelBuilder = ConfigureDbModelBuilder;
            this.SeedDataBase = SeedDataBase;
        }

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (this.ConfigureDbModelBuilder != null)
            {
                this.ConfigureDbModelBuilder(modelBuilder);
                var initializer = new CommonDbInitializer(modelBuilder, this.SeedDataBase);
                Database.SetInitializer(initializer);
            }
        }
    }
}