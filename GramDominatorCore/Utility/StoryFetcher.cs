using CefSharp.DevTools.IO;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GramDominatorCore.Utility
{
    public class StoryFetcher
    {
        public IgHttpHelper helper { get; set; }
        public StoryFetcher()
        {
            helper = new IgHttpHelper();
        }
        public async Task<List<SessionModel>> GetParam(string Url, Dictionary<string,string> paramnames=null)
        {
            var param = new List<SessionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,Url);
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-platform", "\"macOS\"");
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 12.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.6811.57 Safari/537.36");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    request.Headers.Add("Sec-Fetch-Site", "none");
                    request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    request.Headers.Add("Sec-Fetch-User", "?1");
                    request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");

                    var response = await client.SendAsync(request);
                    var headers = response.Headers;
                    var cookies = headers.GetValues("Set-Cookie");
                    if (cookies != null && cookies.Count() > 0)
                    {
                        foreach(var item in paramnames)
                        {
                            var itemValue = cookies.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains(item.Key));
                            if (!string.IsNullOrEmpty(itemValue))
                            {
                                param.Add(new SessionModel
                                {
                                    key = item.Key,
                                    value = Utilities.GetBetween(itemValue, item.Key + "=", ";"),
                                    domain = item.Value,
                                    expires = Utilities.GetBetween(itemValue, "expires=", ";")
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return param;
        }

        public async Task<string> HitRequest(string API, List<SessionModel> param, string Body,
            string Origin,string Referer,StorySource source=StorySource.Storynavigation)
        {
            var cookieContainer = new CookieContainer();
            foreach (var val in param)
            {
                cookieContainer.Add(new Cookie { Name = val.key, Value = val.value, Domain = val.domain });
            }
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            var client = new HttpClient(handler);
            // Add headers
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", WebUtility.UrlDecode(param.FirstOrDefault(x => x.key == "XSRF-TOKEN").value)); // truncated for clarity
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.6829.70 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
            client.DefaultRequestHeaders.Add("Origin", Origin);
            client.DefaultRequestHeaders.Add("Referer", Referer);
            var content = new StringContent(Body, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await client.PostAsync(API, content);

            // Output response
            var stream = await response.Content.ReadAsStreamAsync();
            return helper.GetDecodedResponse(stream, string.Join(", ", response.Content.Headers.ContentEncoding)).Response;
        }
        public string GetInstaUsername(string instaUsername)
        {
            if (string.IsNullOrEmpty(instaUsername) || !instaUsername.Contains("www.instagram.com"))
                return instaUsername;
            var splitted = instaUsername.Split('/').ToList();
            return splitted.LastOrDefault(x => !string.IsNullOrEmpty(x));
        }
        public int GenerateSixDigitNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000);
        }
        public async Task<string> GetResponse(string Username, string API)
        {
            var hash = ComputeSha256Hash(Username);
            using (HttpClient client = new HttpClient())
            {
                // JSON body
                var jsonBody = new
                {
                    username = Username,
                    ts = 1745319687828,//DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    _ts = 1744817817869,//DateTimeOffset.UtcNow.AddDays(-5).ToUnixTimeMilliseconds(),
                    _tsc = 109004, //GenerateSixDigitNumber(),
                    _s = "05ac695b8e898b9825e2e139e4d84d38e40cf97df924d280a724b13455e93319"//hash
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(jsonBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // Custom headers
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Linux\"");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.6832.83 Safari/537.36");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"132\", \"Chromium\";v=\"132\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("Origin", "https://storiesig.info");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Referer", "https://storiesig.info/");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"132\", \"Chromium\";v=\"132\"");

                // Send POST
                var response = await client.PostAsync(API, content);

                // Output
                return await response.Content.ReadAsStringAsync();
            }
        }
        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to hex string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // lower-case hex
                }
                return builder.ToString();
            }
        }
        public async Task<bool> IsImageVisibleAsync(string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, imageUrl))
                    {
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            return response.IsSuccessStatusCode &&
                                response.Content.Headers.ContentType.MediaType.StartsWith("image");
                        }
                    }

                } // Send HEAD request to check headers only


                // Check if status is OK (200) and content type is an image

            }
            catch
            {
                return false;
            }
        }

        public async Task<StoryCollection> GetStoryByBrowser(string username, StoryCollection StoryCollection,ICommand downloadMedia,ICommand DownloadHighlight)
        {
            if(StoryCollection is null)
                StoryCollection = new StoryCollection();
            if (string.IsNullOrEmpty(username))
                return StoryCollection;
            var browser = new PuppeteerBrowserActivity(new DominatorHouseCore.Models.DominatorAccountModel()
            {
                CancellationSource=new System.Threading.CancellationTokenSource(),
                AccountBaseModel = new DominatorHouseCore.Models.DominatorAccountBaseModel()
            },isNeedResourceData:true);
            try
            {
                var HeadLess = true;
#if DEBUG
                HeadLess = false;
#endif
                await browser.LaunchBrowserAsync(HeadLess: HeadLess, targetUrl: "https://anonyig.com/en/");
                var script = "document.querySelector('input[type=\"text\"]').getBoundingClientRect().{0};";
                var XandY = await browser.GetXAndYAsync(customScriptX: string.Format(script, "x"), customScriptY: string.Format(script, "y"));
                await browser.MouseClickAsync(XandY.Key+10, XandY.Value+5,delayAfter:2);
                await browser.EnterCharsAsync(username, delayAtLast:3);
                await browser.ExecuteScriptAsync("document.querySelector('button[class=\"search-form__button\"]').click();",delayInSec:6);
                var pageSource = await browser.GetPageSourceAsync();
                if(!string.IsNullOrEmpty(pageSource) && pageSource.Contains("class=\"tabs-component__item\""))
                {
                    await browser.ExecuteScriptAsync("[...document.querySelectorAll('button[type=\"button\"]')].find(x=>x.textContent===\"stories\").click();", delayInSec:6);
                    pageSource = await browser.GetPageSourceAsync();
                    var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(pageSource, "li","class", "profile-media-list__item");
                    StoryCollection.Stories.Clear();
                    if(Nodes != null && Nodes.Count > 0)
                    {
                        foreach (var node in Nodes)
                        {
                            var mediaUrl = HtmlParseUtility.GetAttributeValueFromTagName(node.InnerHtml, "img", "class", "media-content__image", "src");
                            var videoUrl = HtmlParseUtility.GetAttributeValueFromTagName(node.InnerHtml, "a", "class", "button button--filled button__download", "href");
                            var date = HtmlParseUtility.GetAttributeValueFromTagName(node.InnerHtml, "p", "class", "media-content__meta-time", "title");
                            var time = DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss tt");
                            if (!string.IsNullOrEmpty(date))
                            {
                                var splitted = date.Split(',').ToList();
                                var datestr = splitted.LastOrDefault(x => !string.IsNullOrEmpty(x))?.Trim();
                                splitted = datestr.Split(':').ToList();
                                int.TryParse(splitted[0], out int hour);
                                int.TryParse(splitted[1], out int minute);
                                int.TryParse(splitted[2]?.Replace("AM","")?.Replace("PM",""), out int second);
                                time = DateTime.Now.AddHours(-hour).AddMinutes(-minute).AddSeconds(-second).ToString("dd:MM:yyyy hh:mm:ss tt");
                            }
                            if (!string.IsNullOrEmpty(mediaUrl))
                            {
                                StoryCollection.Stories.Add(new StoriesMedia
                                {
                                    StoryUrl = mediaUrl,
                                    VideoUrl = videoUrl,
                                    Username = StoryCollection.Username,
                                    DownloadStoryCommand = downloadMedia,
                                    Type = !string.IsNullOrEmpty(videoUrl) && videoUrl.EndsWith(".mp4") ? "video" : "image",
                                    StoryDate = time
                                });
                            }
                            
                        }
                    }
                    await browser.ExecuteScriptAsync("[...document.querySelectorAll('button[type=\"button\"]')].find(x=>x.textContent===\"highlights\").click();", delayInSec: 6);
                    pageSource = await browser.GetPageSourceAsync();
                    Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(pageSource, "button", "class", "highlight__button");
                    StoryCollection.Highlights.Clear();
                    if (Nodes != null && Nodes.Count > 0)
                    {
                        foreach (var node in Nodes)
                        {
                            var mediaUrl = HtmlParseUtility.GetAttributeValueFromTagName(node.InnerHtml, "img", "class", "highlight__image", "src");
                            var title = HtmlParseUtility.GetInnerTextFromTagName(node.InnerHtml, "p", "class", "highlight__title");
                            if (!string.IsNullOrEmpty(mediaUrl))
                            {
                                StoryCollection.Highlights.Add(new InstaHightlight
                                {
                                    CoverUrl = mediaUrl,
                                    Username = StoryCollection.Username,
                                    DownloadStoryCommand = DownloadHighlight,
                                    Title = title
                                });
                            }

                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                browser?.ClosedBrowser();
            }
            return StoryCollection;
        }


        public async Task<string> HitStoryOrHighlights(string API, string Username)
        {
            var FinalResponse = "";
            var client = new HttpClient();
        // Construct raw auth string
            string authRaw = $"-1::{Username}::LTE6Om11cmllbGdhbGxlOjpySlAydEJSS2Y2a3RiUnFQVUJ0UkU5a2xnQldiN2Q-";

            // Base64 encode
            string authEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authRaw));

            // Set headers
            var request = new HttpRequestMessage(HttpMethod.Post,API);
            request.Headers.Add("Host", "anonstories.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Ubuntu; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.6784.83 Safari/537.36");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("Origin", "https://anonstories.com");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://anonstories.com/");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

            // Set content
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("auth", authEncoded)
            });
            request.Content = formData;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            try
            {
                var response = await client.SendAsync(request);
                var stream1 = await response.Content.ReadAsStreamAsync();
                var responseText1 = HttpHelper.Decode(stream1, string.Join(",", response.Content.Headers.ContentEncoding));
                FinalResponse = responseText1?.Response;
            }
            catch (Exception)
            {
            }
            return FinalResponse;
        }
    }
}
