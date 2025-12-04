using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorUI.QDCoreLibrary;

namespace QuoraDominatorUI.Factories
{
    internal class QuoraPublisherCollectionFactory : IPublisherCollectionFactory
    {
        public IPublisherCoreFactory GetPublisherCoreFactory()
        {
            var qdPublisherCoreFactory = new QdPublisherCoreFactory();
            var qdPublisherCoreBuilder = QdPublisherCoreBuilder.Instance(qdPublisherCoreFactory);
            return qdPublisherCoreBuilder.GetQdPublisherCoreObjects();
        }
    }
}