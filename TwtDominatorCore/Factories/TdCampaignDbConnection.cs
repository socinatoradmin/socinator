using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using TwtDominatorCore.DbMigration;

namespace TwtDominatorCore.Factories
{
    public interface ITdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class TdCampaignDbConnection : VersionedDbConnection, ITdCampaignDbConnection
    {
        public TdCampaignDbConnection(ITdCampaignDbMigrations dbMigration) : base(dbMigration)
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