using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign;

namespace TumblrDominatorCore.DbMigrations
{
    public interface ITumblrCampaignDbMigrations : IDbMigration
    {
    }

    public class TumblrCampaignDbMigrations : BaseDbMigrations, ITumblrCampaignDbMigrations
    {
        /// <summary>
        ///     Tumblr Campaign DBMigration
        /// </summary>
        public TumblrCampaignDbMigrations()
        {
            AddMigrations(5, conn =>
            {
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedUser>();
                conn.CreateTable<UnFollowedUser>();
                conn.CreateTable<UnLikedPosts>();

                return "Initialization";
            });
        }
    }
}