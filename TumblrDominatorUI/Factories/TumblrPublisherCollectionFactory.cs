using DominatorHouseCore.Interfaces;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorUI.TumblrCoreLibrary;

namespace TumblrDominatorUI.Factories
{
    internal class TumblrPublisherCollectionFactory : IPublisherCollectionFactory
    {
        /// <summary>
        ///     Implementing Constructor Injection for Tumblr Post Publisher
        /// </summary>
        /// <returns></returns>
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var tumPublisherCoreFactory = new TumblrPublisherCoreFactory();
            var tumPublisherCoreBuilder = TumblrPublisherCoreBuilder.Instance(tumPublisherCoreFactory);
            return tumPublisherCoreBuilder.TumblrPublisherCoreObjects();
        }
    }
}