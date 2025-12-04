using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using TwtDominatorCore.DbMigrations;

namespace TwtDominatorCore.Factories
{
    public interface ITdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class TdAccountDbConnection : VersionedDbConnection, ITdAccountDbConnection
    {
        public TdAccountDbConnection(ITdAccountDbMigrations dbMigration) : base(dbMigration)
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