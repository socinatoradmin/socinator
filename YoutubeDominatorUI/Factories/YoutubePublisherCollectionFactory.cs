using DominatorHouseCore.Interfaces;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorUI.YdCoreLibrary;

namespace YoutubeDominatorUI.Factories
{
    public class YoutubePublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var ydPublisherCoreFactory = new YdPublisherCoreFactory();
            var ydPublisherCoreBuilder = YdPublisherCoreBuilder.Instance(ydPublisherCoreFactory);
            return ydPublisherCoreBuilder.GetPdPublisherCoreObjects();
        }
    }
}