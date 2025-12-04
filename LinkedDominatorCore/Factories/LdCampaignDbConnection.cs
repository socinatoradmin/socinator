using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DbMigrations;
using SQLite;

namespace LinkedDominatorCore.Factories
{
    public interface ILdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class LdCampaignDbConnection : VersionedDbConnection, ILdCampaignDbConnection
    {
        public LdCampaignDbConnection(ILdCampaignDbMigrations dbMigration) : base(dbMigration)
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