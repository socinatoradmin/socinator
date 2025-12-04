using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;

namespace YoutubeDominatorCore.DbMigrations
{
    public interface IYdAccountDbMigrations : IDbMigration
    {
    }

    public class YdAccountDbMigrations : BaseDbMigrations, IYdAccountDbMigrations
    {
        /// <summary>
        /// Account level DB migration
        /// Whenever any changes (added columns, tables or modified some attributes etc) then this account db need to be migrated with the new changes.
        /// So, we need to change the migration version number and return a string value('what you have changed') when it happens(mentioned above)
        /// </summary>
        public YdAccountDbMigrations()
        {
            // Here, first parameter value is migration version number
            AddMigrations(10, conn =>
            {
                conn.CreateTable<DailyStatitics>();
                conn.CreateTable<FeedInfos>();
                conn.CreateTable<Friendships>();
                conn.CreateTable<InteractedChannels>();
                conn.CreateTable<OwnChannels>();
                conn.CreateTable<InteractedPosts>();
                conn.CreateTable<PrivateWhitelist>();
                conn.CreateTable<PrivateBlacklist>();

                // this is the information what you have changed in account DB 
                return "Added two columns for report module in InteractedPosts";
            });
        }
    }
}