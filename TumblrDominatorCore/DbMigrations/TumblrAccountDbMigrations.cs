using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;

namespace TumblrDominatorCore.DbMigrations
{
    public interface ITumblrdAccountDbMigrations : IDbMigration
    {
    }

    /// <summary>
    ///     Tumblr Account DBMigration
    /// </summary>
    public class TumblrAccountDbMigrations : BaseDbMigrations, ITumblrdAccountDbMigrations
    {
        public TumblrAccountDbMigrations()
        {
            AddMigrations(6, conn =>
            {
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<FeedInfo>();
                conn.CreateTable<OwnBlogs>();
                conn.CreateTable<Friendships>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedUser>();
                conn.CreateTable<UnFollowedUser>();
                conn.CreateTable<PrivateBlacklist>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<UnLikedPosts>();

                return "Initialization";
            });
        }
    }
}