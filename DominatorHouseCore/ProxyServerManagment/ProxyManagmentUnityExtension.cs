#region

using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.ProxyServerManagment
{
    internal class ProxyManagmentUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IProxyServerParserService, ProxyServerParserService>();
            Container.RegisterSingleton<IProxyValidationService, ProxyValidationService>();
        }
    }
}