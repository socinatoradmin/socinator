/*
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.Publisher
{
 
/*
    public class GroupPostlistResponseHandler : FdResponseHandler
    {
        public List<FacebookPostDetails> ListFacebookPostDetails { get; set; }

        public string Pagelet { get; set; }

        public string AjaxToken { get; set; }

        public bool HasMoreResults { get; set; }

        public GroupPostlistResponseHandler(IResponseParameter responseParameter, string ajaxpipeToken)
            : base(responseParameter)
        {

            if (!string.IsNullOrEmpty(ajaxpipeToken))
            {
                AjaxToken = ajaxpipeToken;
            }

            ListFacebookPostDetails = new List<FacebookPostDetails>();


            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse);

            GetPagelet(decodedResponse);
        }


        private void GetPagelet(string decodedResponse)
        {
            try
            {
                string pageletSection;
                if (string.IsNullOrEmpty(AjaxToken))
                {
                    pageletSection = Regex.Matches(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = "{\"" + Regex.Replace(pageletSection, ":", "\":").Replace(",", ",\"") + "}";

                    Pagelet = pageletSection;

                    var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                    AjaxToken = ajaxTokenSection;
                }
                else
                {
                    pageletSection = Regex.Matches(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},", RegexOptions.Singleline)[0].Groups[1].ToString();
                    Pagelet = ("{" + pageletSection + "}");
                }

                if (!string.IsNullOrEmpty(Pagelet))
                    HasMoreResults = true;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPostIdList(string decodedResponse)
        {
            FdFunctions objFdFunctions = new FdFunctions();

            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(decodedResponse);

            try
            {
                //"(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _4-u8\"])"

                HtmlNodeCollection objNodecollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _4-u8\"])");

                List<string> objNodeList = objFdFunctions.GetInnerHtmlListFromNodeCollection(objNodecollection);

                foreach (string node in objNodeList)
                {
                    FacebookPostDetails objFacebookPostDetails = new FacebookPostDetails();

                    try
                    {
                        objHtmlDocument.LoadHtml(node);
                        var postDetails = objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])")[0].OuterHtml;
                        var postId = Regex.Matches(postDetails, "value=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        objFacebookPostDetails.Id = postId;
                        ListFacebookPostDetails.Add(objFacebookPostDetails);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (ListFacebookPostDetails.Count > 0)
                        Success = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
#1#
}
*/
