using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;

namespace TwtDominatorCore.DbMigrations
{
    public interface ITdAccountDbMigrations : IDbMigration
    {
    }

    public class TdAccountDbMigrations : BaseDbMigrations, ITdAccountDbMigrations
    {
        public TdAccountDbMigrations()
        {
            AddMigrations(2, conn =>
            {
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<FeedInfoes>();
                conn.CreateTable<Friendships>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<PrivateBlacklist>();
                return "Added 'Status'";
            });
        }
    }
}