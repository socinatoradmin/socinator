using DominatorHouseCore.Dal.DbMigrations;

namespace GramDominatorCore.DbMigrations
{
    public interface IGdCampaignDbMigrations : IDbMigration
    {

    }
    public class GdCampaignDbMigrations : BaseDbMigrations, IGdCampaignDbMigrations
    {
        public GdCampaignDbMigrations()
        {
            AddMigrations(5, conn =>
            {
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.UnfollowedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.HashtagScrape>();   
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.MakeCloseFriendCampaign>();   
                return "Initialize";
            });
        }
    }
}
