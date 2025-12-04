using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using LinkedDominatorCore.Factories;

namespace LinkedDominatorUI.Factories
{
    internal class LinkedInNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public LinkedInNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var ldNetworkCoreFactory = new LDNetworkCoreFactory();
            var ldCoreBuilder = LDCoreBuilder.Instance(ldNetworkCoreFactory, _strategies);
            return ldCoreBuilder.GetLdCoreObjects();
        }
    }
}