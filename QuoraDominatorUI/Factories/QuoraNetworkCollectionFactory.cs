using DominatorHouseCore.Interfaces;
using QuoraDominatorUI.QDCoreLibrary;

namespace QuoraDominatorUI.Factories
{
    internal class QuoraNetworkCollectionFactory : INetworkCollectionFactory
    {
        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var qdNetworkCoreFactory = new QdNetworkCoreFactory();
            var qdCoreBuilder = QdCoreBuilder.Instance(qdNetworkCoreFactory);
            return qdCoreBuilder.GetQdCoreObjects();
        }
    }
}