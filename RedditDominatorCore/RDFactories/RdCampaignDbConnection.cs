using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using RedditDominatorCore.DbMigrations;
using SQLite;

namespace RedditDominatorCore.RDFactories
{
    public interface IRdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class RdCampaignDbConnection : VersionedDbConnection, IRdCampaignDbConnection
    {
        public RdCampaignDbConnection(IRdCampaignDbMigrations dbMigration) : base(dbMigration)
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
}