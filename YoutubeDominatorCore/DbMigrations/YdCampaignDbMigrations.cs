using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.YdTables.Campaign;

namespace YoutubeDominatorCore.DbMigrations
{
    public interface IYdCampaignDbMigrations : IDbMigration
    {
    }

    public class YdCampaignDbMigrations : BaseDbMigrations, IYdCampaignDbMigrations
    {
        /// <summary>
        /// Campaign level DB migration
        /// Whenever any changes (added columns, tables or modified some attributes etc) then this campaign db need to be migrated with the new changes.
        /// So, we need to change the migration version number and return a string value('what you have changed') when it happens(mentioned above)
        /// </summary>
        public YdCampaignDbMigrations()
        {
            // Here, first parameter value is migration version number
            AddMigrations(10, conn =>
            {
                conn.CreateTable<InteractedChannels>();
                conn.CreateTable<InteractedPosts>();

                // this is the information what you have changed in campaign DB 
                return "Added two columns for report module in InteractedPosts";
            });
        }
    }
}