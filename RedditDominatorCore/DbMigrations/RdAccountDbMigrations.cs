using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;

namespace RedditDominatorCore.DbMigrations
{
    public interface IRdAccountDbMigrations : IDbMigration
    {
    }

    public class RdAccountDbMigrations : BaseDbMigrations, IRdAccountDbMigrations
    {
        public RdAccountDbMigrations()
        {
            AddMigrations(3, conn =>
            {
                conn.CreateTable<InteractedPost>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<PrivateBlacklist>();
                conn.CreateTable<InteractedSubreddit>();
                conn.CreateTable<OwnCommunities>();
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<InteractedAutoActivityPost>();
                return "Initialization";
            });
        }
    }
}