using DominatorHouseCore.Dal;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.DbMigrations;
using SQLite;

namespace PinDominatorCore.PDFactories
{
    public interface IPdAccountDbConnection : IAccountDatabaseConnection
    {
    }

    public class PdAccountDbConnection : VersionedDbConnection, IPdAccountDbConnection
    {
        public PdAccountDbConnection(IPdAccountDbMigrations dbMigration) : base(dbMigration)
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