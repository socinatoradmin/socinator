using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Dal;
using GramDominatorCore.DbMigrations;

namespace GramDominatorCore.GDFactories
{
    public interface IGdAccountDbConnection : IAccountDatabaseConnection
    {

    }
    public class GdAccountDbConnection : VersionedDbConnection, IGdAccountDbConnection
    {
        public GdAccountDbConnection(IGdAccountDbMigrations dbMigration) : base(dbMigration)
        {
        }

        public SQLite.SQLiteConnection GetSqlConnection(string accountId)
        {
            var directoryName = ConstantVariable.GetIndexAccountDir() + "\\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + $"\\{accountId}.db";
            var dbConnection = GetSqlConnectionAndRunMigration(connectionString);
            return dbConnection;
        }
        //public SQLite.SQLiteConnection GetSqlConnection(string accountId)

        //{
        //    lock (DatabaseLock)
        //    {
        //        var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
        //        DirectoryUtilities.CreateDirectory(directoryName);
        //        var connectionString = directoryName + $"\\{accountId}.db";
        //        var dbConnection = new SQLite.SQLiteConnection(connectionString);
        //        try
        //        {
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.FeedInfoes>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.Friendships>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.DailyStatitics>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateBlacklist>();
        //            dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateWhitelist>();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }

        //        return dbConnection;
        //    }

        //}


        //public string ConnectionString { get; set; }

        //public DbContext GetContext(string accountId)
        //{
        //    try
        //    {
        //        var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
        //        DirectoryUtilities.CreateDirectory(directoryName);
        //        ConnectionString = directoryName + $"\\{accountId}.db";
        //        var dbConnection = new SQLiteConnection(@"data source=" + ConnectionString);
        //        var context = new GdAccountDbContext(dbConnection, false);
        //        return context;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    return null;
        //}
    }

    //public class GdAccountDbContext : DbContext
    //{

    //    public GdAccountDbContext(string nameOrConnectionString)
    //        : base(nameOrConnectionString)
    //    {
    //        Configure();
    //    }

    //    public GdAccountDbContext(DbConnection connection, bool contextOwnsConnection)
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
    //        var tdModuleConfiguration = new GdModuleConfiguration();
    //        tdModuleConfiguration.Configuration(modelBuilder);
    //        var initializer = new GdDatabaseInitializer(modelBuilder);
    //        Database.SetInitializer(initializer);
    //    }
    //}

    //public class GdAccountCustomHistory : IHistory
    //{
    //    public int Id { get; set; }
    //    public string Hash { get; set; }
    //    public string Context { get; set; }
    //    public DateTime CreateDate { get; set; }
    //}

    //public class GdDatabaseInitializer : SQliteMigration<GdAccountDbContext>
    //{
    //    public GdDatabaseInitializer(DbModelBuilder modelBuilder)
    //        : base(modelBuilder)
    //    {
    //        // SeedDataBase
    //    }

    //    protected override void Seed(GdAccountDbContext context)
    //    {

    //    }
    //}

    //public class GdModuleConfiguration : IModuleConfiguration
    //{
    //    public void Configuration(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.FeedInfoes>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.Friendships>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.DailyStatitics>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateBlacklist>();
    //        modelBuilder.Entity<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateWhitelist>();
    //    }
    //}

}