using Unity;
using Unity.Extension;

namespace LinkedDominatorCore.DbMigrations
{
    public class LdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<ILdAccountDbMigrations, LdAccountDbMigrations>();
            Container.RegisterSingleton<ILdCampaignDbMigrations, LdCampaignDbMigrations>();
        }
    }
}