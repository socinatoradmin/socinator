using System;
using System.Data.Entity;

namespace SQLite.CodeFirst
{
    public class CommonDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<CommonDbContext>
    {
        public CommonDbInitializer(DbModelBuilder modelBuilder,Action<CommonDbContext> SeedDataBase )
            : base(modelBuilder, typeof(CustomHistory))
        {
           // SeedDataBase
        }

        protected override void Seed(CommonDbContext context)
        {
        }
    }
}