using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.DbMigration;

namespace FaceDominatorCore.FDFactories
{

    public interface IFdCampaignDbConnection : ICampaignDatabaseConnection
    {

    }

    public class FdCampaignDbConnection : VersionedDbConnection, IFdCampaignDbConnection
    {

        public FdCampaignDbConnection(IFdCampaignDbMigrations dbMigration) : base(dbMigration)
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

        //private readonly object _lock = new object();
        //public SQLiteConnection GetSqlConnection(string campaignId)
        //{
        //    lock (_lock)
        //    {
        //        var directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
        //        DirectoryUtilities.CreateDirectory(directoryName);
        //        var connectionString = directoryName + $"\\{campaignId}.db";
        //        var dbConnection = new SQLiteConnection(connectionString);

        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups>();
        //        dbConnection.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>();


        //        return dbConnection;
        //    }

        //}
    }


}
