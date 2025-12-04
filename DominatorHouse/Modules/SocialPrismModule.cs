using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DominatorHouse.Modules
{
    // ReSharper disable once UnusedMember.Global
    // the class is used through the configuration (see app.config)
    public class SocialPrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // ReSharper disable once UnusedVariable
            var regionManager = containerProvider.Resolve<IRegionManager>();
        }
    }
}
