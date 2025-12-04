using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.DbMigrations;
using SQLite;

namespace QuoraDominatorCore.Factories
{
    public interface IQdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class QdAccountDbConnection : VersionedDbConnection, IQdAccountDbConnection
    {
        public QdAccountDbConnection(IQdAccountDbMigrations dbMigration) : base(dbMigration)
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