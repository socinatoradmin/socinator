#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface IProxyFileManager
    {
        bool SaveProxy(ProxyManagerModel proxy);
        List<ProxyManagerModel> GetAllProxy();
        void EditProxy(ProxyManagerModel proxy);
        void EditAllProxy(List<ProxyManagerModel> proxy);
        void Delete(Predicate<ProxyManagerModel> match);
        ProxyManagerModel GetProxyById(string ProxyId);
        Task UpdateProxyStatusAsync(ProxyManagerModel currentProxyManager, string url);
        bool VerifyProxy(Proxy currentProxy, string url);

        bool SaveProxyManagerSettings(ProxyManagerSettings setting);
        ProxyManagerSettings GetProxyManagerSettings();
    }

    public class ProxyFileManager : IProxyFileManager
    {
        private readonly IBinFileHelper _binFileHelper;

        public ProxyFileManager()
        {
            _binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        public bool SaveProxy(ProxyManagerModel proxy)
        {
            try
            {
                _binFileHelper.SaveProxy(proxy);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ProxyManagerModel> GetAllProxy()
        {
            return _binFileHelper.GetProxyDetails();
        }


        public void EditProxy(ProxyManagerModel proxy)
        {
            _binFileHelper.UpdateProxy(proxy);
        }

        public void EditAllProxy(List<ProxyManagerModel> proxy)
        {
            _binFileHelper.UpdateAllProxy(proxy);
        }


        public void Delete(Predicate<ProxyManagerModel> match)
        {
            var proxy = _binFileHelper.GetProxyDetails();
            proxy.RemoveAll(match);
            _binFileHelper.UpdateAllProxy(proxy);
        }


        public ProxyManagerModel GetProxyById(string ProxyId)
        {
            return GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyId == ProxyId);
        }


        public bool VerifyProxy(Proxy currentProxy, string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(new Uri(url));
                request.Timeout = 5000;

                if (currentProxy != null)
                {
                    request.Proxy = new WebProxy(currentProxy.ProxyIp, int.Parse(currentProxy.ProxyPort))
                    {
                        BypassProxyOnLocal = true
                    };
                    if (!string.IsNullOrEmpty(currentProxy.ProxyUsername)
                        && !string.IsNullOrEmpty(currentProxy.ProxyPassword))
                        request.Proxy.Credentials =
                            new NetworkCredential(currentProxy.ProxyUsername, currentProxy.ProxyPassword);

                    GlobusLogHelper.log.Info(Log.ProxyVerificationStarted, SocialNetworks.Social,
                        currentProxy.ProxyIp + ":" + currentProxy.ProxyPort);

                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        if (response.StatusCode.ToString() == "OK")
                            return true;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

            return false;
        }


        public async Task UpdateProxyStatusAsync(ProxyManagerModel currentProxyManager, string url)
        {
            var stopWatch = new Stopwatch();

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    GlobusLogHelper.log.Info("LangKeyEnterURLInInputFieldToVerifyProxy".FromResourceDictionary());
                    return;
                }

                var request = (HttpWebRequest) WebRequest.Create(new Uri(url));
                request.Timeout = 5000;

                if (currentProxyManager != null)
                {
                    request.Proxy = new WebProxy(currentProxyManager.AccountProxy.ProxyIp,
                        int.Parse(currentProxyManager.AccountProxy.ProxyPort))
                    {
                        BypassProxyOnLocal = true
                    };
                    if (!string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyUsername)
                        && !string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyPassword))
                        request.Proxy.Credentials = new NetworkCredential(
                            currentProxyManager.AccountProxy.ProxyUsername,
                            currentProxyManager.AccountProxy.ProxyPassword);

                    stopWatch.Start();
                    GlobusLogHelper.log.Info(Log.ProxyVerificationStarted, SocialNetworks.Social,
                        currentProxyManager.AccountProxy.ProxyIp + ":" + currentProxyManager.AccountProxy.ProxyPort);

                    using (var response = (HttpWebResponse) await request.GetResponseAsync())
                    {
                        currentProxyManager.Status = response.StatusCode.ToString() == "OK" ? "Working" : "Not Working";
                    }
                }
            }
            catch (Exception)
            {
                if (currentProxyManager != null)
                {
                    currentProxyManager.Status = "Fail";
                    currentProxyManager.Failures = 1;
                }
            }
            finally
            {
                if (currentProxyManager != null && currentProxyManager.Status == "Working")
                    currentProxyManager.Failures = 0;

                stopWatch.Stop();
                var ts = stopWatch.Elapsed;
                currentProxyManager.ResponseTime = $"{ts.Milliseconds} milli seconds";
            }

            EditProxy(currentProxyManager);
        }

        public bool SaveProxyManagerSettings(ProxyManagerSettings setting)
        {
            return _binFileHelper.SaveProxyManagerSettings(setting);
        }

        public ProxyManagerSettings GetProxyManagerSettings()
        {
            return _binFileHelper.GetProxyManagerSettings();
        }
    }
}