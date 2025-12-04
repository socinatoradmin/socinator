using Unity;
using Unity.Extension;

namespace YoutubeDominatorCore.DbMigrations
{
    public class YdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IYdAccountDbMigrations, YdAccountDbMigrations>();
            Container.RegisterSingleton<IYdCampaignDbMigrations, YdCampaignDbMigrations>();
        }
    }
}