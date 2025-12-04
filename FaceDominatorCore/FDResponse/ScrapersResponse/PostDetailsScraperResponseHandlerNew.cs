using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class PostDetailsScraperResponseHandlerNew : FdResponseHandler
    {
        public FacebookAdsDetails PostDetails { get; set; }

        public string ComposerId { get; set; }

        public bool IsSuccess { get; set; }

        public PostDetailsScraperResponseHandlerNew(IResponseParameter responseParameter,
            FacebookAdsDetails objPostDetails, bool isVideoPost = false, bool isFetchCorrectDate = false)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    var postDate = string.Empty;

                    var decodedResonse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response, true);

                    if (string.IsNullOrEmpty(decodedResonse))
                        decodedResonse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                    if (!isFetchCorrectDate)
                        GetFacebookPostDetails(decodedResonse, objPostDetails, isVideoPost);
                    else
                    {
                        postDate = FdRegexUtility.FirstMatchExtractor(decodedResonse, FdConstants.DateTimeRegex);
                        objPostDetails.PostedDateTime = (!string.IsNullOrEmpty(postDate) && FdFunctions.IsIntegerOnly(postDate)) ? int.Parse(postDate).EpochToDateTimeLocal() : DateTime.Now;
                    }

                    PostDetails = objPostDetails;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public bool GetFacebookPostDetails(string postResponse, FacebookPostDetails objPostDetails, bool isVideoPost = false)
        {

            var adId = string.Empty;

            var adTitle = string.Empty;

            var ownerId = string.Empty;

            var ownerLogoUrl = string.Empty;

            var adSubDescription = string.Empty;

            var adMediaUrl = string.Empty;

            var adOtherMediaUrl = string.Empty;

            var adMediaType = string.Empty;

            var adNavigationUrl = string.Empty;

            var ownerName = string.Empty;

            var postDetailsFullText = string.Empty;

            var commentCount = "0";

            var shareCount = "0";

            var reactionCount = "0";

            var adFullUrl = string.Empty;

            adFullUrl = FdConstants.FbHomeUrl + FdRegexUtility.FirstMatchExtractor(postResponse, "URL=/(.*?)\"");


            commentCount = !FdFunctions.IsIntegerOnly(objPostDetails.CommentorCount) || objPostDetails.CommentorCount == "0"
                ? FdRegexUtility.FirstMatchExtractor(postResponse, "comment_count:{total_count:(.*?)}")
                : objPostDetails.CommentorCount;

            shareCount = !FdFunctions.IsIntegerOnly(objPostDetails.SharerCount) || objPostDetails.SharerCount == "0" ?
                FdRegexUtility.FirstMatchExtractor(postResponse, "share_count:{count:(.*?)}") : objPostDetails.SharerCount;

            reactionCount = !FdFunctions.IsIntegerOnly(objPostDetails.LikersCount) || objPostDetails.LikersCount == "0" ?
                FdRegexUtility.FirstMatchExtractor(postResponse, "reaction_count:{count:(.*?)}") : objPostDetails.LikersCount;

            var adPostedDateTime = Regex.Split(postResponse, "data-utime=").LastOrDefault();

            adPostedDateTime = FdRegexUtility.FirstMatchExtractor(adPostedDateTime, "\"(.*?)\"");

            if (!string.IsNullOrEmpty(objPostDetails.Title) && objPostDetails.Title != string.Empty)
                adTitle = objPostDetails.Title;


            if (!string.IsNullOrEmpty(objPostDetails.SubDescription) && objPostDetails.SubDescription != string.Empty)
                adSubDescription = objPostDetails.SubDescription;

            try
            {

                var postRegionData = Utilities.GetBetween("<" + postResponse + ">", "</abbr>", "<input");

                if (string.IsNullOrEmpty(postRegionData))
                    postRegionData = Utilities.GetBetween(postResponse, "_wp7\">", "Remove</button>");



                if (string.IsNullOrEmpty(adId))
                    adId = Utilities.GetBetween(postResponse, "name=\"ft_ent_identifier\" value=\"", "\"");

                string postDetailsNew = postResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);


                HtmlDocument objHtmlDocumentPostDetails = new HtmlDocument();

                objHtmlDocumentPostDetails.LoadHtml(postDetailsNew);


                HtmlDocument objHtmlDocumentTemp = new HtmlDocument();

                #region Scrape the Main Descriptions

                var mainDescription = Utilities.GetBetween(postResponse, "tn&quot;:&quot;K&quot;&#125;\">",
                    "tn&quot;:&quot;H&quot;&#125");

                if (!string.IsNullOrEmpty(mainDescription))
                {

                    mainDescription = Regex.Replace(mainDescription, "<br /> ", "\r\n");

                    mainDescription = Regex.Replace(mainDescription + ">", "<.*?>", string.Empty).Replace("\\-", "-");

                    mainDescription = WebUtility.HtmlDecode(mainDescription);
                }

                #endregion

                #region Scrape Ads Titles

                try
                {

                    var adTitleDescDetails = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[@class=\"_6m3 _--6\"]");

                    if (adTitleDescDetails == null)
                    {
                        if (postDetailsNew.Contains("_d5z lfloat _ohe"))
                            adTitleDescDetails = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[@class=\"_d5z lfloat _ohe\"]");
                        else if (postDetailsNew.Contains("_3ekx _29_4"))
                            adTitleDescDetails = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[@class=\"_3ekx _29_4\"]");

                    }

                    if (adTitleDescDetails != null)
                    {
                        objHtmlDocumentTemp.LoadHtml(adTitleDescDetails[0].InnerHtml);

                        var adTitleDetails = (objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"mbs _6m6 _2cnj _5s6c\"]") ??
                                              objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_-iv\"]")) ??
                                             objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_275z _5s6c\"]") ??
                                             objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_275z _6m6 _2cnj _5s6c\"]");

                        if (adTitleDetails != null)
                        {
                            adTitle = adTitleDetails[0].InnerText;
                            adTitle = WebUtility.HtmlDecode(adTitle);
                        }

                        var adSubDescriptionDetails = (objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_6m7 _3bt9\"]") ??
                                                       objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_-iw\"]")) ??
                                                      objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_5q4r\"]");

                        if (adSubDescriptionDetails != null)
                        {
                            adSubDescription = adSubDescriptionDetails[0].InnerText;
                            adSubDescription = WebUtility.HtmlDecode(adSubDescription);
                        }
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }


                var postTitleData = Utilities.GetBetween("<" + postResponse + ">", "</abbr>", "<input");
                var splitPostTitle = Regex.Split(postTitleData, "data-lynx-mode");

                foreach (var postTitle in splitPostTitle)
                {
                    if (postTitle.Contains("_6m7 _3bt9") || postTitle.Contains("_59tj _2iau"))
                    {
                        //AdTitle
                        #region AdTitle

                        if (string.IsNullOrEmpty(adTitle))
                        {

                            if (string.IsNullOrEmpty(adTitle))
                                adTitle = Utilities.GetBetween(postTitle, ">", "<");
                            if (string.IsNullOrEmpty(adTitle))
                                adTitle = Utilities.GetBetween(postTitle, "target=\"_blank\">", "<");

                            if (string.IsNullOrEmpty(adTitle))
                                adTitle = Utilities.GetBetween(postTitle, "_275z _5s6c\">", "<");
                        }

                        #endregion

                        //Scrape the Ad Subdescription
                        #region Scrape the Ad Subdescription

                        if (String.IsNullOrEmpty(adSubDescription))
                        {
                            adSubDescription = Utilities.GetBetween(postTitle, "_6m7 _3bt9\">", "<");

                            if (string.IsNullOrEmpty(adSubDescription))
                                adSubDescription = Utilities.GetBetween(postTitle, "_5q4r\">", "<");

                            adSubDescription = WebUtility.HtmlDecode(adSubDescription);
                        }

                        #endregion

                        //Scrape the Navigation Url
                        #region Scrape the Navigation Url

                        adNavigationUrl = Utilities.GetBetween(postTitle, "php?u=", "\"");

                        adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);

                        adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);

                        adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");

                        #endregion

                    }


                    if (postTitle.Contains("_6lz _6mb ellipsis") || postTitle.Contains("_275y _42ef"))
                    {
                        try
                        {
                            //Scrape the web url
                            #region Scrape the web url

                            var adWebUrl = Utilities.GetBetween(postTitle, "_6lz _6mb ellipsis\">", "<");

                            if (string.IsNullOrEmpty(adWebUrl))
                                adWebUrl = Utilities.GetBetween(postTitle, "_275y _42ef\">", "<");

                            if (string.IsNullOrEmpty(adWebUrl) && !string.IsNullOrEmpty(adNavigationUrl))
                                adWebUrl = Utilities.GetBetween(adNavigationUrl, "", "?");

                            if (!CheckUrlValid(adNavigationUrl) && !string.IsNullOrEmpty(adWebUrl))
                                adNavigationUrl = adWebUrl;

                            #endregion
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    adTitle = WebUtility.HtmlDecode(adTitle);
                }

                #endregion

                #region Scrape Media Files



                //Scrape Ad Image

                #region Scrape Ad Media Image

                if (string.IsNullOrEmpty(adMediaUrl))
                {
                    string postDetailsNew2;

                    try
                    {
                        postDetailsNew2 = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("(//div[@class=\"_3x-2\"])")[0].InnerHtml;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        postDetailsNew2 = postResponse;
                    }

                    string[] split;


                    if (string.IsNullOrEmpty(adMediaUrl))
                    {
                        split = Regex.Split(postDetailsNew2, "data-plsi=").Skip(1).ToArray();

                        if (split.Length > 0)
                        {
                            adMediaUrl = Utilities.GetBetween(split[0], "\"", "\"");
                            adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
                        }

                        if (split.Length > 1)
                        {
                            var sourceUrl = string.Empty;
                            foreach (var mediaElement in split)
                            {
                                if (string.IsNullOrEmpty(adOtherMediaUrl))
                                {
                                    sourceUrl = Utilities.GetBetween(mediaElement, "\"", "\"");
                                    sourceUrl = WebUtility.HtmlDecode(sourceUrl);
                                    Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                    adOtherMediaUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
                                }
                                else
                                {
                                    sourceUrl = Utilities.GetBetween(mediaElement, "\"", "\"");
                                    sourceUrl = WebUtility.HtmlDecode(sourceUrl);
                                    Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                    adOtherMediaUrl = adOtherMediaUrl + "||," + Convert.ToBase64String(_byte, 0, _byte.Length);
                                }

                            }
                        }
                    }

                    if (string.IsNullOrEmpty(adMediaUrl))
                    {
                        split = Regex.Split(postDetailsNew2, "data-ploi=").Skip(1).ToArray();

                        if (split.Length > 0)
                        {
                            adMediaUrl = Utilities.GetBetween(split[0], "\"", "\"");
                            adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
                        }

                        if (split.Length > 1)
                        {
                            var firstMediaUrlSet = false;

                            foreach (var eachString in split)
                            {
                                if (eachString.Contains("</video>"))
                                    continue;

                                var sourceUrl = Utilities.GetBetween(eachString, "src=\"", "\"");
                                sourceUrl = FdFunctions.GetDecodedResponse(sourceUrl);
                                sourceUrl = WebUtility.HtmlDecode(sourceUrl);

                                if (!firstMediaUrlSet)
                                {
                                    firstMediaUrlSet = true;
                                    if (string.IsNullOrEmpty(adMediaUrl))
                                    {
                                        adMediaUrl = sourceUrl;
                                    }
                                }
                                if (string.IsNullOrEmpty(adOtherMediaUrl))
                                {
                                    Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                    sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
                                    adOtherMediaUrl = sourceUrl;

                                }
                                else
                                {
                                    Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                    sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
                                    adOtherMediaUrl = adOtherMediaUrl + "||," + sourceUrl;
                                }
                            }
                        }

                    }


                    split = Regex.Split(postDetailsNew2, "scaledImageFitWidth").Skip(1).ToArray();

                    if (split.Length == 0)
                    {
                        split = Regex.Split(postDetailsNew2, "scaledImageFitHeight").Skip(1).ToArray();
                    }

                    if (split.Length > 0 && string.IsNullOrEmpty(adMediaUrl))
                    {
                        adMediaUrl = Utilities.GetBetween(split[0], "src=\"", "\"");

                        adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
                    }


                    if (string.IsNullOrEmpty(adMediaUrl))
                    {

                        if (postDetailsNew2.Contains("uiList _22st _509- _4ki"))
                        {
                            var data = Regex.Split(postDetailsNew2, "uiList _22st _509- _4ki").Skip(1).ToArray();
                            split = Regex.Split(data[0], "_5ya").Skip(1).ToArray();
                        }


                        if (split.Length > 0)
                        {
                            split = Regex.Split(postDetailsNew2, "_kvn img").Skip(1).ToArray();
                        }
                        if (split.Length > 0)
                        {
                            var firstMediaUrlSet = false;

                            foreach (var imagePath in split)
                            {
                                try
                                {

                                    if (imagePath.Contains("</video>"))
                                        continue;

                                    var sourceUrl = Utilities.GetBetween(imagePath, "src=\"", "\"");
                                    sourceUrl = FdFunctions.GetDecodedResponse(sourceUrl);
                                    sourceUrl = WebUtility.HtmlDecode(sourceUrl);

                                    if (!firstMediaUrlSet)
                                    {
                                        firstMediaUrlSet = true;
                                        if (string.IsNullOrEmpty(adMediaUrl))
                                            adMediaUrl = sourceUrl;

                                    }

                                    if (split.Length > 1)
                                    {
                                        if (string.IsNullOrEmpty(adOtherMediaUrl))
                                        {
                                            Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                            sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
                                            adOtherMediaUrl = sourceUrl;
                                        }
                                        else
                                        {
                                            Byte[] _byte = MediaUtilites.GetImageBytesFromUrl(sourceUrl);
                                            sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
                                            adOtherMediaUrl = adOtherMediaUrl + "||," + sourceUrl;
                                        }
                                    }

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.StackTrace);
                                }
                            }

                            if (string.IsNullOrEmpty(adTitle))
                            {
                                adTitle = Utilities.GetBetween(postDetailsNew2, "_1032\">", "<");
                                adTitle = WebUtility.HtmlDecode(adTitle);
                            }

                            if (string.IsNullOrEmpty(adSubDescription))
                            {
                                adSubDescription = Utilities.GetBetween(postDetailsNew2, "_1m-h\">", "<");
                                adSubDescription = WebUtility.HtmlDecode(adSubDescription);
                            }

                            if (string.IsNullOrEmpty(adNavigationUrl))
                            {

                                if (!string.IsNullOrEmpty(postRegionData))
                                {
                                    adNavigationUrl = Utilities.GetBetween(postDetailsNew2, "php?u=", "\"");
                                    adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);
                                    adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);
                                    adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");
                                }

                                if (string.IsNullOrEmpty(adNavigationUrl))
                                {
                                    adNavigationUrl = Utilities.GetBetween(postDetailsNew2, "php?u=", "\"");
                                    adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);
                                    adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);
                                    adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");
                                }
                                if (!CheckUrlValid(adNavigationUrl))
                                    adNavigationUrl = "";

                            }
                        }
                    }
                    adMediaType = !string.IsNullOrEmpty(adMediaUrl) ? "IMAGE" : string.Empty;

                    if (adMediaType == "IMAGE")
                        objPostDetails.AdMediaType = AdMediaType.IMAGE;
                }

                #endregion


                //Scrape the video url

                #region Scrape the video url


                if (string.IsNullOrEmpty(adMediaUrl))
                {
                    int count = 1;

                    int videoCount = 0;

                    if (postDetailsNew.Contains("uiList _22st _509- _4ki"))
                    {
                        var adVideoDetails = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//li[@class=\"_5ya\"]");

                        if (adVideoDetails != null)
                            count = adVideoDetails.Count;

                    }

                    var splitMediaUrl = Regex.Split(postResponse, "sd_src_no_ratelimit\":\"").Skip(1).ToArray();

                    if (splitMediaUrl.Length == 0)
                        splitMediaUrl = Regex.Split(postResponse, "sd_src_no_ratelimit:\"").Skip(1).ToArray();

                    if (splitMediaUrl.Length > 1)
                    {

                        var firstMediaUrlSet = false;

                        foreach (var splitdata in splitMediaUrl)
                        {
                            try
                            {

                                var sourceUrl = Utilities.GetBetween(splitdata, "", "\"");
                                sourceUrl = FdFunctions.GetDecodedResponse(sourceUrl);
                                if (!firstMediaUrlSet)
                                {
                                    firstMediaUrlSet = true;
                                    if (string.IsNullOrEmpty(adMediaUrl))
                                        adMediaUrl = sourceUrl;
                                }

                                if (videoCount >= count)
                                    break;

                                if (splitMediaUrl.Length > 1 && count > 1)
                                {
                                    adOtherMediaUrl = string.IsNullOrEmpty(adOtherMediaUrl)
                                        ? sourceUrl
                                        : adOtherMediaUrl + "||," + sourceUrl;
                                }

                                videoCount++;

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        adMediaUrl = Utilities.GetBetween(postResponse, "sd_src\":\"", "\"");

                        if (string.IsNullOrEmpty(adMediaUrl))
                            adMediaUrl = Utilities.GetBetween(postResponse, "sd_src:\"", "\"");

                        adMediaUrl = FdFunctions.GetDecodedResponse(adMediaUrl);
                    }

                    adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);

                    if (!string.IsNullOrEmpty(adMediaUrl))
                    {
                        adMediaType = "VIDEO";

                        objPostDetails.AdMediaType = AdMediaType.VIDEO;
                    }
                }
                #endregion



                #endregion


                if (adNavigationUrl.Contains("http://") || adNavigationUrl.Contains("https://"))
                {
                    string httpParameter = "https://";
                    adNavigationUrl += " ";
                    string destinationUrl = Utilities.GetBetween(adNavigationUrl, httpParameter, " ");

                    if (string.IsNullOrEmpty(destinationUrl))
                    {
                        httpParameter = "http://";
                        adNavigationUrl += " ";
                        destinationUrl = Utilities.GetBetween(adNavigationUrl, httpParameter, " ");
                    }

                    if (!string.IsNullOrEmpty(destinationUrl))
                    {
                        destinationUrl = httpParameter + destinationUrl;
                        adNavigationUrl = destinationUrl;
                    }
                }

                try
                {

                    var objCollection = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//span[@class=\"fwb fcg\"]") ??
                                        objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//span[@class=\"fwb\"]");

                    var postOwnerName = objCollection[0].InnerText;

                    var postOwnerId = objCollection[0].OuterHtml;

                    postOwnerId = Utilities.GetBetween(postOwnerId, postOwnerId.Contains("member_id") ? "member_id=" : "page.php?id=", "&");

                    if (string.IsNullOrEmpty(postOwnerId))
                        postOwnerId = Utilities.GetBetween(postDetailsNew, "entity_id\":\"", "\"");

                    ownerName = postOwnerName;
                    ownerId = postOwnerId;


                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                if (adTitle == "Facebook Creator")
                    adTitle = string.Empty;

                try
                {

                    postDetailsFullText = WebUtility.HtmlDecode(postDetailsFullText);

                    if (string.IsNullOrEmpty(postDetailsFullText))
                        postDetailsFullText = postResponse;

                    if (!postDetailsFullText.Contains(adSubDescription))
                        adSubDescription = string.Empty;
                    if (!postDetailsFullText.Contains(adTitle))
                        adTitle = string.Empty;

                    if (string.IsNullOrEmpty(adTitle) && adMediaType == "VIDEO")
                    {
                        try
                        {

                            var body = FdFunctions.GetDecodedResponse(postResponse);
                            objHtmlDocumentTemp.LoadHtml(body);
                            var postTitleDetails = objHtmlDocumentTemp.DocumentNode.SelectNodes("//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _5uun _4-u8\"]");

                            if (postTitleDetails != null)
                            {
                                if (postTitleDetails[0].InnerHtml.Contains(adId))
                                    adTitle = FdRegexUtility.FirstMatchExtractor(postTitleDetails[0].InnerHtml, FdConstants.AdTitleRegx).Replace(",", string.Empty).Trim();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }

                    if (!postDetailsFullText.Contains(mainDescription))
                    {
                        var splitDescription = Regex.Split(mainDescription, "\r\n");

                        var isCorrectDesc = true;

                        foreach (string desc in splitDescription)
                        {
                            if (!postDetailsFullText.Contains(desc))
                            {
                                isCorrectDesc = false;
                                break;
                            }
                        }

                        if (!isCorrectDesc)
                        {
                            var maindescriptionDetails = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[@class=\"_5pbx userContent _3576\"]");
                            if (maindescriptionDetails != null)
                            {
                                mainDescription = maindescriptionDetails[0].InnerText;
                                mainDescription = WebUtility.HtmlDecode(mainDescription);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                //mtm _5pco _2zpv

                if (string.IsNullOrEmpty(mainDescription))
                {
                    var description = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[starts-with(@class,'mtm _5pco')]");
                    if (description != null)
                    {
                        mainDescription = description[0].InnerHtml;
                        mainDescription = mainDescription.Replace("<p>", string.Empty);
                        mainDescription = mainDescription.Replace("</p>", "\r\n");
                        mainDescription = mainDescription.Replace("<br>", "\r\n");
                        objHtmlDocumentTemp.LoadHtml(mainDescription);
                        mainDescription = objHtmlDocumentTemp.DocumentNode.InnerText;
                    }
                }

                if (string.IsNullOrEmpty(mainDescription))
                {
                    var description = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[starts-with(@class, '_5pbx userContent')]");
                    if (description != null)
                    {
                        mainDescription = description[0].InnerText;
                    }
                }
                if (string.IsNullOrEmpty(mainDescription))
                {
                    var description = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[starts-with(@class, '_5qxm _5qxn _620g')]");
                    if (description != null)
                    {
                        var decodedDescription = description[0].InnerText;
                        description = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//div[starts-with(@class, '_4a6n _a5_')]");

                        mainDescription = description != null ? description[0].InnerText : decodedDescription;
                    }
                }

                if (postDetailsNew.Contains("aria-label=\"Map attachment\""))
                {
                    var description = objHtmlDocumentPostDetails.DocumentNode.SelectNodes("//img[starts-with(@class, '_a3f img')]");
                    if (description != null)
                    {
                        adMediaUrl = description[0].OuterHtml;
                        adMediaUrl = Utilities.GetBetween(adMediaUrl, "src=\"", "\"");

                        if (!string.IsNullOrEmpty(adMediaUrl))
                            objPostDetails.AdMediaType = AdMediaType.IMAGE;
                    }
                }

                if (postDetailsNew.Contains("TahoeController") && string.IsNullOrEmpty(adMediaUrl))
                {
                    objPostDetails.AdMediaType = AdMediaType.VIDEO;
                }
                else if (string.IsNullOrEmpty(adMediaUrl))
                {
                    objPostDetails.AdMediaType = AdMediaType.NoMedia;
                }

                var title = Utilities.GetBetween(postResponse, "data-ad-preview=\"headline\">", "<");

                if (string.IsNullOrEmpty(adTitle) ||
                    adTitle == "NA")
                {
                    adTitle = title;
                }

                objPostDetails.MediaUrl = !string.IsNullOrEmpty(adMediaUrl) ? adMediaUrl : objPostDetails.MediaUrl;
                objPostDetails.OtherMediaUrl = !string.IsNullOrEmpty(adOtherMediaUrl) ? adOtherMediaUrl : objPostDetails.OtherMediaUrl;
                objPostDetails.Title = !string.IsNullOrEmpty(adTitle) && adTitle != "Instagram" ? adTitle : string.Empty;
                objPostDetails.SubDescription = !string.IsNullOrEmpty(adSubDescription) ? adSubDescription : string.Empty;

                if (string.IsNullOrEmpty(objPostDetails.OwnerId) && !string.IsNullOrEmpty(ownerId))
                {
                    objPostDetails.OwnerName = !string.IsNullOrEmpty(ownerName) ? ownerName : string.Empty;
                    objPostDetails.OwnerId = !string.IsNullOrEmpty(ownerId) ? ownerId : string.Empty;
                    objPostDetails.OwnerLogoUrl = !string.IsNullOrEmpty(ownerLogoUrl) ? ownerLogoUrl : string.Empty;
                }

                if (!isVideoPost)
                {
                    objPostDetails.FullPostUrl = adFullUrl;
                    objPostDetails.Caption = !string.IsNullOrEmpty(mainDescription) ? mainDescription : string.Empty;
                    objPostDetails.NavigationUrl = !string.IsNullOrEmpty(adNavigationUrl) ? adNavigationUrl : objPostDetails.PostUrl;
                    objPostDetails.PostedDateTime = (!string.IsNullOrEmpty(adPostedDateTime) && FdFunctions.IsIntegerOnly(adPostedDateTime)) ? Int32.Parse(adPostedDateTime).EpochToDateTimeLocal() : DateTime.Now;
                    objPostDetails.LikersCount = !string.IsNullOrEmpty(reactionCount) && FdFunctions.IsIntegerOnly(reactionCount) ? reactionCount : "0";
                    objPostDetails.CommentorCount = !string.IsNullOrEmpty(commentCount) && FdFunctions.IsIntegerOnly(commentCount) ? commentCount : "0";
                    objPostDetails.SharerCount = !string.IsNullOrEmpty(shareCount) && FdFunctions.IsIntegerOnly(shareCount) ? shareCount : "0";
                }

                IsSuccess = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

        public bool CheckUrlValid(string source)
        {
            Uri uriResult;

            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

    }
}


