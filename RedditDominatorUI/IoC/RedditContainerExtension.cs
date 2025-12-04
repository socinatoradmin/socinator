using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel;
using RedditDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Resolution;

namespace RedditDominatorUI.IoC
{
    public class RedditContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentyNetwork = SocialNetworks.Reddit;
        protected override void Initialize()
        {
            // specify name of the module (CurrentyNetwork.ToString()) when you think it will be many registration upon your interface
            Container.RegisterType<ISocialNetworkModule, RedditDominatorModule>(CurrentyNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, RedditNetworkCollectionFactory>(CurrentyNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, RedditPublisherCollectionFactory>(CurrentyNetwork.ToString());
        }
    }

    public class RedditDominatorModule : ISocialNetworkModule
    {
        public SocialNetworks Network => SocialNetworks.Reddit;

        private readonly IUnityContainer _unityContainer;

        public RedditDominatorModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }


        public INetworkCollectionFactory GetNetworkCollectionFactory(
            DominatorAccountViewModel.AccessorStrategies strategies)
        {
            return _unityContainer.Resolve<INetworkCollectionFactory>(Network.ToString(),
                new ParameterOverrides { { "strategies", strategies } });
        }


        public IPublisherCollectionFactory GetPublisherCollectionFactory()
        {
            return _unityContainer.Resolve<IPublisherCollectionFactory>(Network.ToString());
        }
    }
}
