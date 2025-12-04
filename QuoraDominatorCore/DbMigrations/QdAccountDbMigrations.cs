using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;

namespace QuoraDominatorCore.DbMigrations
{
    public interface IQdAccountDbMigrations : IDbMigration
    {
    }

    public class QdAccountDbMigrations : BaseDbMigrations, IQdAccountDbMigrations
    {
        public QdAccountDbMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<FeedInfoes>();
                conn.CreateTable<Friendships>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<InteractedAnswers>();
                conn.CreateTable<InteractedMessage>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<InteractedQuestion>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<PrivateBlacklist>();

                return "Initialization";
            });
        }
    }
}