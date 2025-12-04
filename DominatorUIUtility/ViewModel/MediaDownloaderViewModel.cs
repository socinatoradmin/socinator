using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace DominatorUIUtility.ViewModel
{
    public class MediaDownloaderViewModel : BindableBase, IMediaDownloaderViewModel
    {
        #region Properties.

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }
        public ICommand DownloadMedia { get; set; }
        private FetchMedia fetcher { get; set; }
        public ICommand SearchMedia { get; set; }
        private bool _IsCheckedAutoDownload;
        public bool IsCheckedAutoDownload
        {
            get { return _IsCheckedAutoDownload; }
            set { SetProperty(ref _IsCheckedAutoDownload, value); }
        }
        private ObservableCollection<MediaInfoModel> _MediaCollection = new ObservableCollection<MediaInfoModel>();
        public ObservableCollection<MediaInfoModel> MediaCollection
        {
            get { return _MediaCollection; }
            set { SetProperty(ref _MediaCollection, value); }
        }
        private string _SearchUrl;
        public string SearchUrl
        {
            get { return _SearchUrl; }
            set { SetProperty(ref _SearchUrl, value); }
        }
        #endregion

        #region Constructor.
        public MediaDownloaderViewModel()
        {
            fetcher = new FetchMedia();
            DownloadMedia = new DelegateCommand<object>(DownloadMediaExecute, sender => true);
            SearchMedia = new DelegateCommand<object>(SearchMediaExecute, sender => true);
        }

        #endregion

        #region Methods.
        private async void SearchMediaExecute(object obj)
        {
            try
            {
                IsLoading = true;
                if (string.IsNullOrWhiteSpace(SearchUrl))
                    SearchUrl = obj as string;
                var platform = await GetPlatformByUrl(SearchUrl);
                if (string.IsNullOrEmpty(platform))
                    return;
                var hash = GenerateHash(SearchUrl);
                //var apiUrl = $"https://in5s.net/mates/en/analyze/ajax?retry=undefined&platform={platform}&mhash={hash}";
                var apiUrl = $"https://maxvideodownloader.net/api/down_options.php?video={WebUtility.UrlEncode(SearchUrl)}";
                var response = await fetcher.GetMediaInfo(apiUrl, SearchUrl);
                var mediaCollectionResponse = new MediaResponseHandler(response, platform);
                if (mediaCollectionResponse != null && mediaCollectionResponse.Medias.Count > 0)
                {
                    foreach (var media in mediaCollectionResponse.Medias)
                    {
                        await ThreadFactory.Instance.Start(() =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MediaCollection.Add(media);
                                if (IsCheckedAutoDownload)
                                {
                                    DownloadMediaExecute(media);
                                }
                            });
                        });
                    }
                }
                obj = SearchUrl = string.Empty;
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }
        public string GenerateHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16).ToLower();
            }
        }
        private async Task<string> GetPlatformByUrl(string searchUrl)
        {
            if (string.IsNullOrEmpty(searchUrl))
                return string.Empty;
            if (searchUrl.Contains(".instagram.com"))
                return "instagram";
            if (searchUrl.Contains("x.com"))
                return "twitter";
            if (searchUrl.Contains(".facebook.com"))
                return "facebook";
            if (searchUrl.Contains(".tiktok.com"))
                return "tiktok";
            return string.Empty;
        }
        private async void DownloadMediaExecute(object obj)
        {
            try
            {
                var model = obj as MediaInfoModel;
                if (model != null)
                {
                    var url = model.IsVideo ? model.VideoUrl : model.MediaUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        var httpClient = new HttpClient();
                        var extension = model.IsVideo ? "mp4" : "jpeg";
                        var path = ConstantVariable.GetDownloadMediaPath();
                        var outputPath = Path.Combine(path, model?.Platform?.ToUpperInvariant() + $" - {DateTime.Now.Ticks}.{extension}");
                        var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                    "Media",
                                    "Download", $"{model.Platform} Download", $"Successfully Downloaded At ==> {outputPath}");
                    }
                }
            }
            catch { }
        }

        #endregion
    }
    public interface IMediaDownloaderViewModel
    {

    }
    public class MediaInfoModel : BindableBase
    {
        public bool IsVideo { get; set; }
        public string Platform { get; set; }
        private string _MediaUrl;
        public string MediaUrl
        {
            get { return _MediaUrl; }
            set { SetProperty(ref _MediaUrl, value); }
        }
        private string _Username;
        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
        }
        private string _MediaTitle;
        public string MediaTitle
        {
            get { return _MediaTitle; }
            set { SetProperty(ref _MediaTitle, value); }
        }
        private string _videoUrl;
        public string VideoUrl
        {
            get { return _videoUrl; }
            set { SetProperty(ref _videoUrl, value); }
        }
    }
    public class MediaResponseHandler
    {
        private JsonHandler handler { get; set; } = JsonHandler.GetInstance;
        public List<MediaInfoModel> Medias { get; set; } = new List<MediaInfoModel>();
        public bool Success { get; set; }
        public MediaResponseHandler(string response, string platform = "")
        {
            try
            {
                if (string.IsNullOrEmpty(platform))
                    return;
                var obj = handler.ParseJsonToJsonObject(response);
                this.Success = handler.GetJTokenValue(obj, "status")?.ToLower() == "success";
                var result = handler.GetJTokenValue(obj, "result");
                if (!string.IsNullOrEmpty(result))
                {
                    var nodes = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(result, "div", "class", "download-content");
                    nodes = nodes is null || nodes.Count == 0 ? HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(result, "div", "class", "download-box") : nodes;
                    nodes = nodes is null || nodes.Count == 0 ? HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(result, "div", "class", "card text-center") : nodes;
                    if (nodes != null && nodes.Count > 0)
                    {
                        foreach (var node in nodes)
                        {
                            if (platform == "instagram")
                            {
                                var mediaUrl = HtmlParseUtility.GetAttributeValueFromTagName(node, "img", "alt", "Preview", "data-src");
                                var videoUrl = HtmlParseUtility.GetAttributeValueFromTagName(node, "a", "download", "true", "href");
                                var text = HtmlParseUtility.GetInnerTextFromTagName(node, "a", "href", videoUrl);
                                var username = HtmlParseUtility.GetInnerTextFromTagName(node, "div", "class", "download-top");
                                if (!string.IsNullOrEmpty(mediaUrl) && !Medias.Any(x => x.MediaUrl == mediaUrl))
                                {
                                    Medias.Add(new MediaInfoModel
                                    {
                                        Username = username,
                                        MediaUrl = mediaUrl,
                                        VideoUrl = videoUrl,
                                        Platform = platform,
                                        MediaTitle = string.IsNullOrEmpty(text) ? "Download" : text,
                                        IsVideo = !string.IsNullOrEmpty(text) && text.ToLower().Contains("video")
                                    });
                                }
                            }
                            else if (platform == "tiktok")
                            {
                                var mediaUrl = Utilities.GetBetween(node, "src=\"", "\"");
                                var videoUrl = HtmlParseUtility.GetAttributeValueFromTagName(node, "a", "rel", "nofollow", "href");
                                var text = HtmlParseUtility.GetInnerTextFromTagName(node, "a", "href", videoUrl)?.Replace("\n", "")?.Replace(" ", "");
                                var username = HtmlParseUtility.GetInnerTextFromTagName(node, "div", "id", "myModalLabel")?.Replace("\n", "")?.Replace(" ", "");
                                if (!string.IsNullOrEmpty(mediaUrl) && !Medias.Any(x => x.MediaUrl == mediaUrl))
                                {
                                    Medias.Add(new MediaInfoModel
                                    {
                                        Username = username?.Trim(),
                                        MediaUrl = mediaUrl,
                                        Platform = platform,
                                        VideoUrl = videoUrl,
                                        MediaTitle = string.IsNullOrEmpty(text) ? "Download" : text,
                                        IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                                    });
                                }
                            }
                            else if (platform == "twitter")
                            {
                                var mediaUrl = HtmlParseUtility.GetAttributeValueFromTagName(node, "img", "class", "card-img-top", "src");
                                var videoNodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(node, "a", "class", "btn btn-block");
                                var videoUrl = videoNodes != null && videoNodes.Count > 0 ? videoNodes.FirstOrDefault().GetAttributeValue("href", "_blank") : HtmlParseUtility.GetAttributeValueFromTagName(node, "a", "rel", "nofollow", "href");
                                var text = HtmlParseUtility.GetInnerTextFromTagName(node, "a", "href", videoUrl)?.Replace("\n", "")?.Replace(" ", "");
                                var username = HtmlParseUtility.GetInnerTextFromTagName(node, "div", "class", "card-header")?.Replace("\n", "")?.Replace(" ", "");
                                if (!string.IsNullOrEmpty(mediaUrl) && !Medias.Any(x => x.MediaUrl == mediaUrl))
                                {
                                    Medias.Add(new MediaInfoModel
                                    {
                                        Username = username?.Trim(),
                                        MediaUrl = mediaUrl,
                                        VideoUrl = videoUrl,
                                        Platform = platform,
                                        MediaTitle = string.IsNullOrEmpty(text) ? "Download" : text,
                                        IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (platform == "tiktok")
                    {
                        var mediaUrl = handler.GetJTokenValue(obj, "thumbnail");
                        var username = handler.GetJTokenValue(obj, "unique_id");
                        var list = handler.GetJTokenOfJToken(obj, "medias");
                        var videoNode = list?.FirstOrDefault(x => handler.GetJTokenValue(x, "type") == "video");
                        var videoUrl = handler.GetJTokenValue(videoNode, "url");
                        var text = handler.GetJTokenValue(videoNode, "extension")?.ToUpper();
                        Medias.Add(new MediaInfoModel
                        {
                            Username = username.Trim(),
                            MediaUrl = mediaUrl,
                            VideoUrl = videoUrl,
                            Platform = platform,
                            MediaTitle = string.IsNullOrEmpty(text) ? "Download" : $"Download {text}",
                            IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                        });
                    }
                    else if (platform == "instagram")
                    {
                        var mediaUrl = handler.GetJTokenValue(obj, "thumbnail");
                        var username = handler.GetJTokenValue(obj, "owner", "username");
                        var list = handler.GetJTokenOfJToken(obj, "medias");
                        var videoNode = list?.FirstOrDefault(x => handler.GetJTokenValue(x, "type") == "video");
                        var videoUrl = handler.GetJTokenValue(videoNode, "url");
                        var text = handler.GetJTokenValue(videoNode, "extension")?.ToUpper();
                        Medias.Add(new MediaInfoModel
                        {
                            Username = username.Trim(),
                            MediaUrl = mediaUrl,
                            VideoUrl = videoUrl,
                            Platform = platform,
                            MediaTitle = string.IsNullOrEmpty(text) ? "Download" : $"Download {text}",
                            IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                        });
                    }
                    else if (platform == "twitter")
                    {
                        var mediaUrl = handler.GetJTokenValue(obj, "thumbnail");
                        var username = handler.GetJTokenValue(obj, "author");
                        var list = handler.GetJTokenOfJToken(obj, "medias");
                        var videoNode = list?.FirstOrDefault(x => handler.GetJTokenValue(x, "type") == "video");
                        var videoUrl = handler.GetJTokenValue(videoNode, "url");
                        var text = handler.GetJTokenValue(videoNode, "extension")?.ToUpper();
                        Medias.Add(new MediaInfoModel
                        {
                            Username = username.Trim(),
                            MediaUrl = mediaUrl,
                            VideoUrl = videoUrl,
                            Platform = platform,
                            MediaTitle = string.IsNullOrEmpty(text) ? "Download" : $"Download {text}",
                            IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                        });
                    }
                    else if (platform == "facebook")
                    {
                        var mediaUrl = handler.GetJTokenValue(obj, "thumbnail");
                        var username = handler.GetJTokenValue(obj, "title");
                        var list = handler.GetJTokenOfJToken(obj, "medias");
                        var videoNode = list?.FirstOrDefault(x => handler.GetJTokenValue(x, "type") == "video");
                        var videoUrl = handler.GetJTokenValue(videoNode, "url");
                        var text = handler.GetJTokenValue(videoNode, "extension")?.ToUpper();
                        Medias.Add(new MediaInfoModel
                        {
                            Username = username.Trim(),
                            MediaUrl = mediaUrl,
                            VideoUrl = videoUrl,
                            Platform = platform,
                            MediaTitle = string.IsNullOrEmpty(text) ? "Download" : $"Download {text}",
                            IsVideo = !string.IsNullOrEmpty(text) && text.Contains("MP4")
                        });
                    }
                }
            }
            catch { }
        }
    }
    public class FetchMedia
    {
        public string GenerateRandomSessionId(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();
            var rng = RandomNumberGenerator.Create();
            var data = new byte[1];
            while (sb.Length < length)
            {
                rng.GetBytes(data);
                char c = chars[data[0] % chars.Length];
                sb.Append(c);
            }
            return sb.ToString();
        }
        public async Task<string> GetMediaInfo(string APIUrl, string PostUrl)
        {
            var FinalResponse = "";
            //using (var client = new HttpClient())
            //{
            //    // Set headers
            //    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"macOS\"");
            //    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            //    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.6814.63 Safari/537.36");
            //    client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            //    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");
            //    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            //    client.DefaultRequestHeaders.Add("Origin", "https://in5s.net");
            //    client.DefaultRequestHeaders.Add("Referer", "https://in5s.net/en/");
            //    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            //    client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
            //    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");

            //    // Create request content
            //    var content = new FormUrlEncodedContent(new[]
            //    {
            //        new KeyValuePair<string, string>("url", PostUrl),
            //        new KeyValuePair<string, string>("ajax", "1"),
            //        new KeyValuePair<string, string>("lang", "en")
            //    });

            //    // Send request
            //    var response = await client.PostAsync(APIUrl, content);
            //    var responseBody = await response.Content.ReadAsStreamAsync();
            //    return HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding))?.Response;
            //}


            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            var client = new HttpClient(handler);

            // Generate random PHPSESSID
            string randomSessionId = GenerateRandomSessionId(26);

            // Set the cookie
            handler.CookieContainer.Add(new Uri("https://maxvideodownloader.net"), new Cookie("PHPSESSID", randomSessionId));

            // Add headers
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.6779.61 Safari/537.36");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Referrer = new Uri("https://maxvideodownloader.net/");

            try
            {
                HttpResponseMessage response = await client.GetAsync(APIUrl);
                var responseBody = await response.Content.ReadAsStreamAsync();
                FinalResponse = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding))?.Response;
            }
            catch (Exception)
            {
            }
            return FinalResponse;
        }
    }
}
