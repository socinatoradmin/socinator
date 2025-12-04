using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class FdSeachGroupResponseHandler : FdResponseHandler
    {
        public List<string> GroupDetais { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        public FdSeachGroupResponseHandler(IResponseParameter responseParameter, List<string> jsonResponseList, GroupDetails facebookGroup)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.facebookGroup = facebookGroup ?? (facebookGroup = new GroupDetails());
            try
            {
                foreach (var postResponse in jsonResponseList)
                {
                    JObject jObject = null;
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    if (!postResponse.IsValidJson())
                    {
                        var decodedResponse = "";
                        if (postResponse.StartsWith("<!DOCTYPE html>"))
                        {
                            var splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"data\":{\"user\":{\"profile_header_renderer\"")) + "/end", "{\"require\"", "/end");
                            decodedResponse = "{\"require\"" + splittedArray;
                            if (decodedResponse.IsValidJson())
                                jObject = parser.ParseJsonToJObject(decodedResponse);
                        }
                        else
                            decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                        fdjArray = parser.GetJArrayElement(decodedResponse);
                    }
                    else
                    {
                        fdjArray = parser.GetJArrayElement(postResponse);
                        if (fdjArray.Count() == 0)
                            jObject = parser.ParseJsonToJObject(postResponse);
                    }
                    if (fdjArray.Count() > 0)
                    {
                        var nodes = parser.GetJTokenOfJToken(fdjArray, 0, "data", "user", "about_app_sections", "nodes", 0, "activeCollections", "nodes", 0, "style_renderer", "profile_field_sections");
                        if (nodes == null || nodes.Count() == 0)
                            nodes = JsonSearcher.FindByKey(fdjArray, "profile_field_sections");
                        if (nodes != null || nodes.Count() > 0)
                        {
                            foreach (var nodeProperty in nodes)
                                elements.Add(nodeProperty);
                        }
                    }
                    if (jObject != null && jObject.Count > 0)
                    {
                        var userNode = JsonSearcher.FindByKey(jObject, "user");
                        var nodeObject = JsonSearcher.FindByKey(userNode, "user");
                        if (nodeObject != null && nodeObject.Count() > 0 && parser.GetJTokenValue(nodeObject, "__typename") == "User")
                        {
                            userNode = nodeObject;
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageID = parser.GetJTokenValue(userNode, "id");
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageName = parser.GetJTokenValue(userNode, "name");
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageUrl = parser.GetJTokenValue(userNode, "url");
                            ObjFdScraperResponseParameters.FanpageDetails.MenuUrl = parser.GetJTokenValue(userNode, "profilePhoto", "url");
                            var detailsSec = JsonSearcher.FindByKey(JsonSearcher.FindByKey(userNode, "profile_social_context"), "content");
                            ObjFdScraperResponseParameters.FanpageDetails.IsVerifiedPage = JsonSearcher.FindStringValueByKey(nodeObject, "is_verified");
                            foreach (var details in detailsSec)
                            {
                                var text = parser.GetJTokenValue(details, "text", "text");
                                var countString = "0";
                                if (text.Contains("followers"))
                                    countString = text.Replace(" ", "").Replace("followers", "");
                                else if (text.Contains("likes"))
                                    countString = text.Replace(" ", "").Replace("likes", "");
                                if (countString.ToLower().Contains("m"))
                                    countString = (double.Parse(FdFunctions.GetDouleOnlyString(countString)) * 1000000).ToString(CultureInfo.InvariantCulture);
                                else if (countString.ToLower().Contains("k"))
                                    countString = (double.Parse(FdFunctions.GetDouleOnlyString(countString)) * 1000).ToString(CultureInfo.InvariantCulture);

                                if (text.Contains("followers"))
                                    ObjFdScraperResponseParameters.FanpageDetails.FanpageFollowerCount = countString;
                                else if (text.Contains("likes"))
                                    ObjFdScraperResponseParameters.FanpageDetails.FanPageLikerCount = countString;

                            }
                        }
                        else
                        {
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "user", "about_app_sections", "nodes", 0, "activeCollections", "nodes", 0, "style_renderer", "profile_field_sections");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "profile_field_sections");
                            if (nodes != null || nodes.Count() > 0)
                            {
                                foreach (var nodeProperty in nodes)
                                    elements.Add(nodeProperty);
                            }
                        }

                    }
                    foreach (var element in elements)
                    {
                        var elementNodes = JsonSearcher.FindByKey(element, "nodes");
                        foreach (var elementNode in elementNodes)
                        {
                            var key = JsonSearcher.FindStringValueByKey(elementNode, "field_type");
                            if (string.IsNullOrEmpty(key) || key == "null_state") continue;
                            var text = parser.GetJTokenValue(elementNode, "title", "text");


                        }
                    }
                }

            }
            catch (Exception e)
            { e.DebugLog(); }
        }
    }
}
