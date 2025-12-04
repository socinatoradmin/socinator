using Unity;
using Unity.Extension;
namespace GramDominatorCore.DbMigrations
{
    public class GdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IGdAccountDbMigrations, GdAccountDbMigrations>();
            Container.RegisterSingleton<IGdCampaignDbMigrations, GdCampaignDbMigrations>();
        }
    }
}
