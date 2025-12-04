using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using YoutubeDominatorCore.DbMigrations;

namespace YoutubeDominatorCore.YDFactories
{
    public interface IYdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class YdAccountDbConnection : VersionedDbConnection, IYdAccountDbConnection
    {
        public YdAccountDbConnection(IYdAccountDbMigrations dbMigration) : base(dbMigration)
        {
        }

        public SQLiteConnection GetSqlConnection(string accountId)
        {
            var directoryName = ConstantVariable.GetIndexAccountDir() + "\\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + $"\\{accountId}.db";
            var dbConnection = GetSqlConnectionAndRunMigration(connectionString);
            return dbConnection;
        }
    }
}