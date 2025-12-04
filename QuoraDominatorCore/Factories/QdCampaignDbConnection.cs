using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.DbMigrations;
using SQLite;

namespace QuoraDominatorCore.Factories
{
    public interface IQdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class QdCampaignDbConnection : VersionedDbConnection, IQdCampaignDbConnection
    {
        public QdCampaignDbConnection(IQdCampaignDbMigrations dbMigration) : base(dbMigration)
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