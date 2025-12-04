using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;

namespace QuoraDominatorCore.DbMigrations
{
    public interface IQdCampaignDbMigrations : IDbMigration
    {
    }

    public class QdCampaignDbMigrations : BaseDbMigrations, IQdCampaignDbMigrations
    {
        public QdCampaignDbMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedMessage>();
                conn.CreateTable<InteractedQuestion>();
                conn.CreateTable<InteractedAnswers>();
                conn.CreateTable<UnfollowedUsers>();
                return "Initialization";
            });
        }
    }
}