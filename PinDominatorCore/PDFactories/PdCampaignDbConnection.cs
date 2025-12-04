using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.DbMigrations;
using SQLite;

namespace PinDominatorCore.PDFactories
{
    public interface IPdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class PdCampaignDbConnection : VersionedDbConnection, IPdCampaignDbConnection
    {
        public PdCampaignDbConnection(IPdCampaignDbMigrations dbMigration) : base(dbMigration)
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