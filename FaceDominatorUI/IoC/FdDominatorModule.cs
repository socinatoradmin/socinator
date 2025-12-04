using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.IoC;
using Unity;
using Unity.Resolution;

namespace FaceDominatorUI.IoC
{
    public class FdDominatorModule : ISocialNetworkModule
    {
        private readonly IUnityContainer _unityContainer;

        public FdDominatorModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public SocialNetworks Network => SocialNetworks.Facebook;


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