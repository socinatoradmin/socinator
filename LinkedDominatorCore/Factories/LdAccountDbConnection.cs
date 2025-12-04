using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DbMigrations;
using SQLite;

namespace LinkedDominatorCore.Factories
{
    public interface ILdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class LdAccountDbConnection : VersionedDbConnection, ILdAccountDbConnection
    {
        public LdAccountDbConnection(ILdAccountDbMigrations dbMigration) : base(dbMigration)
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