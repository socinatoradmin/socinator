using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;

namespace LinkedDominatorCore.DbMigrations
{
    public interface ILdCampaignDbMigrations : IDbMigration
    {
    }

    public class LdCampaignDbMigrations : BaseDbMigrations, ILdCampaignDbMigrations
    {
        public LdCampaignDbMigrations()
        {
            AddMigrations(7, dbConnection =>
            {
                dbConnection.CreateTable<InteractedCompanies>();
                dbConnection.CreateTable<InteractedGroups>();
                dbConnection.CreateTable<InteractedJobs>();
                dbConnection.CreateTable<InteractedPosts>();
                dbConnection.CreateTable<InteractedUsers>();
                dbConnection.CreateTable<InteractedPage>();
                return "Added New Table 'InteractedPage'";
            });
        }
    }
}