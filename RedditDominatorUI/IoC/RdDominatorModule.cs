using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.IoC;
using Unity;
using Unity.Resolution;

namespace RedditDominatorUI.IoC
{
    public class RdDominatorModule : ISocialNetworkModule
    {
        private readonly IUnityContainer _unityContainer;

        public RdDominatorModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public SocialNetworks Network => SocialNetworks.Reddit;


        public INetworkCollectionFactory GetNetworkCollectionFactory(AccessorStrategies strategies)
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