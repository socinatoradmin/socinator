using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PinDominatorCore.PDLibrary.PowerAdsSpy
{
    public interface IAdsScraperFunction
    {
        string Domain { get; set; }
        IPdHttpHelper HttpHelper { get; set; }
        void GetAllPinsForAdsScrape(DominatorAccountModel dominatorAccountModel, string adsresponse);
    }
    public class AdsScraperFunction : IAdsScraperFunction
    {
        private readonly IDelayService _delayService;        
        public IPdHttpHelper HttpHelper { get; set; }
        public string Domain { get; set; }        
        //private readonly IAccountScopeFactory _accountScopeFactory;
        public AdsScraperFunction(IPdHttpHelper httpHelper, IDelayService delayService)
        {
            _delayService = delayService;
            HttpHelper = httpHelper;
            //_accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }
        public void GetAllPinsForAdsScrape(DominatorAccountModel dominatorAccountModel, string adsresponse)
        {
            try
            {
                try
                {                    
                    string ticks = null;
                    string geturl = string.Empty;
                    int count_for_no_Ads = 0;
                    AdsScraperResponseHandler adsscraperResponseHandler = null;
                    string bookMark = Utilities.GetBetween(adsresponse, "\"bookmarks\":[\"", "\"]");
                    SetHeaders(dominatorAccountModel);
                    int count = 0;
                    var location = GetIpDetails(dominatorAccountModel, HttpHelper);
                    do
                    {
                        if (string.IsNullOrEmpty(ticks))
                            ticks = DateTime.Now.Ticks.ToString();

                        //for business account  bookmark is not getting first time
                        if (string.IsNullOrEmpty(bookMark))
                        {
                            geturl = "https://www.pinterest.com/resource/UserHomefeedResource/get/?source_url=/homefeed/&data={\"options\":{\"isPrefetch\":false,\"field_set_key\":\"hf_grid_partner\",\"in_nux\":false,\"in_news_hub\":false,\"prependPartner\":true,\"static_feed\":false,\"no_fetch_context_on_resource\":false},\"context\":{}}&_=" + $"{ticks}";
                        }
                        else
                        {
                            geturl = "https://www.pinterest.com/resource/UserHomefeedResource/get/?source_url=/&data={\"options\":{\"bookmarks\":[\"" + $"{ bookMark}" +
                            "  \"],\"isPrefetch\":false,\"field_set_key\":\"hf_grid\",\"in_nux\":false,\"in_news_hub\":false,\"prependPartner\":false,\"static_feed\":false,\"no_fetch_context_on_resource\":false},\"context\":{}}&_=" + $"{ticks}";
                        }
                        dominatorAccountModel.Token.ThrowIfCancellationRequested();
                        var response = HttpHelper.GetRequest(geturl);
                        adsscraperResponseHandler = new AdsScraperResponseHandler(response, location);
                        bookMark = adsscraperResponseHandler.BookMark.ToString();
                        count++;
                        if (adsscraperResponseHandler.LstPin.Count > 0)
                        {
                            AddDataInToApi(adsscraperResponseHandler.LstPin);
                        }
                        else
                        {
                            count_for_no_Ads++;
                            count--;
                        }
                        //for some countries ads is not showing so we hit for 2 times
                        //and if we not get any ads then will break it
                        if (count_for_no_Ads > 1)
                            break;

                    } while (count < 2);

                }
                catch (Exception)
                {
                    //ex.DebugLog();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }


        public Dictionary<IpLocationDetails, string> GetIpDetails(DominatorAccountModel dominatorAccountModel, IPdHttpHelper httpHelper)
        {
            var lockerRequest = new object();
            var dictIpDetails = new Dictionary<IpLocationDetails, string>();
            var proxyLocationDetails = string.Empty;
            try
            {
                lock (lockerRequest)
                {
                    try
                    {
                        _delayService.ThreadSleep(2000);
                        var objProxy = new Proxy
                        {
                            ProxyIp = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp,
                            ProxyPort = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort,
                            ProxyUsername = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                            ProxyPassword = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword
                        };

                        var parameter = HttpHelper.GetRequestParameter();

                        parameter.Proxy = objProxy;
                        var ip = string.Empty;
                        if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                        {
                            ip = httpHelper.GetRequest("https://app.multiloginapp.com/WhatIsMyIP").Response;
                            ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                            ip = Utilities.GetBetween(ip, ">", "<").Trim();
                        }
                        else
                            ip = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;

                        try
                        {
                            //var locationUrl = "https://api.db-ip.com/v2/eb79c26170d0e9921e5b8372b2e212f86afa399c/" + ip.Trim();
                            var locationUrl = $"http://ip-api.com/json/{ip.Trim()}";

                            proxyLocationDetails = httpHelper.GetRequest(locationUrl).Response;

                            if (proxyLocationDetails.Contains("INVALID_ADDRESS"))
                            {
                                ip = httpHelper.GetRequest("https://app.multiloginapp.com/WhatIsMyIP").Response;
                                ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                                ip = Utilities.GetBetween(ip, ">", "<").Trim();

                                locationUrl = $"http://ip-api.com/json/{ip.Trim()}";

                                proxyLocationDetails = httpHelper.GetRequest(locationUrl).Response;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        //To get district,city , state and country
                        //var district = Utilities.GetBetween(proxyLocationDetails, "district\": \"", "\"");
                        var city = Utilities.GetBetween(proxyLocationDetails, "\"city\":\"", "\"");
                        var state = Utilities.GetBetween(proxyLocationDetails, "regionName\":\"", "\"");
                        var country = Utilities.GetBetween(proxyLocationDetails, "country\":\"", "\"");

                        //Added the information into dictionary
                        dictIpDetails.Add(IpLocationDetails.City, !string.IsNullOrEmpty(city) ? city : string.Empty);
                        dictIpDetails.Add(IpLocationDetails.State, !string.IsNullOrEmpty(state) ? state : string.Empty);
                        dictIpDetails.Add(IpLocationDetails.Country, !string.IsNullOrEmpty(country) ? country : string.Empty);
                        dictIpDetails.Add(IpLocationDetails.Ip, !string.IsNullOrEmpty(ip) ? ip : string.Empty);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return dictIpDetails;
        }

        private void AddDataInToApi(List<AdsDataModel> lstPin)
        {
            try
            {
                var apiUrl = "https://pint.poweradspy.com/api/insert_ads";
                var result = string.Empty;
                foreach (var data in lstPin)
                {
                    try
                    {
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.KeepAlive = true;
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var serializedData = JsonConvert.SerializeObject(data, Formatting.Indented);                            
                            streamWriter.Write(serializedData);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        _delayService.ThreadSleep(1000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
            }

        }



        public IRequestParameters SetHeaders(DominatorAccountModel objDominatorAccountModel)
        {
            var requestHeader = HttpHelper.GetRequestParameter();

            objDominatorAccountModel.UserAgentWeb = PdConstants.UserAgent;

            requestHeader.Proxy = objDominatorAccountModel.AccountBaseModel.AccountProxy;
            requestHeader.Headers = new WebHeaderCollection();
            requestHeader.UserAgent = PdConstants.UserAgent;
            requestHeader.ContentType = "application/x-www-form-urlencoded";
            requestHeader.Accept = "application/json, text/javascript, *; q=0.01";
            requestHeader.KeepAlive = true;
            requestHeader.Headers["Accept-Language"] = "en-US,en;q=0.9";
            requestHeader.Headers["X-Requested-With"] = "XMLHttpRequest";
            requestHeader.Headers.Add("X-Pinterest-AppState", "active");
            requestHeader.Referer = "";

            if (string.IsNullOrEmpty(Domain) && HttpHelper.GetRequestParameter().Cookies != null &&
                HttpHelper.GetRequestParameter().Cookies.Count > 0)
            {
                Domain = HttpHelper.GetRequestParameter()?.Cookies["csrftoken"].Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
            }

            //Assign browser cookies if Http mode don't have cookies
            if ((requestHeader.Cookies == null || requestHeader.Cookies.Count < 5) && objDominatorAccountModel.Cookies.Count != 0)
                requestHeader.Cookies = objDominatorAccountModel.Cookies;

            if (requestHeader.Cookies != null)
                foreach (Cookie item in requestHeader.Cookies)
                    if (item.Name == "csrftoken")
                    {
                        var csrftokenValue = item.Value;
                        requestHeader.Headers.Add("X-CSRFToken", csrftokenValue);
                        break;
                    }

            HttpHelper.SetRequestParameter(requestHeader);
            return requestHeader;
        }
    }
}
