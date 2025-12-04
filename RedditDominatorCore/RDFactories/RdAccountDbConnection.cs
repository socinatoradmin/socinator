using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using RedditDominatorCore.DbMigrations;
using SQLite;

namespace RedditDominatorCore.RDFactories
{
    public interface IRdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class RdAccountDbConnection : VersionedDbConnection, IRdAccountDbConnection
    {
        public RdAccountDbConnection(IRdAccountDbMigrations dbMigration) : base(dbMigration)
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