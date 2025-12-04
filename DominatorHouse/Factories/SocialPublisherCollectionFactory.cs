using DominatorHouse.DominatorCores;
using DominatorHouseCore.Interfaces;

namespace Socinator.Factories
{
    public class SocialPublisherCollectionFactory  : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {          
            var socialPublisherCoreFactory = new SocialPublisherCoreFactory();
            var socialPublisherCoreBuilder = SocialPublisherCoreBuilder.Instance(socialPublisherCoreFactory);
            return socialPublisherCoreBuilder.GetSocialPublisherCoreObjects();
        }
    }
}