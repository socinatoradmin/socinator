#region

using DominatorHouseCore.Dal.DbMigrations;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.Dal
{
    public class DbMigrationUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IGlobalDatabaseMigrations, GlobalDatabaseMigrations>();
            Container.RegisterSingleton<IGlobalDatabaseBlackListMigrations, GlobalDatabaseBlackListMigrations>();
            Container.RegisterSingleton<IGlobalDatabaseWhiteListMigrations, GlobalDatabaseWhiteListMigrations>();
        }
    }
}