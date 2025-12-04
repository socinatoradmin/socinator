using Unity;
using Unity.Extension;

namespace RedditDominatorCore.DbMigrations
{
    public class RdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IRdAccountDbMigrations, RdAccountDbMigrations>();
            Container.RegisterSingleton<IRdCampaignDbMigrations, RdCampaignDbMigrations>();
        }
    }
}