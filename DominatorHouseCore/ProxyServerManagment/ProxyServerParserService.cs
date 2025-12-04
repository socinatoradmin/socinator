#region

using System.Collections.Generic;
using System.Text.RegularExpressions;
using DominatorHouseCore.Models;
using DominatorHouseCore.ProxyServerManagment.Models;

#endregion

namespace DominatorHouseCore.ProxyServerManagment
{
    public interface IProxyServerParserService
    {
        ProxyParsingResult ParseProxies(IReadOnlyCollection<string> rows);
    }

    public class ProxyServerParserService : IProxyServerParserService
    {
        private readonly IProxyValidationService _proxyValidationService;

        public ProxyServerParserService(IProxyValidationService proxyValidationService)
        {
            _proxyValidationService = proxyValidationService;
        }

        public ProxyParsingResult ParseProxies(IReadOnlyCollection<string> rows)
        {
            var lstInvalidProxies = new List<string>();
            var proxies = new List<ProxyManagerModel>();
            foreach (var row in rows)
            {
                var values = Regex.Split(row, "\t");
                if (values.Length < 2)
                    continue;

                var pmm = new ProxyManagerModel();

                if (values.Length == 2 || values.Length == 4)
                {
                    pmm.AccountProxy.ProxyIp = values[0];
                    pmm.AccountProxy.ProxyPort = values[1];
                }

                if (values.Length == 4)
                {
                    pmm.AccountProxy.ProxyUsername = values[2];
                    pmm.AccountProxy.ProxyPassword = values[3];
                }

                if (values.Length == 6 || values.Length > 6)
                {
                    pmm.AccountProxy.ProxyGroup = values[0];
                    pmm.AccountProxy.ProxyName = values[1];
                    pmm.AccountProxy.ProxyIp = values[2];
                    pmm.AccountProxy.ProxyPort = values[3];
                    pmm.AccountProxy.ProxyUsername = values[4];
                    pmm.AccountProxy.ProxyPassword = values[5];
                }

                if (values.Length == 7)
                    pmm.Status = values[6];

                if (_proxyValidationService.IsValidProxy(pmm.AccountProxy.ProxyIp, pmm.AccountProxy.ProxyPort))
                    proxies.Add(pmm);
                else
                    lstInvalidProxies.Add(row);
            }

            return new ProxyParsingResult(lstInvalidProxies, proxies);
        }
    }
}