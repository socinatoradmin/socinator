using System.Security.Cryptography.X509Certificates;
using CefSharp;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ProxyRequestHandler : IRequestHandler
    {
        private readonly string _password;

        private readonly string _userName;

        public ResourceRequestHandler ResourceRequestHandler;

        public ProxyRequestHandler(string userName, string password, BrowserWindow embedBrowser
            , bool isNeedResourceData = false, DominatorHouseCore.Enums.SocialNetworks sn = DominatorHouseCore.Enums.SocialNetworks.Social,
            bool NeedRequestHeaders = false)
        {
            // get the proxy username
            _userName = userName;

            // get the proxy password
            _password = password;

            IsNeedResourceData = isNeedResourceData;

            ResourceRequestHandler = new ResourceRequestHandler(embedBrowser, IsNeedResourceData,sn,NeedRequestHeaders);
        }


        public bool IsNeedResourceData { get; set; }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture);
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
                callback.Continue(_userName, _password);

                return true;
            }

            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback)
        {
            callback?.Dispose();
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