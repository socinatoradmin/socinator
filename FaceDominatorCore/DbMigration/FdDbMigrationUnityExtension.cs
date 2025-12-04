using Unity;
using Unity.Extension;

namespace FaceDominatorCore.DbMigration
{
    public class FdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IFdAccountDbMigrations, FdAccountDbMigrations>();
            Container.RegisterSingleton<IFdCampaignDbMigrations, FdCampaignDbMigrations>();
        }
    }
}
