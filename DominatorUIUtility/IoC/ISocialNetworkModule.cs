using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.IoC
{
    public interface ISocialNetworkModule
    {
        SocialNetworks Network { get; }

        INetworkCollectionFactory GetNetworkCollectionFactory(
            AccessorStrategies strategies);

        IPublisherCollectionFactory GetPublisherCollectionFactory();
    }
}