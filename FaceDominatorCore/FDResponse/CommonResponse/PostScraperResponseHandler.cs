using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class PostScraperResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool Status { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();


        public PostScraperResponseHandler(IResponseParameter responseParameter, FacebookPostDetails objPostDetails)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            var decodedResonse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

            try
            {
                if (string.IsNullOrEmpty(decodedResonse))
                    decodedResonse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                GetFacebookPostDetailsNewUI(decodedResonse, objPostDetails);

                ObjFdScraperResponseParameters.PostDetails = objPostDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #region Method for Old UI
        //public bool GetFacebookPostDetails(string postResponse, FacebookPostDetails objPostDetails)
        //{

        //    var adId = string.Empty;

        //    var adTitle = string.Empty;

        //    var ownerId = string.Empty;

        //    var ownerLogoUrl = string.Empty;

        //    var adSubDescription = string.Empty;

        //    var adMediaUrl = string.Empty;

        //    var adOtherMediaUrl = string.Empty;

        //    var adMediaType = string.Empty;

        //    var adNavigationUrl = string.Empty;

        //    var ownerName = string.Empty;

        //    FbEntityTypes entityType = FbEntityTypes.Friend;

        //    var postDetailsFullText = string.Empty;

        //    var scrapedUrl = string.Empty;

        //    var adPostedDateTime = Utilities.GetBetween(postResponse, "data-utime=\"", "\"");

        //    scrapedUrl = Utilities.GetBetween(postResponse, "URL=/", "?");

        //    if (!string.IsNullOrEmpty(objPostDetails.Title) && objPostDetails.Title != "NA")
        //        adTitle = objPostDetails.Title;

        //    if (!string.IsNullOrEmpty(objPostDetails.SubDescription) && objPostDetails.SubDescription != "NA")
        //        adSubDescription = objPostDetails.SubDescription;
        //    try
        //    {

        //        var postRegionData = Utilities.GetBetween("<" + postResponse + ">", "</abbr>", "<input");

        //        if (string.IsNullOrEmpty(postRegionData))
        //            postRegionData = Utilities.GetBetween(postResponse, "_wp7\">", "Remove</button>");


        //        if (string.IsNullOrEmpty(adId))
        //            adId = Utilities.GetBetween(postResponse, "name=\"ft_ent_identifier\" value=\"", "\"");

        //        HtmlDocument objHtmlDocumentNew = new HtmlDocument();

        //        string postDetailsNew;

        //        try
        //        {
        //            postDetailsNew = postResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);

        //            objHtmlDocumentNew.LoadHtml(postDetailsNew);
        //            //_4-u2 mbm _4mrt _5jmm _5pat _5v3q _4-u8
        //            var postDetailsCollection = objHtmlDocumentNew.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _7cqq _5uun _4-u8\"])") ??
        //                                        objHtmlDocumentNew.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _4-u8\"])");

        //            // some times it was throwing null reffrence exception
        //            if (postDetailsCollection != null)
        //                foreach (var post in postDetailsCollection)
        //                {
        //                    if (post.InnerHtml.Contains(adId))
        //                    {
        //                        postDetailsNew = post.InnerHtml;
        //                        postDetailsFullText = post.InnerText;
        //                        break;
        //                    }
        //                }
        //            else
        //            {
        //                postDetailsNew = postResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //            postDetailsNew = postResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
        //        }




        //        #region Scrape the Main Descriptions

        //        var mainDescription = Utilities.GetBetween(postResponse, "tn&quot;:&quot;K&quot;&#125;\">",
        //            "tn&quot;:&quot;H&quot;&#125");

        //        if (!string.IsNullOrEmpty(mainDescription))
        //        {
        //            mainDescription = Regex.Replace(mainDescription, "<br /> ", "\r\n");

        //            mainDescription = Regex.Replace(mainDescription + ">", "<.*?>", string.Empty).Replace("\\-", "-");

        //            mainDescription = WebUtility.HtmlDecode(mainDescription);
        //        }

        //        #endregion

        //        #region Scrape Ads Titles

        //        try
        //        {

        //            objHtmlDocumentNew.LoadHtml(postDetailsNew);

        //            var adTitleDescDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_6m3 _--6\"]");

        //            if (adTitleDescDetails == null)
        //            {
        //                if (postDetailsNew.Contains("_d5z lfloat _ohe"))
        //                    adTitleDescDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_d5z lfloat _ohe\"]");
        //                else if (postDetailsNew.Contains("_3ekx _29_4"))
        //                    adTitleDescDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_3ekx _29_4\"]");

        //            }

        //            if (adTitleDescDetails != null)
        //            {
        //                objHtmlDocumentNew.LoadHtml(adTitleDescDetails[0].InnerHtml);

        //                var adTitleDetails = (objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"mbs _6m6 _2cnj _5s6c\"]") ??
        //                                      objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_-iv\"]")) ??
        //                                     objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_275z _5s6c\"]");

        //                if (adTitleDetails != null)
        //                {
        //                    adTitle = adTitleDetails[0].InnerText;
        //                    adTitle = WebUtility.HtmlDecode(adTitle);
        //                }

        //                var adSubDescriptionDetails = (objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_6m7 _3bt9\"]") ??
        //                                               objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_-iw\"]")) ??
        //                                              objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_5q4r\"]");

        //                if (adSubDescriptionDetails != null)
        //                {
        //                    adSubDescription = adSubDescriptionDetails[0].InnerText;
        //                    adSubDescription = WebUtility.HtmlDecode(adSubDescription);
        //                }
        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }


        //        var postTitleData = Utilities.GetBetween("<" + postResponse + ">", "</abbr>", "<input");
        //        var splitPostTitle = Regex.Split(postTitleData, "data-lynx-mode");

        //        foreach (var postTitle in splitPostTitle)
        //        {
        //            if (postTitle.Contains("_6m7 _3bt9") || postTitle.Contains("_59tj _2iau"))
        //            {
        //                //AdTitle
        //                #region AdTitle

        //                if (string.IsNullOrEmpty(adTitle))
        //                {
        //                    if (string.IsNullOrEmpty(adTitle))
        //                        adTitle = Utilities.GetBetween(postTitle, ">", "<");

        //                    if (string.IsNullOrEmpty(adTitle))
        //                        adTitle = Utilities.GetBetween(postTitle, "target=\"_blank\">", "<");

        //                    if (string.IsNullOrEmpty(adTitle))
        //                        adTitle = Utilities.GetBetween(postTitle, "_275z _5s6c\">", "<");
        //                }

        //                #endregion

        //                //Scrape the Ad Subdescription
        //                #region Scrape the Ad Subdescription

        //                if (String.IsNullOrEmpty(adSubDescription))
        //                {
        //                    adSubDescription = Utilities.GetBetween(postTitle, "_6m7 _3bt9\">", "<");
        //                    if (string.IsNullOrEmpty(adSubDescription))
        //                        adSubDescription = Utilities.GetBetween(postTitle, "_5q4r\">", "<");

        //                    adSubDescription = WebUtility.HtmlDecode(adSubDescription);
        //                }

        //                #endregion

        //                //Scrape the Navigation Url
        //                #region Scrape the Navigation Url

        //                adNavigationUrl = Utilities.GetBetween(postTitle, "php?u=", "\"");

        //                adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);

        //                adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);

        //                adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");

        //                #endregion

        //            }


        //            if (postTitle.Contains("_6lz _6mb ellipsis") || postTitle.Contains("_275y _42ef"))
        //            {
        //                try
        //                {
        //                    //Scrape the web url
        //                    #region Scrape the web url

        //                    var adWebUrl = Utilities.GetBetween(postTitle, "_6lz _6mb ellipsis\">", "<");

        //                    if (string.IsNullOrEmpty(adWebUrl))
        //                        adWebUrl = Utilities.GetBetween(postTitle, "_275y _42ef\">", "<");

        //                    if (string.IsNullOrEmpty(adWebUrl) && !string.IsNullOrEmpty(adNavigationUrl))
        //                        adWebUrl = Utilities.GetBetween(adNavigationUrl, "", "?");

        //                    if (!CheckUrlValid(adNavigationUrl) && !string.IsNullOrEmpty(adWebUrl))
        //                        adNavigationUrl = adWebUrl;

        //                    #endregion
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e);
        //                }
        //            }
        //            adTitle = WebUtility.HtmlDecode(adTitle);
        //        }

        //        #endregion

        //        #region Scrape Media Files



        //        //Scrape Ad Image

        //        #region Scrape Ad Media Image

        //        if (string.IsNullOrEmpty(adMediaUrl))
        //        {
        //            HtmlDocument objHtmlDocumentNew2 = new HtmlDocument();

        //            string postDetailsNew2;

        //            try
        //            {
        //                postDetailsNew2 = postResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);

        //                objHtmlDocumentNew2.LoadHtml(postDetailsNew2);

        //                //postDetailsNew2 = objHtmlDocumentNew2.DocumentNode.SelectNodes("(//div[@class=\"_3x-2\"])")[0].InnerHtml;
        //                postDetailsNew2 =
        //                    objHtmlDocumentNew2.DocumentNode.SelectNodes("(//div[@class=\"_3x-2\"])") == null
        //                        ? postResponse
        //                        : objHtmlDocumentNew2.DocumentNode.SelectNodes("(//div[@class=\"_3x-2\"])")[0].InnerHtml;
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //                postDetailsNew2 = postResponse;
        //            }

        //            string[] split;




        //            if (string.IsNullOrEmpty(adMediaUrl))
        //            {
        //                split = Regex.Split(postDetailsNew2, "data-plsi=").Skip(1).ToArray();

        //                if (split.Length > 0)
        //                {
        //                    adMediaUrl = Utilities.GetBetween(split[0], "\"", "\"");
        //                    adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
        //                }

        //                if (split.Length > 1)
        //                {
        //                    foreach (var mediaElement in split)
        //                    {
        //                        if (string.IsNullOrEmpty(adOtherMediaUrl))
        //                        {
        //                            adOtherMediaUrl = Utilities.GetBetween(mediaElement, "\"", "\"");
        //                            adOtherMediaUrl = WebUtility.HtmlDecode(adOtherMediaUrl);
        //                        }
        //                        else
        //                        {
        //                            adOtherMediaUrl = adOtherMediaUrl + "||," + Utilities.GetBetween(mediaElement, "\"", "\"");
        //                            adOtherMediaUrl = WebUtility.HtmlDecode(adOtherMediaUrl);
        //                        }

        //                    }
        //                }
        //            }

        //            if (string.IsNullOrEmpty(adMediaUrl))
        //            {
        //                split = Regex.Split(postDetailsNew2, "data-ploi=").Skip(1).ToArray();

        //                if (split.Length > 0)
        //                {
        //                    adMediaUrl = Utilities.GetBetween(split[0], "\"", "\"");
        //                    adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
        //                }

        //                if (split.Length > 1)
        //                {
        //                    foreach (var eachString in split)
        //                    {
        //                        if (string.IsNullOrEmpty(adOtherMediaUrl))
        //                        {
        //                            adOtherMediaUrl = Utilities.GetBetween(eachString, "\"", "\"");
        //                            adOtherMediaUrl = WebUtility.HtmlDecode(adOtherMediaUrl);
        //                        }
        //                        else
        //                        {
        //                            adOtherMediaUrl = adOtherMediaUrl + "||," + Utilities.GetBetween(eachString, "\"", "\"");
        //                            adOtherMediaUrl = WebUtility.HtmlDecode(adOtherMediaUrl);
        //                        }
        //                    }
        //                }

        //            }


        //            split = Regex.Split(postDetailsNew2, "scaledImageFitWidth").Skip(1).ToArray();

        //            if (split.Length == 0)
        //                split = Regex.Split(postDetailsNew2, "scaledImageFitHeight").Skip(1).ToArray();

        //            if (split.Length > 0 && string.IsNullOrEmpty(adMediaUrl))
        //            {
        //                adMediaUrl = Utilities.GetBetween(split[0], "src=\"", "\"");

        //                adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);
        //            }


        //            if (string.IsNullOrEmpty(adMediaUrl))
        //            {

        //                if (postDetailsNew2.Contains("uiList _22st _509- _4ki"))
        //                {
        //                    var data = Regex.Split(postDetailsNew2, "uiList _22st _509- _4ki").Skip(1).ToArray();
        //                    split = Regex.Split(data[0], "_5ya").Skip(1).ToArray();
        //                }


        //                if (split.Length > 0)
        //                    split = Regex.Split(postDetailsNew2, "_kvn img").Skip(1).ToArray();

        //                if (split.Length > 0)
        //                {
        //                    var firstMediaUrlSet = false;

        //                    foreach (var imagePath in split)
        //                    {
        //                        try
        //                        {

        //                            if (imagePath.Contains("</video>"))
        //                                continue;

        //                            var sourceUrl = Utilities.GetBetween(imagePath, "src=\"", "\"");
        //                            sourceUrl = FdFunctions.GetDecodedResponse(sourceUrl);
        //                            sourceUrl = WebUtility.HtmlDecode(sourceUrl);

        //                            if (!firstMediaUrlSet)
        //                            {
        //                                firstMediaUrlSet = true;
        //                                if (string.IsNullOrEmpty(adMediaUrl))
        //                                {
        //                                    adMediaUrl = sourceUrl;
        //                                }
        //                            }

        //                            if (split.Length > 1)
        //                            {

        //                                adOtherMediaUrl = string.IsNullOrEmpty(adOtherMediaUrl)
        //                                    ? sourceUrl
        //                                    : adOtherMediaUrl + "||," + sourceUrl;
        //                                //if (string.IsNullOrEmpty(adOtherMediaUrl))
        //                                //{
        //                                //    adOtherMediaUrl = sourceUrl;
        //                                //}
        //                                //else
        //                                //{
        //                                //    adOtherMediaUrl = adOtherMediaUrl + "||," + sourceUrl;
        //                                //}
        //                            }

        //                        }
        //                        catch (Exception e)
        //                        {
        //                            Console.WriteLine(e.StackTrace);
        //                        }
        //                    }

        //                    if (string.IsNullOrEmpty(adTitle))
        //                    {
        //                        adTitle = Utilities.GetBetween(postDetailsNew2, "_1032\">", "<");
        //                        adTitle = WebUtility.HtmlDecode(adTitle);
        //                    }

        //                    if (string.IsNullOrEmpty(adSubDescription))
        //                    {
        //                        adSubDescription = Utilities.GetBetween(postDetailsNew2, "_1m-h\">", "<");
        //                        adSubDescription = WebUtility.HtmlDecode(adSubDescription);
        //                    }

        //                    if (string.IsNullOrEmpty(adNavigationUrl))
        //                    {

        //                        if (!string.IsNullOrEmpty(postRegionData))
        //                        {
        //                            adNavigationUrl = Utilities.GetBetween(postDetailsNew2, "php?u=", "\"");
        //                            adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);
        //                            adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);
        //                            adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");
        //                        }

        //                        if (string.IsNullOrEmpty(adNavigationUrl))
        //                        {
        //                            adNavigationUrl = Utilities.GetBetween(postDetailsNew2, "php?u=", "\"");
        //                            adNavigationUrl = Uri.UnescapeDataString(adNavigationUrl);
        //                            adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);
        //                            adNavigationUrl = Utilities.GetBetween(adNavigationUrl, "", "&h=");
        //                        }
        //                        if (!CheckUrlValid(adNavigationUrl))
        //                        {
        //                            adNavigationUrl = "";
        //                        }
        //                    }
        //                }
        //            }
        //            adMediaType = !string.IsNullOrEmpty(adMediaUrl) ? "IMAGE" : "NA";

        //            if (adMediaType == "IMAGE")
        //                objPostDetails.MediaType = MediaType.Image;
        //        }

        //        #endregion


        //        //Scrape the video url

        //        #region Scrape the video url


        //        if (string.IsNullOrEmpty(adMediaUrl))
        //        {
        //            int count = 1;

        //            int videoCount = 0;

        //            if (postDetailsNew.Contains("uiList _22st _509- _4ki"))
        //            {
        //                objHtmlDocumentNew.LoadHtml(postDetailsNew);

        //                var adVideoDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//li[@class=\"_5ya\"]");

        //                if (adVideoDetails != null)
        //                    count = adVideoDetails.Count;

        //            }

        //            //var splitMediaUrl = Regex.Split(postResponse, "hd_src:\"").Skip(1).ToArray();

        //            //if (splitMediaUrl.Length == 0)
        //            //    splitMediaUrl = Regex.Split(postResponse, "hd_src\":\"").Skip(1).ToArray();

        //            var splitMediaUrl = Regex.Split(postResponse, "sd_src_no_ratelimit\":\"").Skip(1).ToArray();

        //            if (splitMediaUrl.Length == 0)
        //                splitMediaUrl = Regex.Split(postResponse, "sd_src_no_ratelimit:\"").Skip(1).ToArray();

        //            if (splitMediaUrl.Length > 1)
        //            {

        //                var firstMediaUrlSet = false;

        //                foreach (var splitdata in splitMediaUrl)
        //                {
        //                    try
        //                    {

        //                        var sourceUrl = Utilities.GetBetween(splitdata, "", "\"");
        //                        sourceUrl = FdFunctions.GetDecodedResponse(sourceUrl);
        //                        if (!firstMediaUrlSet)
        //                        {
        //                            firstMediaUrlSet = true;
        //                            if (string.IsNullOrEmpty(adMediaUrl))
        //                                adMediaUrl = sourceUrl;
        //                        }

        //                        if (videoCount >= count)
        //                            break;

        //                        if (splitMediaUrl.Length > 1 && count > 1)
        //                        {
        //                            if (string.IsNullOrEmpty(adOtherMediaUrl))
        //                            {
        //                                //Byte[] _byte = objGlobusHttpHelper.getImageBytesfromUrl(new Uri(sourceUrl));
        //                                //sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
        //                                adOtherMediaUrl = sourceUrl;
        //                            }
        //                            else
        //                            {
        //                                //Byte[] _byte = objGlobusHttpHelper.getImageBytesfromUrl(new Uri(sourceUrl));
        //                                //sourceUrl = Convert.ToBase64String(_byte, 0, _byte.Length);
        //                                adOtherMediaUrl = adOtherMediaUrl + "||," + sourceUrl;
        //                            }
        //                        }

        //                        videoCount++;

        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Console.WriteLine(e.StackTrace);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                adMediaUrl = Utilities.GetBetween(postResponse, "hd_src\":\"", "\"");

        //                if (string.IsNullOrEmpty(adMediaUrl))
        //                    adMediaUrl = Utilities.GetBetween(postResponse, "hd_src:\"", "\"");

        //                adMediaUrl = FdFunctions.GetDecodedResponse(adMediaUrl);
        //            }

        //            adMediaUrl = WebUtility.HtmlDecode(adMediaUrl);

        //            if (!string.IsNullOrEmpty(adMediaUrl))
        //            {
        //                adMediaType = "VIDEO";

        //                objPostDetails.MediaType = MediaType.Video;
        //            }
        //        }
        //        #endregion



        //        #endregion


        //        if (adNavigationUrl.Contains("http://") || adNavigationUrl.Contains("https://"))
        //        {
        //            string httpParameter = "https://";
        //            adNavigationUrl += " ";
        //            string destinationUrl = Utilities.GetBetween(adNavigationUrl, httpParameter, " ");

        //            if (string.IsNullOrEmpty(destinationUrl))
        //            {
        //                httpParameter = "http://";
        //                adNavigationUrl += " ";
        //                destinationUrl = Utilities.GetBetween(adNavigationUrl, httpParameter, " ");
        //            }

        //            if (!string.IsNullOrEmpty(destinationUrl))
        //            {
        //                destinationUrl = httpParameter + destinationUrl;
        //                adNavigationUrl = destinationUrl;
        //            }
        //        }

        //        try
        //        {
        //            HtmlDocument objNDocument = new HtmlDocument();
        //            objNDocument.LoadHtml(postDetailsNew);

        //            var objCollection = objNDocument.DocumentNode.SelectNodes("//span[@class=\"fwb fcg\"]") ??
        //                                objNDocument.DocumentNode.SelectNodes("//span[@class=\"fwb\"]");

        //            if (objCollection != null)
        //            {
        //                var postOwnerName = objCollection[0].InnerText;

        //                var postOwnerId = objCollection[0].OuterHtml;

        //                if (postOwnerId.Contains("page.php?id="))
        //                {
        //                    postOwnerId = Utilities.GetBetween(postOwnerId, "page.php?id=", "&");
        //                    entityType = FbEntityTypes.Page;
        //                }
        //                else if (postOwnerId.Contains("user.php?id="))
        //                {
        //                    postOwnerId = Utilities.GetBetween(postOwnerId, "user.php?id=", "&");
        //                    entityType = FbEntityTypes.Friend;
        //                }

        //                if (string.IsNullOrEmpty(postOwnerId) || !FdFunctions.IsIntegerOnly(postOwnerId))
        //                {
        //                    if (postOwnerId.Contains("pageID\":\""))
        //                    {
        //                        postOwnerId = Utilities.GetBetween(postDetailsNew, "pageID\":\"", "\"");
        //                        ownerName = Utilities.GetBetween(postDetailsNew, "pageName\":\"", "\"");
        //                        entityType = FbEntityTypes.Page;
        //                    }
        //                    else
        //                    {
        //                        postOwnerId = Utilities.GetBetween(postDetailsNew, "entity_id\":\"", "\"");
        //                        entityType = FbEntityTypes.Friend;
        //                    }
        //                }

        //                ownerName = postOwnerName;
        //                ownerId = postOwnerId;
        //            }


        //            var postEntityDetails = Utilities.GetBetween(postResponse, "page_uri:\"", "\"");

        //            if (string.IsNullOrEmpty(postEntityDetails))
        //                postEntityDetails = Utilities.GetBetween(postResponse, "page_uri\":\"", "\"");

        //            //if (postEntityDetails.Contains("www.facebook.com/groups/"))
        //            //{
        //            //    ownerId = Utilities.GetBetween(postEntityDetails, "www.facebook.com/groups/", "/");
        //            //    entityType = FbEntityTypes.Group;
        //            //}


        //        }
        //        catch (AggregateException ex)
        //        {
        //            ex.DebugLog();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }

        //        if (adTitle == "Facebook Creator")
        //            adTitle = string.Empty;

        //        HtmlDocument objHtmlDocument = new HtmlDocument();

        //        try
        //        {

        //            postDetailsFullText = WebUtility.HtmlDecode(postDetailsFullText);

        //            if (string.IsNullOrEmpty(postDetailsFullText))
        //                postDetailsFullText = postResponse;
        //            if (!postDetailsFullText.Contains(adSubDescription))
        //                adSubDescription = string.Empty;
        //            if (!postDetailsFullText.Contains(adTitle))
        //                adTitle = string.Empty;

        //            if (string.IsNullOrEmpty(adTitle) && adMediaType == "VIDEO")
        //            {
        //                try
        //                {

        //                    var body = FdFunctions.GetDecodedResponse(postResponse);
        //                    objHtmlDocumentNew.LoadHtml(body);
        //                    var postTitleDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _5uun _4-u8\"]");

        //                    if (postTitleDetails != null)
        //                        if (postTitleDetails[0].InnerHtml.Contains(adId))
        //                            adTitle = FdRegexUtility
        //                                .FirstMatchExtractor(postTitleDetails[0].InnerHtml, FdConstants.AdTitleRegx)
        //                                .Replace(",", string.Empty).Trim();

        //                }
        //                catch (Exception ex)
        //                {
        //                    ex.DebugLog();
        //                }
        //            }

        //            if (!postDetailsFullText.Contains(mainDescription))
        //            {
        //                var splitDescription = Regex.Split(mainDescription, "\r\n");

        //                var isCorrectDesc = true;

        //                foreach (string desc in splitDescription)
        //                {
        //                    if (!postDetailsFullText.Contains(desc))
        //                    {
        //                        isCorrectDesc = false;
        //                        break;
        //                    }
        //                }

        //                if (!isCorrectDesc)
        //                {
        //                    objHtmlDocumentNew.LoadHtml(postDetailsNew);
        //                    var maindescriptionDetails = objHtmlDocumentNew.DocumentNode.SelectNodes("//div[@class=\"_5pbx userContent _3576\"]");
        //                    if (maindescriptionDetails != null)
        //                    {
        //                        mainDescription = maindescriptionDetails[0].InnerText;
        //                        mainDescription = WebUtility.HtmlDecode(mainDescription);
        //                    }
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }

        //        //mtm _5pco _2zpv

        //        if (string.IsNullOrEmpty(mainDescription))
        //        {
        //            objHtmlDocument.LoadHtml(postDetailsNew);
        //            var description = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'mtm _5pco')]");
        //            if (description != null)
        //            {
        //                mainDescription = description[0].InnerHtml;
        //                mainDescription = mainDescription.Replace("<p>", string.Empty);
        //                mainDescription = mainDescription.Replace("</p>", "\r\n");
        //                mainDescription = mainDescription.Replace("<br>", "\r\n");
        //                objHtmlDocument.LoadHtml(mainDescription);
        //                mainDescription = objHtmlDocument.DocumentNode.InnerText;
        //            }
        //        }

        //        if (string.IsNullOrEmpty(mainDescription))
        //        {
        //            objHtmlDocument.LoadHtml(postDetailsNew);
        //            var description = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_5pbx userContent')]");
        //            if (description != null)
        //                mainDescription = description[0].InnerText;

        //        }
        //        if (string.IsNullOrEmpty(mainDescription))
        //        {
        //            objHtmlDocument.LoadHtml(postDetailsNew);
        //            var description = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_5qxm _5qxn _620g')]");
        //            if (description != null)
        //            {
        //                var decodedDescription = description[0].InnerText;
        //                description = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_4a6n _a5_')]");

        //                mainDescription = description != null ? description[0].InnerText : decodedDescription;
        //            }
        //        }

        //        if (postDetailsNew.Contains("aria-label=\"Map attachment\""))
        //        {
        //            objHtmlDocument.LoadHtml(postDetailsNew);
        //            var description = objHtmlDocument.DocumentNode.SelectNodes("//img[starts-with(@class, '_a3f img')]");
        //            if (description != null)
        //            {
        //                adMediaUrl = description[0].OuterHtml;
        //                adMediaUrl = Utilities.GetBetween(adMediaUrl, "src=\"", "\"");

        //                if (!string.IsNullOrEmpty(adMediaUrl))
        //                    objPostDetails.MediaType = MediaType.Image;
        //            }
        //        }

        //        if (postDetailsNew.Contains("TahoeController") && string.IsNullOrEmpty(adMediaUrl))
        //            objPostDetails.MediaType = MediaType.Video;
        //        else if (string.IsNullOrEmpty(adMediaUrl))
        //            objPostDetails.MediaType = MediaType.NoMedia;

        //        objPostDetails.MediaUrl = !string.IsNullOrEmpty(adMediaUrl) ? adMediaUrl : "NA";
        //        objPostDetails.OtherMediaUrl = !string.IsNullOrEmpty(adOtherMediaUrl) ? adOtherMediaUrl : "NA";
        //        objPostDetails.Title = !string.IsNullOrEmpty(adTitle) ? adTitle : "NA";
        //        objPostDetails.SubDescription = !string.IsNullOrEmpty(adSubDescription) ? adSubDescription : "NA";
        //        objPostDetails.Caption = !string.IsNullOrEmpty(mainDescription) ? mainDescription : "NA";
        //        objPostDetails.NavigationUrl = !string.IsNullOrEmpty(adNavigationUrl) ? adNavigationUrl : objPostDetails.PostUrl;
        //        objPostDetails.OwnerName = !string.IsNullOrEmpty(ownerName) ? ownerName : string.Empty;
        //        objPostDetails.OwnerId = !string.IsNullOrEmpty(ownerId) ? ownerId : string.Empty;
        //        objPostDetails.OwnerLogoUrl = !string.IsNullOrEmpty(ownerLogoUrl) ? ownerLogoUrl : string.Empty;
        //        objPostDetails.PostedDateTime = (!string.IsNullOrEmpty(adPostedDateTime) && FdFunctions.IsIntegerOnly(adPostedDateTime)) ? Int32.Parse(adPostedDateTime).EpochToDateTimeLocal() : DateTime.Now;
        //        objPostDetails.ScapedUrl = !string.IsNullOrEmpty(scrapedUrl) && !scrapedUrl.Contains("watch") ? $"{FdConstants.FbHomeUrl}{scrapedUrl}" : $"{FdConstants.FbHomeUrl}{objPostDetails.Id}";
        //        objPostDetails.EntityType = entityType;
        //        Status = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }


        //    return false;
        //} 
        #endregion

        public bool GetFacebookPostDetailsNewUI(string postResponse, FacebookPostDetails objPostDetails)
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

            FbEntityTypes entityType = FbEntityTypes.Friend;

            var postDetailsFullText = string.Empty;

            var scrapedUrl = string.Empty;

            var adPostedDateTime = Utilities.GetBetween(postResponse, "created_time\":", ",\"");

            try
            {
                ownerId = FdRegexUtility.FirstMatchExtractor(postResponse, "page_id\":\"(.*?)\",");
                if (!FdFunctions.IsIntegerOnly(ownerId))
                {
                    var pageInsights = FdRegexUtility.FirstMatchExtractor(postResponse, "page_insights(.*?)page_id_type");
                    ownerId = FdRegexUtility.FirstMatchExtractor(pageInsights, "page_id\":\"(.*?)\",");
                }
                var postIdDetails = FdRegexUtility.FirstMatchExtractor(postResponse, "post_id\":\"(.*?)\",");
                objPostDetails.Id = !string.IsNullOrEmpty(objPostDetails.Id) ? objPostDetails.Id : postIdDetails;
                var mediaUrlNode = HtmlParseUtility.GetOuterHtmlFromTagName(postResponse, "link", "as", "image");
                adMediaUrl = FdRegexUtility.FirstMatchExtractor(mediaUrlNode, "href=\"(.*?)\"");
                //ownerLogoUrl = HtmlParseUtility.GetOuterHtmlFromTagName(postResponse, "link", "as", "image");           
            }
            catch (Exception)
            { }

            try
            {
                objPostDetails.MediaUrl = !string.IsNullOrEmpty(adMediaUrl) ? adMediaUrl : "NA";
                objPostDetails.MediaType = !string.IsNullOrEmpty(objPostDetails.MediaUrl) ? MediaType.Image : MediaType.NoMedia;
                objPostDetails.OtherMediaUrl = "NA";
                objPostDetails.Title = !string.IsNullOrEmpty(adTitle) ? adTitle : "NA";
                objPostDetails.SubDescription = "NA";
                objPostDetails.Caption = "NA";
                objPostDetails.NavigationUrl = objPostDetails.PostUrl;
                objPostDetails.OwnerName = !string.IsNullOrEmpty(ownerName) ? ownerName : string.Empty;
                objPostDetails.OwnerId = !string.IsNullOrEmpty(ownerId) ? ownerId : string.Empty;
                objPostDetails.OwnerLogoUrl = !string.IsNullOrEmpty(ownerLogoUrl) ? ownerLogoUrl : string.Empty;
                objPostDetails.PostedDateTime = (!string.IsNullOrEmpty(adPostedDateTime) && FdFunctions.IsIntegerOnly(adPostedDateTime)) ? Int32.Parse(adPostedDateTime).EpochToDateTimeLocal() : DateTime.Now;
                objPostDetails.ScapedUrl = $"{FdConstants.FbHomeUrl}{objPostDetails.Id}";
                objPostDetails.EntityType = entityType;
                Status = true;
            }
            catch (Exception)
            { }

            return true;
        }

        public bool CheckUrlValid(string source)
        {
            Uri uriResult;

            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

    }
}

