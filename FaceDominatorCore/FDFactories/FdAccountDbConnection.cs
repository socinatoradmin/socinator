using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.DbMigration;

namespace FaceDominatorCore.FDFactories
{

    public interface IFdAccountDbConnection : IAccountDatabaseConnection
    {

    }

    public class FdAccountDbConnection : VersionedDbConnection, IFdAccountDbConnection
    {
        public FdAccountDbConnection(IFdAccountDbMigrations dbMigration) : base(dbMigration)
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

        //private readonly object _lock = new object();
        //public SQLiteConnection GetSqlConnection(string accountId)
        //{
        //    lock (_lock)
        //    {
        //        var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
        //        DirectoryUtilities.CreateDirectory(directoryName);
        //        var connectionString = directoryName + $"\\{accountId}.db";
        //        var dbConnection = new SQLiteConnection(connectionString);

        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.FeedInfo>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.DailyStatitics>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.Friends>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.OwnGroups>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.OwnPages>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.LikedPages>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedGroups>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.PrivateBlacklist>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.PrivateWhitelist>();

        //        return dbConnection;
        //    }
        //}


    }

}