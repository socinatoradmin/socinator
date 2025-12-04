using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using TumblrDominatorCore.DbMigrations;

namespace TumblrDominatorCore.TumblrFactory
{
    public interface ITumblrCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class TumblrCampaignDbConnection : VersionedDbConnection, ITumblrCampaignDbConnection
    {
        public TumblrCampaignDbConnection(ITumblrCampaignDbMigrations dbMigration) : base(dbMigration)
        {
        }

        public SQLiteConnection GetSqlConnection(string accountId)
        {
            var directoryName = ConstantVariable.GetIndexCampaignDir() + "\\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + $"\\{accountId}.db";
            var dbConnection = GetSqlConnectionAndRunMigration(connectionString);
            return dbConnection;
        }
    }


    //public class TumblrCampaignDbConnection : IDatabaseConnection
    //{
    //    private readonly object _lock = new object();
    //    public SQLite.SQLiteConnection GetSqlConnection(string campaignId)
    //    {
    //        lock (_lock)
    //        {
    //            var directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
    //            DirectoryUtilities.CreateDirectory(directoryName);
    //            var connectionString = directoryName + $"\\{campaignId}.db";
    //            var dbConnection = new SQLiteConnection(connectionString);

    //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedPosts>();
    //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser>();
    //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.UnFollowedUser>();
    //            return dbConnection;
    //        }

    //    }
    //}

    //public class TumblrCampaignDbContext : DbContext
    //{
    //    public TumblrCampaignDbContext(string nameOrConnectionString)
    //        : base(nameOrConnectionString)
    //    {
    //        Configure();
    //    }

    //    public TumblrCampaignDbContext(DbConnection connection, bool contextOwnsConnection)
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
    //        var fdCampaignModuleConfiguration = new TumblrCampaignModuleConfiguration();
    //        fdCampaignModuleConfiguration.Configuration(modelBuilder);
    //        var initializer = new TumblrCampaignDatabaseInitializer(modelBuilder);
    //        Database.SetInitializer(initializer);
    //    }
    //}


    //public class TumblrCampaignDatabaseInitializer : SQliteMigration<TumblrCampaignDbContext>
    //{
    //    public TumblrCampaignDatabaseInitializer(DbModelBuilder modelBuilder)
    //        : base(modelBuilder)
    //    {
    //        // SeedDataBase
    //    }

    //    protected override void Seed(TumblrCampaignDbContext context)
    //    {

    //    }
    //}

    //public class TumblrCampaignModuleConfiguration : IModuleConfiguration
    //{
    //    public void Configuration(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedPosts>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.UnFollowedUser>();
    //    }
    //}
}