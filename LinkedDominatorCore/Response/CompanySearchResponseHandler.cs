using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class CompanySearchResponseHandler : LdResponseHandler
    {
        public CompanySearchResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("<!DOCTYPE html>") || response.Response.Contains("\"primaryResultType\":\"COMPANIES\""));
                if (!Success)
                    return;

                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;

                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    var tempJson = "{\"data\":{\"metadata\":{\"primaryResultType\":\"" + Utils
                                       .GetBetween(response.Response, "{\"data\":{\"metadata\":{\"primaryResultType\":\"",
                                           "</code>")?.Trim();
                    RespJ = jsonJArrayHandler.ParseJsonToJObject(tempJson);
                    TotalResults = jsonJArrayHandler.GetJTokenValue(RespJ, "data", "metadata", "totalResultCount");
                    if(string.IsNullOrEmpty(TotalResults))
                        TotalResults= System.Text.RegularExpressions.Regex.Replace((HtmlAgilityHelper.GetStringInnerTextFromClassName(response.Response,
                            "pb2 t-black--light t-14")), "[^0-9]", "");
                    NormalCompanyBrowserResponseHandler();
                    return;
                }


                TotalResults = jsonJArrayHandler.GetJTokenValue(RespJ, "metadata", "totalResultCount");
                var arrayElement = jsonJArrayHandler.GetJArrayElement(
                        jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 0, "elements"));
                if(arrayElement == null || arrayElement.Count == 0)
                    arrayElement = jsonJArrayHandler.GetJArrayElement(
                        jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 1, "results"));
                if(arrayElement==null || !arrayElement.HasValues)
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenOfJToken(jsonJArrayHandler.ParseJsonToJObject(jsonJArrayHandler.GetJTokenOfJToken(RespJ, "elements").FirstOrDefault(x => x.ToString().Contains("entityResult")).ToString()), "items").ToString()).ToString());
                if (arrayElement == null || arrayElement.Count == 0)
                {
                    Success = false;
                    return;
                }

                foreach (var element in arrayElement)
                    try
                    {
                        var companyDetails = jsonJArrayHandler.GetJTokenOfJToken(element, "itemUnion", "entityResult");
                        var companyId =jsonJArrayHandler.GetJTokenValue(companyDetails, "trackingUrn")?.Split(':').Last();

                        var objLinkedinCompany = new LinkedinCompany(companyId)
                        {
                            CompanyName = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(companyDetails, "image",
                                "attributes", 0, "miniCompany", "name"))),
                            TotalEmployees = "N/A",
                            Industry = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(companyDetails, "headline", "text"))),
                            IsFollowed = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(companyDetails, "hitInfo",
                                "com.linkedin.voyager.search.SearchCompany", "following", "following")))
                        };
                        objLinkedinCompany.CompanyName = objLinkedinCompany.CompanyName == "N/A"?Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(companyDetails, "title","text"))):objLinkedinCompany.CompanyName;
                        objLinkedinCompany.Industry = objLinkedinCompany.Industry == "N/A" ? Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(companyDetails, "primarySubtitle", "text"))) : objLinkedinCompany.Industry;
                        var companyUrl = jsonJArrayHandler.GetJTokenValue(companyDetails, "navigationUrl");
                        objLinkedinCompany.CompanyUrl = string.IsNullOrEmpty(companyUrl) ? objLinkedinCompany.CompanyUrl : companyUrl;
                        objLinkedinCompany.IsFollowed = objLinkedinCompany.IsFollowed == "N/A" ? Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(companyDetails, "primaryActions",0, "actionDetails", "followAction", "following")) : objLinkedinCompany.IsFollowed;
                        // get logo url
                        if(!string.IsNullOrEmpty(objLinkedinCompany.Industry) && objLinkedinCompany.Industry.Contains("•") && string.IsNullOrEmpty(objLinkedinCompany.Location))
                            objLinkedinCompany.Location= System.Text.RegularExpressions.Regex.Split(objLinkedinCompany.Industry, "•")[1].ToString().Trim();
                        var logoRootUrlId = jsonJArrayHandler.GetJTokenValue(companyDetails, "image","attributes", 0, "miniCompany", "logo", "com.linkedin.common.VectorImage", "rootUrl");
                        logoRootUrlId = string.IsNullOrEmpty(logoRootUrlId) ? jsonJArrayHandler.GetJTokenValue(companyDetails, "image","attributes", 0, "detailData", "companyLogo", "logo", "vectorImage", "rootUrl"):logoRootUrlId;
                        objLinkedinCompany.LogoUrl = Utils.AssignNa(string.IsNullOrEmpty(logoRootUrlId) ? jsonJArrayHandler.GetJTokenValue(companyDetails, "image","attributes", 0, "detailDataUnion", "nonEntityCompanyLogo", "vectorImage","artifacts", 0, "fileIdentifyingUrlPathSegment"): logoRootUrlId);
                        if (CompanyList.All(x => x.CompanyId != objLinkedinCompany.CompanyId))
                            CompanyList.Add(objLinkedinCompany);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public CompanySearchResponseHandler(IResponseParameter response, bool isSalesNavigator) : base(response)
        {
            if (!string.IsNullOrEmpty(response.Response) && response.Response != "Please Check Headers")
            {
                Success = !response.Response.Contains("\"searchResults\":[]") &&
                          !response.Response.Contains("\"elements\":[]");
                if(!Success)
                    Success= response.Response.Contains("\"elements\":[]");
                ProfilePageSource = response.Response;
            }

            if (!Success)
                return;

            if (response.Response.Contains("<!DOCTYPE html>"))
                ProfilePageSource = Utils.GetBetweenAndAddStart(response.Response,
                    "{\"metadata\":{\"decoratedSpotlights\":{\"all\":", "</code>");
            if(string.IsNullOrEmpty(ProfilePageSource))
                ProfilePageSource = Utils.GetBetweenAndAddStart(response.Response,
                    "{\"metadata\":{\"totalDisplayCount\"", "</code>");
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            try
            {
                var objJObject = JObject.Parse(ProfilePageSource);
                JArray arrayElement;

                try
                {
                    arrayElement = JArray.Parse(objJObject["searchResults"].ToString());
                    if (arrayElement.Contains("searchResults\":[]"))
                    {
                        Success = false;
                        return;
                    }
                }
                catch (Exception)
                {
                    arrayElement = JArray.Parse(objJObject["elements"].ToString());
                }


                TotalResults = jsonJArrayHandler.GetJTokenValue(objJObject, "pagination", "total");
                Count = jsonJArrayHandler.GetJTokenValue(objJObject, "pagination", "count");

                if (string.IsNullOrEmpty(TotalResults))
                    TotalResults = jsonJArrayHandler.GetJTokenValue(objJObject, "paging", "total");
                if (string.IsNullOrEmpty(Count))
                    Count = jsonJArrayHandler.GetJTokenValue(objJObject, "paging", "count");


                foreach (var item in arrayElement)
                    try
                    {
                        var companyId = jsonJArrayHandler.GetJTokenValue(item, "entityUrn");
                        if (string.IsNullOrEmpty(companyId))
                            companyId = jsonJArrayHandler.GetJTokenValue(item, "objectUrn");
                        companyId = companyId.Split(':').Last()?.Replace("(", "").Replace(")", "");

                        #region Creating New object for LinkedinUser

                        //companyName
                        var linkedinCompany = new LinkedinCompany(companyId)
                        {
                            CompanyId = companyId,
                            CompanyName =
                                Utils.InsertSpecialCharactersInCsv(Utils.RemoveSpecialCharacters(
                                    jsonJArrayHandler.GetJTokenValue(item, "companyName"))),
                            Industry = Utils.AssignNa(Utils.RemoveSpecialCharacters(
                                Utils.InsertSpecialCharactersInCsv(jsonJArrayHandler.GetJTokenValue(item, "industry")))),
                            CompanyUrl = $"https://www.linkedin.com/sales/company/{companyId}",
                            TotalEmployees =
                                Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(item, "employeeDisplayCount"))),
                            Location = Utils.AssignNa(Utils.RemoveSpecialCharacters(
                                Utils.InsertSpecialCharactersInCsv(jsonJArrayHandler.GetJTokenValue(item, "location")))),
                            LogoUrl = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "companyPictureDisplayImage", "rootUrl") + jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetTokenElement(item, "companyPictureDisplayImage", "artifacts").ToString()).LastOrDefault(x => x.ToString().Contains("\"width\": 400") || x.ToString().Contains("\"width\": 200") || x.ToString().Contains("\"width\": 100")), "fileIdentifyingUrlPathSegment")),
                            IsFollowed = "N/A"
                        };

                        #endregion


                        if (!CompanyList.Contains(linkedinCompany))
                            CompanyList.Add(linkedinCompany);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        GlobusLogHelper.log.Info(
                            "memberId doesnot exist on getting List of salesnavigator users from your search query");
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string Count { get; set; }
        public string TotalResults { get; set; }
        public string ProfilePageSource { get; set; }
        public List<LinkedinCompany> CompanyList { get; } = new List<LinkedinCompany>();

        private void NormalCompanyBrowserResponseHandler()
        {
            try
            {
                CompanyList.Clear();
                var companyNodeList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                    "reusable-search__result-container");
                foreach (var companyNode in companyNodeList)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(companyNode.OuterHtml);
                    var companyId = Utils.GetBetween(companyNode.OuterHtml, "/company/", "\"")?.Trim()
                        ?.Trim('/');
                    var company = new LinkedinCompany(companyId);
                    company.CompanyName =Utils.GetBetween(Utils.GetBetween(htmlDoc.Text, "<a class=\"app-aware-link \"", "</a>"), "<!---->", "<!---->");
                    if(string.IsNullOrEmpty(company.CompanyName))
                        company.CompanyName =
                        HtmlAgilityHelper.GetStringInnerTextFromClassName(companyNode.OuterHtml,
                            "entity-result__title-line flex-shrink-1 entity-result__title-text--black ", htmlDoc);
                    if (string.IsNullOrEmpty(company.CompanyName))
                        company.CompanyName =
                        HtmlAgilityHelper.GetStringInnerTextFromClassName(companyNode.OuterHtml,
                            "entity-result__title-text t-16", htmlDoc);            
                    company.LogoUrl = new LdDataHelper().GetSource(companyNode.OuterHtml);
                    company.IsFollowed = HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                        "artdeco-button artdeco-button--muted artdeco-button--2 artdeco-button--secondary ember-view",
                        htmlDoc);
                    company.IsFollowed = company.IsFollowed.Contains("Following") ? "true" : "false";

                    company.Industry = System.Web.HttpUtility.HtmlDecode(HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                        "entity-result__primary-subtitle t-14 t-black", htmlDoc));
                    if (company.Industry.Contains("•"))
                    {
                        var locationString = System.Text.RegularExpressions.Regex.Split(company.Industry, "•");
                        company.Location = locationString[1].Trim();
                        company.Industry = company.Industry.Replace($"• {company.Location}", "");
                    }
                    else
                    {
                        company.Location = company.Industry;
                        company.Industry = string.Empty;
                    }
                    CompanyList.Add(company);
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}