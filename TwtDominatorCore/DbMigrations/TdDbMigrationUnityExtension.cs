using TwtDominatorCore.DbMigration;
using Unity;
using Unity.Extension;

namespace TwtDominatorCore.DbMigrations
{
    public class TdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<ITdAccountDbMigrations, TdAccountDbMigrations>();
            Container.RegisterSingleton<ITdCampaignDbMigrations, TdCampaignDbMigrations>();
        }
    }
}