using Unity;
using Unity.Extension;

namespace TumblrDominatorCore.DbMigrations
{
    public class TumblrDbMigrationUnityExtension : UnityContainerExtension
    {
        /// <summary>
        ///     Initializing DBmigration functinality using Unity Container Pattern
        /// </summary>
        protected override void Initialize()
        {
            Container.RegisterSingleton<ITumblrdAccountDbMigrations, TumblrAccountDbMigrations>();
            Container.RegisterSingleton<ITumblrCampaignDbMigrations, TumblrCampaignDbMigrations>();
        }
    }
}