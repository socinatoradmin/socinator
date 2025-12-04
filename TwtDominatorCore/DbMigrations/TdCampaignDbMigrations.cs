using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.TdTables.Campaign;

namespace TwtDominatorCore.DbMigration
{
    public interface ITdCampaignDbMigrations : IDbMigration
    {
    }

    public class TdCampaignDbMigrations : BaseDbMigrations, ITdCampaignDbMigrations
    {
        public TdCampaignDbMigrations()
        {
            AddMigrations(2, conn =>
            {
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<UnfollowedUsers>();
                return "Added 'Status'";
            });
        }
    }
}