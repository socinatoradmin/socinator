using Unity;
using Unity.Extension;

namespace PinDominatorCore.DbMigrations
{
    public class PdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IPdAccountDbMigrations, PdAccountDbMigrations>();
            Container.RegisterSingleton<IPdCampaignDbMigrations, PdCampaignDbMigrations>();
        }
    }
}