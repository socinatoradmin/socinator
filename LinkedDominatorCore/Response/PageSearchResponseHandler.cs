using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.Response
{
    public class PageSearchResponseHandler : LdResponseHandler
    {
        public PageSearchResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            if (RespJ == null && response.Response.Contains("<!DOCTYPE html>"))
            {
                BrowserResponseHandler();
            }
            else
            {
                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var TupleElements = Utils.GetArrayToken(response.Response,false);
                var arrayElements = TupleElements.Item1;
                foreach (var results in arrayElements)
                {
                    if (string.IsNullOrEmpty(jsonJArrayHandler.GetJTokenValue(results, "template")))
                        continue;
                    var pageId = jsonJArrayHandler.GetJTokenValue(results, "objectUrn")?.Replace("urn:li:company:", "")
                        .Trim();
                    pageId = string.IsNullOrEmpty(pageId) ?jsonJArrayHandler.GetJTokenValue(results, "trackingUrn")?.Replace("urn:li:company:", "") : pageId;
                    var pagename = jsonJArrayHandler.GetJTokenValue(results, "name");
                    pagename = Utils.RemoveSpecialCharacters(string.IsNullOrEmpty(pagename) ?jsonJArrayHandler.GetJTokenValue(results,"title","text"): pagename);
                    var pageUrlname = jsonJArrayHandler.GetJTokenValue(results, "universalName");
                    pageUrlname = string.IsNullOrEmpty(pageUrlname) ?jsonJArrayHandler.GetJTokenValue(results, "navigationUrl") : pageUrlname;
                    var ObjLinkedinPage = new LinkedinPage
                    {
                        PageId = pageId,
                        PageName = pagename,
                        PageUrl =pageUrlname.Contains("company")?pageUrlname:$"https://www.linkedin.com/company/{pageUrlname}"
                    };
                    PageList.Add(ObjLinkedinPage);
                }
                Success = TupleElements.Item2;
            }
        }

        public List<LinkedinPage> PageList { get; } = new List<LinkedinPage>();

        private void BrowserResponseHandler()
        {
            var pageUserNodeList = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(Response.Response,HTMLTags.Div, "data-view-name",
                "search-entity-result-universal-template");
            foreach (var pageList in pageUserNodeList)
            {
                var pageData = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(pageList.InnerHtml,"a", "data-test-app-aware-link", "")?.LastOrDefault();
                if(pageData != null)
                {
                    var Pagename1 = Utils.RemoveHtmlTags(pageData.InnerText);
                    var PageUrl1 = Utils.GetBetween(pageData.OuterHtml, "href=\"", "/\"");
                    var ObjLinkedPage = new LinkedinPage
                    {
                        PageName = Pagename1,
                        PageUrl = PageUrl1
                    };
                    PageList.Add(ObjLinkedPage);
                }
                
            }
        }
    }
}