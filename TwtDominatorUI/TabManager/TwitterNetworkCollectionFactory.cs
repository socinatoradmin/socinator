using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using TwtDominatorUI.TdCoreLibrary;

namespace TwtDominatorUI.Factories
{
    internal class TwitterNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public TwitterNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var tdNetworkCoreFactory = new TdNetworkCoreFactory();
            var tdCoreBuilder = TdCoreBuilder.Instance(tdNetworkCoreFactory, _strategies);
            return tdCoreBuilder.GetTdCoreObjects();
        }
    }
}