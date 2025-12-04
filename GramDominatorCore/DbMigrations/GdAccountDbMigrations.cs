using DominatorHouseCore.Dal.DbMigrations;

namespace GramDominatorCore.DbMigrations
{
    public interface IGdAccountDbMigrations : IDbMigration
    {

    }
    public class GdAccountDbMigrations : BaseDbMigrations, IGdAccountDbMigrations
    {
        public GdAccountDbMigrations()
        {
            AddMigrations(11, conn =>
            {
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.FeedInfoes>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.Friendships>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.DailyStatitics>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateBlacklist>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.PrivateWhitelist>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UserConversation>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.MakeCloseFriendAccount>();
                return "Added UserConversation Table";
            });
        }
    }
}
