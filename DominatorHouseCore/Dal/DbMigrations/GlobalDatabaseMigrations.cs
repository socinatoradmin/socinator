#region

using DominatorHouseCore.DatabaseHandler.DHTables;

#endregion

namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseMigrations : IDbMigration
    {
    }

    public class GlobalDatabaseMigrations : BaseDbMigrations, IGlobalDatabaseMigrations
    {
        public GlobalDatabaseMigrations()
        {
            AddMigrations(7, conn =>
            {
                conn.CreateTable<AccountDetails>();
                conn.CreateTable<BlackWhiteListUser>();
                conn.CreateTable<LocationList>();
                conn.CreateTable<InstaAccountBackup>();
                return "Initialization";
            });
        }
    }
}