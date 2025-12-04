using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDFactories;
using FaceDominatorUI.FdCoreLibrary;

namespace FaceDominatorUI.Factories
{
    public class FacebookPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var fdPublisherCoreFactory = new FdPublisherCoreFactory();
            var fdPublisherCoreBuilder = FdPublisherCoreBuilder.Instance(fdPublisherCoreFactory);
            return fdPublisherCoreBuilder.GetFdPublisherCoreObjects();
        }
    }
}