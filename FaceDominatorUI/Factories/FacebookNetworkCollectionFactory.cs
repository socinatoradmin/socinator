using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDFactories;
using FaceDominatorUI.FdCoreLibrary;

namespace FaceDominatorUI.Factories
{
    internal class FacebookNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public FacebookNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var fdNetworkCoreFactory = new FdNetworkCoreFactory();
            var fdCoreBuilder = FdCoreBuilder.Instance(fdNetworkCoreFactory, _strategies);
            return fdCoreBuilder.GetFdCoreObjects();
        }
    }
}