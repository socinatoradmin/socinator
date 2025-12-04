using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDFactories;
using GramDominatorUI.GDCoreLibrary;

namespace GramDominatorUI.Factories
{
    public class InstagramPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var gdPublisherCoreFactory = new GdPublisherCoreFactory();
            var gdPublisherCoreBuilder = GdPublisherCoreBuilder.Instance(gdPublisherCoreFactory);
            return gdPublisherCoreBuilder.GetGdPublisherCoreObjects();
        }
    }
}