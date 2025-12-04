using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class GetFanpageFullDetailsResponseHandler : FdResponseHandler
    {

        public FacebookAdsDetails ObjFacebookAdsDetails = new FacebookAdsDetails();

        public GetFanpageFullDetailsResponseHandler(IResponseParameter responseParameter) :
            base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    ObjFacebookAdsDetails = new FacebookAdsDetails();

                    var decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);
                    GetBasicDetails(decodedResponse, responseParameter.Response);
                    GetFullDetails(decodedResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public void GetBasicDetails(string decodedResponse, string response)
        {
            try
            {
                var pageId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntityIdRegex);

                if (string.IsNullOrEmpty(pageId))
                {
                    pageId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PageIdModRegx);
                }

                if (!string.IsNullOrEmpty(pageId))
                {
                    ObjFacebookAdsDetails.OwnerId = pageId;
                }

                var pageName = FdRegexUtility.FirstMatchExtractor(response, FdConstants.FanpageNameRegx);

                if (string.IsNullOrEmpty(pageName))
                {
                    pageName = FdRegexUtility.FirstMatchExtractor(response, FdConstants.PageTitleRegx);
                }

                pageName = FdFunctions.GetNewPrtialDecodedResponse(pageName);

                if (!string.IsNullOrEmpty(pageName))
                {
                    ObjFacebookAdsDetails.OwnerName = pageName;
                }


                var pageCategory = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.CategoryLabelRegx).Replace("\"", "");

                if (string.IsNullOrEmpty(pageCategory))
                {
                    pageCategory = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.CategoryNameRegx);
                }

                if (string.IsNullOrEmpty(pageCategory))
                {
                    try
                    {
                        var edcodedResponse = FdFunctions.GetDecodedResponse(decodedResponse);

                        HtmlDocument objDocument = new HtmlDocument();
                        objDocument.LoadHtml(edcodedResponse);

                        var nodeCollection = objDocument.DocumentNode.SelectNodes("//div[@class=\"clearfix _ikh\"]");

                        pageCategory = nodeCollection.FirstOrDefault(x => x.InnerHtml.Contains("https://static.xx.fbcdn.net/rsrc.php/v3/y_/r/On-c9iceH4S.png"))?.InnerText;

                        if (pageCategory != null && pageCategory.Contains(" · "))
                        {
                            pageCategory = Regex.Split(pageCategory, " · ")[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                if (string.IsNullOrEmpty(pageCategory))
                {
                    ObjFacebookAdsDetails.OwnerCategory = FdRegexUtility.FirstMatchExtractor(decodedResponse, "page_about_category\">(.*?)<");
                }

                if (string.IsNullOrEmpty(pageCategory) && string.IsNullOrEmpty(ObjFacebookAdsDetails.OwnerCategory))
                {
                    ObjFacebookAdsDetails.OwnerCategory = FdRegexUtility.FirstMatchExtractor(decodedResponse, "category_type\":\"(.*?)\"");
                    ObjFacebookAdsDetails.OwnerCategory = ObjFacebookAdsDetails.OwnerCategory.Replace('_', ' ');
                }

                if (string.IsNullOrEmpty(pageCategory) && string.IsNullOrEmpty(ObjFacebookAdsDetails.OwnerCategory))
                    pageCategory = FdRegexUtility.FirstMatchExtractor(Regex.Split(response,
                           "page_about_category").LastOrDefault() ?? string.Empty, ">(.*?)<").Replace("_", " ");

                if (string.IsNullOrEmpty(pageCategory) && string.IsNullOrEmpty(ObjFacebookAdsDetails.OwnerCategory))
                    pageCategory = FdRegexUtility.FirstMatchExtractor(Regex.Split(response,
                           "\"/pages/category/").LastOrDefault() ?? string.Empty, ">(.*?)<").Replace("_", " ");

                if (string.IsNullOrEmpty(ObjFacebookAdsDetails.OwnerCategory) && !string.IsNullOrEmpty(pageCategory))
                    ObjFacebookAdsDetails.OwnerCategory = pageCategory;

                var pageLocation = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PageLocationRegx);

                if (!string.IsNullOrEmpty(pageLocation))
                {
                    ObjFacebookAdsDetails.OwnerLocation = pageLocation;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }


        public void GetFullDetails(string decodedResponse)
        {
            try
            {
                var ownerLogoUrl = string.Empty;

                try
                {
                    var ownerLogoData = Regex.Split(decodedResponse, "pageHasPhotos\":", RegexOptions.Singleline)[1];

                    if (!string.IsNullOrEmpty(ownerLogoData))
                    {
                        ownerLogoUrl = Regex.Matches(ownerLogoData, "uri\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
                        ownerLogoUrl = Regex.Replace(ownerLogoUrl, "\\/", "/");
                        ownerLogoUrl = Regex.Replace(ownerLogoUrl, "\\\\", "");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                if (string.IsNullOrEmpty(ownerLogoUrl))
                {
                    HtmlDocument objDocument2 = new HtmlDocument();

                    objDocument2.LoadHtml(decodedResponse);

                    ownerLogoUrl = objDocument2.DocumentNode.SelectNodes("//div[@class=\"_38vo\"]")[0].OuterHtml;

                    ownerLogoUrl = Regex.Matches(ownerLogoUrl, FdConstants.ImageSrcRegex.ToString(), RegexOptions.Singleline)[0].Groups[1].ToString();
                }

                ObjFacebookAdsDetails.OwnerLogoUrl = ownerLogoUrl;

                var composerId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ComposerIdRegex);

                ObjFacebookAdsDetails.ComposerId = composerId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
