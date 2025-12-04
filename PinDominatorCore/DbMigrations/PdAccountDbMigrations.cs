using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;

namespace PinDominatorCore.DbMigrations
{
    public interface IPdAccountDbMigrations : IDbMigration
    {
    }
    public class PdAccountDbMigrations : BaseDbMigrations, IPdAccountDbMigrations
    {
        public PdAccountDbMigrations()
        {
            AddMigrations(22, conn =>
            {
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<FeedInfoes>();
                conn.CreateTable<Friendships>();
                conn.CreateTable<InteractedUsers>();
                conn.CreateTable<InteractedBoards>();
                conn.CreateTable<OwnBoards>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<ScrapPins>();
                conn.CreateTable<ScrapBoards>();
                conn.CreateTable<UnfollowedUsers>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<PrivateBlacklist>();
                conn.CreateTable<CreateAccount>();

                return "Added New table CreateAccount";
            });
        }
    }
}