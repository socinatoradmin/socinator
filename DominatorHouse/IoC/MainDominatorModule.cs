using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.IoC;
using Unity;
using Unity.Resolution;

namespace DominatorHouse
{
    public class MainDominatorModule : ISocialNetworkModule
    {
        public SocialNetworks Network => SocialNetworks.Social;

        private readonly IUnityContainer _unityContainer;

        public MainDominatorModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }


        public INetworkCollectionFactory GetNetworkCollectionFactory(
            AccessorStrategies strategies)
        {
            return _unityContainer.Resolve<INetworkCollectionFactory>(Network.ToString(),
                new ParameterOverride("strategies", strategies));
        }


        public IPublisherCollectionFactory GetPublisherCollectionFactory()
        {
            return _unityContainer.Resolve<IPublisherCollectionFactory>(Network.ToString());
        }
    }
}
