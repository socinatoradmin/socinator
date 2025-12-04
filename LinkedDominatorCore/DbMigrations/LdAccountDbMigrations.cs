using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;

namespace LinkedDominatorCore.DbMigrations
{
    public interface ILdAccountDbMigrations : IDbMigration
    {
    }

    public class LdAccountDbMigrations : BaseDbMigrations, ILdAccountDbMigrations
    {
        public LdAccountDbMigrations()
        {
            AddMigrations(14, dbConnection =>
            {
                dbConnection.CreateTable<Connections>();
                dbConnection.CreateTable<RemovedConnections>();
                dbConnection.CreateTable<Groups>();
                dbConnection.CreateTable<UnjoinedGroups>();
                dbConnection.CreateTable<DailyStatitics>();
                dbConnection.CreateTable<FeedInfo>();
                dbConnection.CreateTable<InteractedCompanies>();
                dbConnection.CreateTable<InteractedGroups>();
                dbConnection.CreateTable<InteractedJobs>();
                dbConnection.CreateTable<InteractedPosts>();
                dbConnection.CreateTable<InteractedUsers>();
                dbConnection.CreateTable<InvitationsSent>();
                dbConnection.CreateTable<PrivateBlacklist>();
                dbConnection.CreateTable<PrivateWhitelist>();
                dbConnection.CreateTable<InteractedPage>();
                dbConnection
                    .CreateTable<SkipInteractedAttachments>();
                dbConnection.CreateTable<Pages>();

                return "Added New Table 'Pages' 'Add column in connections' 'Add column in InteractedUsers'";
            });
        }
    }
}