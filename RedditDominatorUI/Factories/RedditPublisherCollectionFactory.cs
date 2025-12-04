using DominatorHouseCore.Interfaces;
using RedditDominatorCore.RDFactories;
using RedditDominatorUI.RdCoreLibrary;

namespace RedditDominatorUI.Factories
{
    public class RedditPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var rdPublisherCoreFactory = new RdPublisherCoreFactory();
            var rdPublisherCoreBuilder = RdPublisherCoreBuilder.Instance(rdPublisherCoreFactory);
            return rdPublisherCoreBuilder.GetRdPublisherCoreObjects();
        }
    }
}