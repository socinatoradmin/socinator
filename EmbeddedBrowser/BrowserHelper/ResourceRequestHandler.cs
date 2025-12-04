using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ResourceRequestHandler : IResourceRequestHandler
    {
        private readonly BrowserWindow _embedBrowser;
        readonly SocialNetworks _Sn;
        public ConcurrentQueue<MemoryStreamResponseFilter> ResponseList = new ConcurrentQueue<MemoryStreamResponseFilter>();

        public ConcurrentQueue<KeyValuePair<string, MemoryStreamResponseFilter>> TwitterresponseList =
            new ConcurrentQueue<KeyValuePair<string, MemoryStreamResponseFilter>>();
        public Dictionary<string,List<string>> Headers = new Dictionary<string,List<string>>();
        internal bool IsHeaderNeeded { get; set; }
        public ResourceRequestHandler(BrowserWindow embedBrowser, bool isNeedResourceData = true, SocialNetworks sn = SocialNetworks.Social,bool NeedHeaders = false)
        {
            // get the proxy username

            // get the proxy password

            this._embedBrowser = embedBrowser;

            IsNeedResourceData = isNeedResourceData;

            _Sn = sn;
            IsHeaderNeeded = NeedHeaders;
        }

        public bool IsNeedResourceData { get; set; }

        public void Dispose()
        {
        }
        public void DisposeCollections()
        {
            ResponseList?.ForEach(x => x?.Dispose());
            TwitterresponseList?.ForEach(x => x.Value?.Dispose());
        }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return null;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            try
            {
                if (IsNeedResourceData && MediaUtilites.IsRequiredMimeType(response.MimeType))
                {
                    var dataFilter = new MemoryStreamResponseFilter();
                    ResponseList.FixedEnqueue(dataFilter);
                    if (_Sn == SocialNetworks.Twitter)
                        TwitterresponseList.FixedEnqueue(new KeyValuePair<string, MemoryStreamResponseFilter>(request.Url, dataFilter));
                    return dataFilter;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new MemoryStreamResponseFilter();
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IRequestCallback callback)
        {
            callback?.Dispose();
            return CefReturnValue.Continue;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return request.Url.StartsWith("https://www.facebook.com");
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            try
            {
                if (_embedBrowser?.Browser is null || _embedBrowser.Browser.IsDisposed) return;
                if (!_embedBrowser.Dispatcher.CheckAccess())
                {
                    _embedBrowser.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        if (_embedBrowser?.Browser?.Address == "https://www.youtube.com/oops")
                        {
                            _embedBrowser.Browser.Address = _embedBrowser.SearchUrl;
                            return;
                        }

                        if (_embedBrowser?.Browser?.Address == "https://accounts.google.com/CookieMismatch")
                        {
                            _embedBrowser.Browser.Address = _embedBrowser.SearchUrl = "https://myaccount.google.com/";
                            return;
                        }

                        if (_embedBrowser.SearchUrl == _embedBrowser?.Browser?.Address)
                            return;
                        if (string.IsNullOrWhiteSpace(_embedBrowser.UrlBar.Text))
                            _embedBrowser.UrlBar.Text = "";
                        _embedBrowser.SearchUrl = _embedBrowser?.Browser?.Address;
                    }));
                }
                else
                {
                    if (_embedBrowser?.Browser?.Address == "https://www.youtube.com/oops")
                    {
                        _embedBrowser.Browser.Address = _embedBrowser.SearchUrl;
                        return;
                    }

                    if (_embedBrowser?.Browser?.Address == "https://accounts.google.com/CookieMismatch")
                    {
                        _embedBrowser.Browser.Address = _embedBrowser.SearchUrl = "https://myaccount.google.com/";
                        return;
                    }

                    if (_embedBrowser.SearchUrl == _embedBrowser?.Browser?.Address)
                        return;
                    if (string.IsNullOrWhiteSpace(_embedBrowser.UrlBar.Text))
                        _embedBrowser.UrlBar.Text = "";

                    _embedBrowser.SearchUrl = _embedBrowser?.Browser?.Address;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, ref string newUrl)
        {
            if (IsHeaderNeeded)
            {
                try
                {
                    var headers = response.Headers;
                    if (headers != null)
                    {
                        foreach (var header in headers.AllKeys)
                        {
                            var values = headers.GetValues(header);
                            var data = new List<string>();
                            if (values != null)
                            {
                                foreach (var value in values)
                                {
                                    data.Add(value.ToString());
                                }
                                if (!Headers.ContainsKey(header))
                                    Headers.Add(header, data);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            IResponse response)
        {
            if (IsHeaderNeeded)
            {
                try
                {
                    var headers = response.Headers;
                    if (headers != null)
                    {
                        foreach (var header in headers.AllKeys)
                        {
                            var values = headers.GetValues(header);
                            var data = new List<string>();
                            if (values != null)
                            {
                                foreach (var value in values)
                                {
                                    data.Add(value.ToString());
                                }
                                if (!Headers.ContainsKey(header))
                                    Headers.Add(header, data);
                            }
                        }
                    }
                }
                catch { }
            }
            return false;
        }
    }
}