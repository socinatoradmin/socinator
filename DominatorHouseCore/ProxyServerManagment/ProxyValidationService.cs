#region

using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.ProxyServerManagment
{
    public interface IProxyValidationService
    {
        bool IsValidProxy(string proxyAddress, string proxyPort);
    }

    public class ProxyValidationService : IProxyValidationService
    {
        public bool IsValidProxy(string proxyAddress, string proxyPort)
        {
            return IsValidProxyIp(proxyAddress) && IsValidProxyPort(proxyPort);
        }

        private bool IsValidProxyIp(string proxyAddress)
        {
            return !string.IsNullOrWhiteSpace(proxyAddress) && Proxy.IsValidProxyIp(proxyAddress);
        }

        private bool IsValidProxyPort(string proxyPort)
        {
            return !string.IsNullOrWhiteSpace(proxyPort) && Proxy.IsValidProxyPort(proxyPort);
        }
    }
}