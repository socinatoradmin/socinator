using DominatorHouseCore.Interfaces;
using TwtDominatorCore.Factories;
using TwtDominatorUI.TdCoreLibrary;

namespace TwtDominatorUI.Factories
{
    public class TwitterPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var tdPublisherCoreFactory = new TdPublisherCoreFactory();
            var tdPublisherCoreBuilder = TdPublisherCoreBuilder.Instance(tdPublisherCoreFactory);
            return tdPublisherCoreBuilder.GetTdPublisherCoreObjects();
        }
    }
}