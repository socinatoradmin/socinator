using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class MyPageResponseHandler : LdResponseHandler
    {
        private readonly JsonJArrayHandler _jsonJArrayHandler = JsonJArrayHandler.GetInstance;

        public List<LinkedinPage> listOfPages = new List<LinkedinPage>();

        public MyPageResponseHandler(IResponseParameter response) : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"organizationDashCompaniesByViewerPermissions\"") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;
            if (response.Response.Contains("<!DOCTYPE html>"))
            {
                var data = Utils.GetBetween(response.Response, "{\"data\":{\"memberGroup\":",
                    "</code>");
                var finalData = "{\"data\":{\"memberGroup\":" + data;
                var arry = JObject.Parse(finalData);
                var pageData = _jsonJArrayHandler.GetTokenElement(arry, "included");
                foreach (var item in pageData)
                {
                    var pageName = _jsonJArrayHandler.GetJTokenValue(item, "name");
                    var pageId = _jsonJArrayHandler.GetJTokenValue(item, "objectUrn")
                        ?.Replace("urn:li:company:", "");
                    var pageUniversalName = _jsonJArrayHandler.GetJTokenValue(item, "universalName");
                    var linkedInpagedetails = new LinkedinPage(pageId);
                    linkedInpagedetails.PageId = pageId;
                    linkedInpagedetails.PageName = pageName;
                    linkedInpagedetails.UniversalPageName = pageUniversalName;
                    listOfPages.Add(linkedInpagedetails);
                }

                return;
            }

            var aary = _jsonJArrayHandler.ParseJsonToJObject(response.Response);
            var companies = _jsonJArrayHandler.GetTokenElement(aary, "companies");
            if (companies == null || !companies.HasValues)
                companies = _jsonJArrayHandler.GetJArrayElement(_jsonJArrayHandler.GetJTokenValue(aary, "data", "organizationDashCompaniesByViewerPermissions", "elements"));
            foreach (var companiesDetails in companies)
            {
                var pageName = _jsonJArrayHandler.GetJTokenValue(companiesDetails, "name");
                var pageId = _jsonJArrayHandler.GetJTokenValue(companiesDetails, "objectUrn")
                    ?.Replace("urn:li:company:", "");
                pageId = string.IsNullOrEmpty(pageId) ? _jsonJArrayHandler.GetJTokenValue(companiesDetails, "entityUrn")
                    ?.Replace("urn:li:fsd_company:", "") : pageId;
                var pageUniversalName = _jsonJArrayHandler.GetJTokenValue(companiesDetails, "universalName");
                var linkedInpagedetails = new LinkedinPage(pageId);
                linkedInpagedetails.PageId = pageId;
                linkedInpagedetails.PageName = pageName;
                linkedInpagedetails.UniversalPageName = pageUniversalName;
                listOfPages.Add(linkedInpagedetails);
            }
        }
    }
}