#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

#endregion

namespace DominatorHouseCore.Utility
{
    public class ImageExtracter
    {
        /// <summary>
        ///     Extract the images url from the url
        /// </summary>
        /// <param name="url">url for fetching image</param>
        /// <param name="isBackgroundImageNeed">pass true for fetching css, backgroud images need from the website</param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractImageUrls ( string url, ref string title,
            bool isBackgroundImageNeed = false )
        {
            var imageUrl = new List<string>();

            var scrapeUrl = new Uri(url);
            var host = scrapeUrl.Host;

            if (host.Contains("google.co.in"))
            {
                var objwebclient = new WebClient();

                objwebclient.Headers.Add("Host", "www.google.co.in");
                //  objwebclient.Headers.Add("User-Agent", " Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");                   
                objwebclient.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                objwebclient.Headers.Add("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                objwebclient.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                objwebclient.Headers.Add("Upgrade-Insecure-Requests", "1");
                ServicePointManager.ServerCertificateValidationCallback +=
                    ( sender, certificate, chain, sslPolicyErrors ) => true;

                var googlePageResult = objwebclient.DownloadString(scrapeUrl);

                googlePageResult = WebUtility.HtmlDecode(googlePageResult);

                title = Utilities.GetBetween(googlePageResult, "<title>", "- Google Search</title>");

                var images = Regex.Split(googlePageResult, "bRMDJf islir").Skip(1).ToArray();
                if (images.Length == 0)
                    images = Regex.Split(googlePageResult, "data-iurl=\"").Skip(1).ToArray();


                imageUrl = new List<string>();
                images.ForEach(x =>
                {
                    var image = Utilities.GetBetween(x, "data-src=\"", "\"");
                    image = Regex.Unescape(image);
                    if (!string.IsNullOrEmpty(image))
                        imageUrl.Add(image);
                });
            }
            else
            {
                // Create a request to getting response of given url
                var webClient = new WebClient();
                var pageResult = webClient.DownloadString(scrapeUrl);

                pageResult = WebUtility.HtmlDecode(pageResult);

                title = Utilities.GetBetween(pageResult, "<title>", "- Google Search</title>");

                var htmlDocument = new HtmlDocument
                {
                    OptionAutoCloseOnEnd = true,
                    OptionCheckSyntax = false,
                    OptionFixNestedTags = true
                };

                htmlDocument.LoadHtml(pageResult);

                // Select the nodes
                var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img[@src]");

                // Fetching Src values from response
                if (htmlNodeCollection != null)
                    imageUrl.AddRange(
                        RemoveInvalidUrls(htmlNodeCollection.Select(node => node.Attributes["src"].Value)));


                // Check if background images are needed from the website 
                if (!isBackgroundImageNeed)
                    return imageUrl;

                using (var enumerator = htmlDocument.DocumentNode.Descendants().Where(d =>
                {
                    // get the style image 
                    return d.Attributes.Contains("style") && d.Attributes["style"].Value.Contains("background:url");
                }).ToList().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        // Getting the background images
                        var input = enumerator.Current?.Attributes["style"].Value;
                        var regex = new Regex(".*?background:url\\('?(?<bgpath>.*)'?\\).*?",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                        if (input != null && regex.IsMatch(input))
                            imageUrl.Add(regex.Match(input).Groups["bgpath"].Value);
                    }
                }
            }

            //}

            return imageUrl;
        }


        public static IEnumerable<string> ExtractLinkDetails ( string url, ref string title, ref string description,
            bool isBackgroundImageNeed = false )
        {
            var imageUrl = new List<string>();

            try
            {
                if (!url.Contains("https://"))
                    url = "https://" + url;

                var scrapeUrl = new Uri(url);

                var webClient = new WebClient { Encoding = Encoding.UTF8 };
                var googlePageResult = webClient.DownloadString(scrapeUrl);

                googlePageResult = Regex.Replace(googlePageResult, "\\\\([^u])", "\\\\$1").Replace("\\", "");
                googlePageResult = WebUtility.HtmlDecode(googlePageResult);


                title = googlePageResult.Contains("\"og_title\"")
                    ? HtmlParseUtility.GetAttributeValueFromTagName(googlePageResult, "meta", "name"
                        , "og_title", "content")
                    : Utilities.GetBetween(googlePageResult, "<title>", "</title>");


                description = googlePageResult.Contains("\"og:description\"")
                    ? HtmlParseUtility.GetAttributeValueFromTagName(googlePageResult, "meta", "property"
                        , "og:description", "content")
                    : string.Empty;

                var image = googlePageResult.Contains("\"og:image\"")
                    ? HtmlParseUtility.GetAttributeValueFromTagName(googlePageResult, "meta", "property"
                        , "og:image", "content")
                    : string.Empty;

                var siteUrl = googlePageResult.Contains("\"og:url\"")
                    ? HtmlParseUtility.GetAttributeValueFromTagName(googlePageResult, "meta", "property"
                        , "og:url", "content")
                    : string.Empty;

                if (!image.Contains("https:") && !string.IsNullOrEmpty(image) && !image.StartsWith("//"))
                    image = siteUrl + image;

                if (string.IsNullOrEmpty(image))
                {
                    var matchCollection = Regex.Matches(googlePageResult, "href=\"(.*?)\"");
                    if (matchCollection.Count > 0)
                        foreach (Match match in matchCollection)
                        {
                            var imageData = match.Groups[1].ToString();
                            if (!imageData.Contains("png") || !imageData.ToLower().Contains("logo")) continue;
                            image = match.Groups[1].ToString();
                            break;
                        }
                }


                if (string.IsNullOrEmpty(image) && googlePageResult.Contains("\"logo\":"))
                {
                    var splitResponse = Regex.Split(googlePageResult, "\"logo\":").Skip(1).ToList();
                    foreach (var response in splitResponse)
                        try
                        {
                            var matchCollection = Regex.Matches(response, ":\"(.*?)\"");
                            if (matchCollection.Count > 0)
                                foreach (Match match in matchCollection)
                                {
                                    var imageData = match.Groups[1].ToString();
                                    if (!imageData.Contains("png")) continue;
                                    image = imageData;
                                    break;
                                }

                            if (!string.IsNullOrEmpty(image))
                                break;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }

                if (string.IsNullOrEmpty(image))
                {
                    var imageList = HtmlParseUtility.GetListInnerHtmlFromTagName(googlePageResult, "div", "class", "kCmkOe");
                    foreach (var img in imageList)
                    {
                        image = HtmlParseUtility.GetAttributeValueFromTagName(img, "img", "class", "DS1iW", "src");
                        imageUrl.Add(image);
                    }
                    return imageUrl;
                }

                if (!image.Contains("https:") && !string.IsNullOrEmpty(image) && image.StartsWith("//"))
                    image = "https:" + image;

                imageUrl.Add(image);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return imageUrl;
        }

        /// <summary>
        ///     Remove invalid image url from given collections
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static IEnumerable<string> RemoveInvalidUrls ( IEnumerable<string> urls )
        {
            var validUrls = new List<string>();

            // Iterate the collection of url to filter is valid or not
            urls.ToList().ForEach(x =>
            {
                // Check whether Valid Url or not
                if (IsValidUrl(x))
                    validUrls.Add(DecodeHtml(x));
            });
            return validUrls;
        }

        public static string DecodeHtml ( string text )
        {
            return HttpUtility.HtmlDecode(text);
        }

        /// <summary>
        ///     Check whether url in valid or not
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        public static bool IsValidUrl ( string sourceUrl )
        {
            // encode the url and get source url
            sourceUrl = HttpUtility.UrlPathEncode(sourceUrl);
            return CheckUrlValid(sourceUrl);
        }

        /// <summary>
        ///     Is give url is valid or not
        /// </summary>
        /// <param name="source">url</param>
        /// <returns></returns>
        public static bool CheckUrlValid ( string source )
        {
            // Check whether Url is based on http or https schema
            if (Uri.TryCreate(source, UriKind.Absolute, out var result))
                return (result.Scheme == Uri.UriSchemeHttp) | (result.Scheme == Uri.UriSchemeHttps);
            return false;
        }
    }
}