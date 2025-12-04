using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using RedditDominatorUI.RdCoreLibrary;

namespace RedditDominatorUI.Factories
{
    internal class RedditNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public RedditNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var rdNetworkCoreFactory = new RdNetworkCoreFactory();
            var rdCoreBuilder = RdCoreBuilder.Instance(rdNetworkCoreFactory, _strategies);
            return rdCoreBuilder.GetRdCoreObjects();
        }
    }
}