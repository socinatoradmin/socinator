using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorUI.GDCoreLibrary;

namespace GramDominatorUI.Factories
{
    internal class InstagramNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public InstagramNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var gdNetworkCoreFactory = new GdNetworkCoreFactory();
            var gdCoreBuilder = GdCoreBuilder.Instance(gdNetworkCoreFactory, _strategies);
            return gdCoreBuilder.GetGDCoreObjects();
        }
    }
}