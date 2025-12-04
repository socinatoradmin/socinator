using DominatorHouseCore.Interfaces;
using Socinator.DominatorCores;

namespace Socinator.Factories
{
    internal class SocialNetworkCollectionFactory : INetworkCollectionFactory
    {
        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var dominatorNetworkCoreFactory = new DominatorNetworkCoreFactory();
            var dominatorCoreBuilder = DominatorCoreBuilder.Instance(dominatorNetworkCoreFactory);
            return dominatorCoreBuilder.GetDominatorCoreObjects();
        }
    }
}