using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;

namespace PinDominatorCore.DbMigrations
{
    public interface IPdCampaignDbMigrations : IDbMigration
    {
    }

    public class PdCampaignDbMigrations : BaseDbMigrations, IPdCampaignDbMigrations
    {
        public PdCampaignDbMigrations()
        {
            AddMigrations(11, conn =>
            {
                conn.CreateTable<InteractedBoards>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<CreateAccount>();
                return "Added new table CreateAccount";
            });
        }
    }
}