using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using CefSharp;

namespace EmbeddedBrowser.BrowserHelper
{
    public class RequestHandlerCustom : IRequestHandler
    {
        private readonly BrowserWindow _embedBrowser;

        public ResourceRequestHandler ResourceRequestHandler;

        public RequestHandlerCustom(BrowserWindow embedBrowser, bool isNeedResourceData = false, DominatorHouseCore.Enums.SocialNetworks sn = DominatorHouseCore.Enums.SocialNetworks.Social,bool NeedHeaders = false)
        {
            this._embedBrowser = embedBrowser;
            IsNeedResourceData = isNeedResourceData;
            ResourceRequestHandler = new ResourceRequestHandler(embedBrowser, IsNeedResourceData, sn, NeedHeaders);
        }


        public bool IsNeedResourceData { get; set; }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            bool userGesture, bool isRedirect)
        {
            if (request.Url.Contains("safety/go?url"))
            {
                Regex pattern = new Regex(@"url=(.*?)&");
                var match = pattern.Match(request.Url).Groups[1].Value;
                chromiumWebBrowser.Load(Uri.UnescapeDataString(match));
            }
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator,
            ref bool disableDefaultHandling)
        {
            return ResourceRequestHandler;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy,
            string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (isProxy)
            {
                callback.Continue(_embedBrowser.DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                    _embedBrowser.DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword);

                return true;
            }

            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode,
            string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy,
            string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser,
            CefTerminationStatus status)
        {
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }
    }
}