using DominatorHouseCore.Dal.DbMigrations;

namespace FaceDominatorCore.DbMigration
{
    public interface IFdAccountDbMigrations : IDbMigration
    {

    }

    public class FdAccountDbMigrations : BaseDbMigrations, IFdAccountDbMigrations
    {
        public FdAccountDbMigrations()
        {
            AddMigrations(16, conn =>
            {
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.FeedInfo>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.DailyStatitics>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.Friends>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.OwnGroups>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.OwnPages>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.LikedPages>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.MarketplaceDetails>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedGroups>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedMarketPlaces>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.PrivateBlacklist>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.PrivateWhitelist>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedEvents>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedCommentReplies>();
                return "Added new table for CommentReplies";
            });
        }
    }
}
