using System;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.Common;
using DominatorUIUtility.Properties;

namespace DominatorUIUtility.ViewModel
{
    public interface IVerifyProxiesViewModel
    {
        Task Verify(string VerificationURL = "https://www.google.com",params ProxyManagerModel[] models);
    }

    public class VerifyProxiesViewModel : SynchronizedViewModel, IVerifyProxiesViewModel
    {
        private readonly IProxyFileManager _proxyFileManager;
        private int _total;
        private string _urlToUseToVerifyProxies = "https://www.google.com";
        private int _verified;
        public VerifyProxiesViewModel(IProxyFileManager proxyFileManager)
        {
            _proxyFileManager = proxyFileManager;
            URLToUseToVerifyProxies = _proxyFileManager?.GetProxyManagerSettings()?.VerificationUrl ?? "https://www.google.com";
        }

        public string URLToUseToVerifyProxies
        {
            get => _urlToUseToVerifyProxies;
            set => SetProperty(ref _urlToUseToVerifyProxies, value);
        }

        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public int Verified
        {
            get => _verified;
            set => SetProperty(ref _verified, value);
        }

        public async Task Verify(string VerificationURL = "https://www.google.com",params ProxyManagerModel[] models)
        {
            URLToUseToVerifyProxies = VerificationURL;
            await ExecuteSynchronized(VerifyInternal, models);
        }

        private async Task VerifyInternal(params ProxyManagerModel[] models)
        {
            Total = models.Length;
            Verified = 0;
            foreach (var model in models)
                await CheckProxyAsync(model);
        }

        private async Task CheckProxyAsync(ProxyManagerModel currentProxyManager)
        {
            try
            {
                await _proxyFileManager.UpdateProxyStatusAsync(currentProxyManager, URLToUseToVerifyProxies);
                GlobusLogHelper.log.Info(Log.ProxyVerificationCompleted, SocialNetworks.Social,
                    currentProxyManager.AccountProxy.ProxyIp + ":" + currentProxyManager.AccountProxy.ProxyPort);
                ToasterNotification.ShowSuccess(currentProxyManager.AccountProxy.ProxyIp + ":" +
                                                currentProxyManager.AccountProxy.ProxyPort + "\n" +
                                                "LangKeyProxyVerificationCompleted".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                Verified++;
            }
        }
    }
}