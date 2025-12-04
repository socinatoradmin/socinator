using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using PinDominator.PdCoreLibrary;

namespace PinDominator.Factories
{
    public class PinterestNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public PinterestNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var pdNetworkCoreFactory = new PdNetworkCoreFactory();
            var pdCoreBuilder = PdCoreBuilder.Instance(pdNetworkCoreFactory, _strategies);
            return pdCoreBuilder.GetPdCoreObjects();
        }
    }
}