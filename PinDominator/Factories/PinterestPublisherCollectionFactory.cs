using DominatorHouseCore.Interfaces;
using PinDominator.PdCoreLibrary;
using PinDominatorCore.PDFactories;

namespace PinDominator.Factories
{
    public class PinterestPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var pdPublisherCoreFactory = new PdPublisherCoreFactory();
            var pdPublisherCoreBuilder = PdPublisherCoreBuilder.Instance(pdPublisherCoreFactory);
            return pdPublisherCoreBuilder.GetPdPublisherCoreObjects();
        }
    }
}