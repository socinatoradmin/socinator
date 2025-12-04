using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using TumblrDominatorCore.DbMigrations;

namespace TumblrDominatorCore.TumblrFactory
{
    public interface ITumblrAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class TumblrAccountDbConnection : VersionedDbConnection, ITumblrAccountDbConnection
    {
        public TumblrAccountDbConnection(ITumblrdAccountDbMigrations dbMigration) : base(dbMigration)
        {
        }

        /// <summary>
        ///     To Get the Account database operation
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
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