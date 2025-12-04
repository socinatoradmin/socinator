using DominatorHouseCore.Interfaces;
using TumblrDominatorUI.TumblrCoreLibrary;

namespace TumblrDominatorUI.Factories
{
    internal class TumblrNetworkCollectionFactory : INetworkCollectionFactory
    {
        /// <summary>
        ///     Implementing Constructor Injection for Tumblr Network
        /// </summary>
        /// <returns></returns>
        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var tumblrNetworkCoreFactory = new TumblrNetworkCoreFactory();
            var tumblrCoreBuilder = TumblrCoreBuilder.Instance(tumblrNetworkCoreFactory);
            return tumblrCoreBuilder.GetTumblrCoreObjects();
        }
    }
}