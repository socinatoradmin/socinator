using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using YoutubeDominatorUI.YdCoreLibrary;

namespace YoutubeDominatorUI.Factories
{
    internal class YoutubeNetworkCollectionFactory : INetworkCollectionFactory
    {
        private readonly AccessorStrategies _strategies;

        public YoutubeNetworkCollectionFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
        }

        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var ydNetworkCoreFactory = new YdNetworkCoreFactory();
            var ydCoreBuilder = YdCoreBuilder.Instance(ydNetworkCoreFactory, _strategies);
            return ydCoreBuilder.GetYdCoreObjects();
        }
    }
}