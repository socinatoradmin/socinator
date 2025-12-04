using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.DbMigrations
{
    public interface IRdCampaignDbMigrations : IDbMigration
    {
    }

    public class RdCampaignDbMigrations : BaseDbMigrations, IRdCampaignDbMigrations
    {
        public RdCampaignDbMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<InteractedPost>();
                conn.CreateTable<InteractedSubreddit>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<InteractedAutoActivityPostCampaign>();
                return "Initialization";
            });
        }
    }
}