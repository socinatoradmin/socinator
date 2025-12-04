using Unity;
using Unity.Extension;

namespace QuoraDominatorCore.DbMigrations
{
    public class QdDbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IQdAccountDbMigrations, QdAccountDbMigrations>();
            Container.RegisterSingleton<IQdCampaignDbMigrations, QdCampaignDbMigrations>();
        }
    }
}