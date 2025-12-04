using DominatorHouseCore.Dal.DbMigrations;

namespace FaceDominatorCore.DbMigration
{
    public interface IFdCampaignDbMigrations : IDbMigration
    {

    }

    public class FdCampaignDbMigrations : BaseDbMigrations, IFdCampaignDbMigrations
    {
        public FdCampaignDbMigrations()
        {
            AddMigrations(14, conn =>
            {
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedMarketPlaces>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedEvents>();
                conn.CreateTable<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedCommentReplies>();
                return "Added new table for CommentReplies";
            });
        }
    }
}
