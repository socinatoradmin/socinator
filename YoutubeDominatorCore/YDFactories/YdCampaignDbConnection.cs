using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using YoutubeDominatorCore.DbMigrations;

namespace YoutubeDominatorCore.YDFactories
{
    public interface IYdCampaignDbConnection : ICampaignDatabaseConnection
    {
    }

    public class YdCampaignDbConnection : VersionedDbConnection, IYdCampaignDbConnection
    {
        public YdCampaignDbConnection(IYdCampaignDbMigrations dbMigration) : base(dbMigration)
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