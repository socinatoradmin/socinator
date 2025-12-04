using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.Factories;

namespace LinkedDominatorUI.Factories
{
    public class LinkedInPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var ldPublisherCoreFactory = new LDPublisherCoreFactory();
            var ldPublisherCoreBuilder = LDPublisherCoreBuilder.Instance(ldPublisherCoreFactory);
            return ldPublisherCoreBuilder.GetLDPublisherCoreObjects();
        }
    }
}