using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Dal;
using GramDominatorCore.DbMigrations;

namespace GramDominatorCore.GDFactories
{
    public interface IGdCampaignDbConnection : ICampaignDatabaseConnection
    {

    }
    public class GdCampaignDbConnection : VersionedDbConnection, IGdCampaignDbConnection
    {
        public GdCampaignDbConnection(IGdCampaignDbMigrations dbMigration) : base(dbMigration)
        {
        }

        public SQLite.SQLiteConnection GetSqlConnection(string accountId)
        {
            var directoryName = ConstantVariable.GetIndexCampaignDir() + "\\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + $"\\{accountId}.db";
            var dbConnection = GetSqlConnectionAndRunMigration(connectionString);
            return dbConnection;
        }

        //public SQLite.SQLiteConnection GetSqlConnection(string accountId)
        //{
        //    var directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
        //    DirectoryUtilities.CreateDirectory(directoryName);
        //    var connectionString = directoryName + $"\\{accountId}.db";
        //    var dbConnection = new SQLite.SQLiteConnection(connectionString);
        //    dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
        //    dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
        //    dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.UnfollowedUsers>();
        //    dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.HashtagScrape>();
        //    return dbConnection;
        //}
        //public string ConnectionString { get; set; }

        //public DbContext GetContext(string accountId)
        //{
        //    var directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
        //    DirectoryUtilities.CreateDirectory(directoryName);
        //    ConnectionString = directoryName + $"\\{accountId}.db";
        //    var dbConnection = new SQLiteConnection(@"data source=" + ConnectionString);
        //    return new GdCampaignDbContext(dbConnection, false);
        //}
    }

    //public class GdCampaignDbContext : DbContext
    //{
    //    public GdCampaignDbContext(string nameOrConnectionString)
    //        : base(nameOrConnectionString)
    //    {
    //        Configure();
    //    }

    //    public GdCampaignDbContext(DbConnection connection, bool contextOwnsConnection)
    //        : base(connection, contextOwnsConnection)
    //    {
    //        Configure();
    //    }

    //    private void Configure()
    //    {
    //        Configuration.ProxyCreationEnabled = true;
    //        Configuration.LazyLoadingEnabled = true;
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        var fdCampaignModuleConfiguration = new GdCampaignModuleConfiguration();
    //        fdCampaignModuleConfiguration.Configuration(modelBuilder);
    //        var initializer = new GdCampaignDatabaseInitializer(modelBuilder);
    //        Database.SetInitializer(initializer);
    //    }
    //}

    //public class GdCampaignCustomHistory : IHistory
    //{
    //    public int Id { get; set; }
    //    public string Hash { get; set; }
    //    public string Context { get; set; }
    //    public DateTime CreateDate { get; set; }
    //}

    //public class GdCampaignDatabaseInitializer : SQliteMigration<GdCampaignDbContext>
    //{
    //    public GdCampaignDatabaseInitializer(DbModelBuilder modelBuilder)
    //        : base(modelBuilder)
    //    {
    //        // SeedDataBase
    //    }

    //    protected override void Seed(GdCampaignDbContext context)
    //    {

    //    }
    //    //public override void InitializeDatabase(GdCampaignDbContext context)
    //    //{
    //    //    string databseFilePath = GetDatabasePathFromContext(context);
    //    //    bool dbExists = File.Exists(databseFilePath);
    //    //    if (dbExists)
    //    //    {
    //    //        var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
    //    //        var tables = metadata.GetItemCollection(DataSpace.SSpace)
    //    //           .GetItems<EntityContainer>()
    //    //           .Single()
    //    //           .BaseEntitySets
    //    //           .OfType<EntitySet>()
    //    //           .Where(s => !s.MetadataProperties.Contains("Type")
    //    //                       || s.MetadataProperties["Type"].ToString() == "Tables");
    //    //        foreach (var table in tables)
    //    //        {
    //    //            var tableName = table.MetadataProperties.Contains("Table")
    //    //                            && table.MetadataProperties["Table"].Value != null
    //    //                ? table.MetadataProperties["Table"].Value.ToString()
    //    //                : table.Name;
    //    //            foreach (var member in table.ElementType.DeclaredMembers)
    //    //            {
    //    //                var cur = context.Database.SqlQuery<TableInfo>($"PRAGMA table_info ({tableName})").ToList();

    //    //                if (cur.All(a => a.name != member.Name))
    //    //                {
    //    //                    context.Database.ExecuteSqlCommand(
    //    //                        $"ALTER TABLE {tableName} ADD COLUMN {member.Name} {((TypeUsage)((ReadOnlyMetadataCollection<System.Data.Entity.Core.Metadata.Edm.MetadataProperty>)member.MetadataProperties["MetadataProperties"].Value)["TypeUsage"].Value).EdmType.Name} null;");
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //    base.InitializeDatabase(context);
    //    //}
    //    //private class TableInfo
    //    //{
    //    //    public long cid { get; set; }
    //    //    public string name { get; set; }
    //    //    public string type { get; set; }
    //    //    public long notnull { get; set; }
    //    //    public string dflt_value { get; set; }
    //    //    public long pk { get; set; }
    //    //}
    //}

    //public class GdCampaignModuleConfiguration : IModuleConfiguration
    //{
    //    public void Configuration(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.UnfollowedUsers>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.HashtagScrape>();
    //    }
    //}



}