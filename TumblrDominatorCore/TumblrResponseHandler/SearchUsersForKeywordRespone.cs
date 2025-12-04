using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchUsersForKeywordRespone : ResponseHandler
    {
        public List<TumblrUser> LstTumblrUser = new List<TumblrUser>();
        public SearchUsersForKeywordRespone()
        {
        }
        public SearchUsersForKeywordRespone(bool istrue)
        {
            Success = istrue;
        }


        public SearchUsersForKeywordRespone(IResponseParameter responeParameter) : base(responeParameter)
        {
            if (responeParameter != null && !string.IsNullOrEmpty(responeParameter.Response))
            {


                #region Users

                if (Response.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}") ||
                     responeParameter.Response.Contains("\"queryKey\":[\"user-info\",true]"))
                {
                    Success = true;
                    try
                    {
                        JObject jObject = null;
                        try
                        {
                            if (!Response.Response.IsValidJson() && Response.Response.StartsWith("<!DOCTYPE html>"))
                            {
                                var requireData1 = TumblrUtility.GetJsonFromPageResponse(Response.Response)?.Trim()?.TrimEnd(';');
                                if (requireData1.IsValidJson()) jObject = parser.ParseJsonToJObject(requireData1);
                            }
                            else if (!Response.Response.IsValidJson())
                            {
                                var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(Response.Response);
                                if (decodedResponse.IsValidJson())
                                    jObject = parser.ParseJsonToJObject(decodedResponse);
                            }
                            else
                            {
                                jObject = parser.ParseJsonToJObject(Response.Response);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        var data = parser.GetJTokenOfJToken(jObject, "response", "timeline", "elements");
                        if (data == null || data.Count() == 0)
                            data = parser.GetJTokenOfJToken(jObject, "queries", "queries", 1, "state", "data");
                        if (data == null || data.Count() == 0)
                            data = parser.GetJTokenOfJToken(jObject, "PeeprRoute", "initialTimeline", "objects");
                        if (data == null || data.Count() == 0)
                            data = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "timeline"), "elements");
                        if (data == null || data.Count() == 0)
                            data = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "initialTimeline"), "objects");
                        foreach (var item in data)
                        {
                            var _name = parser.GetJTokenValue(item, "blog", "name");
                            if (string.IsNullOrEmpty(_name))
                                _name = JsonSearcher.FindStringValueByKey(item, "name");
                            var _fullName = parser.GetJTokenValue(item, "blogName");
                            if (string.IsNullOrEmpty(_fullName))
                                _fullName = JsonSearcher.FindStringValueByKey(item, "blogName");
                            var followed = parser.GetJTokenValue(item, "blog", "followed");
                            if (string.IsNullOrEmpty(followed))
                                followed = JsonSearcher.FindStringValueByKey(item, "followed");
                            var can_Follow = parser.GetJTokenValue(item, "blog", "canBeFollowed");
                            if (string.IsNullOrEmpty(can_Follow))
                                can_Follow = JsonSearcher.FindStringValueByKey(item, "canBeFollowed");
                            var profilePic = parser.GetJTokenValue(item, "blog", "avatar", 0, "url");
                            if (string.IsNullOrEmpty(profilePic))
                                profilePic = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(item, "avatar"), "url");
                            var uuid = parser.GetJTokenValue(item, "blog", "uuid");
                            if (string.IsNullOrEmpty(uuid))
                                uuid = JsonSearcher.FindStringValueByKey(item, "uuid");
                            var placement_Id = parser.GetJTokenValue(item, "placementId");
                            if (string.IsNullOrEmpty(placement_Id))
                                placement_Id = JsonSearcher.FindStringValueByKey(item, "placementId");
                            var shareLikes = parser.GetJTokenValue(item, "blog", "shareLikes");
                            if (string.IsNullOrEmpty(shareLikes))
                                shareLikes = JsonSearcher.FindStringValueByKey(item, "shareLikes");

                            var shareFollowing = parser.GetJTokenValue(item, "blog", "shareFollowing");
                            if (string.IsNullOrEmpty(shareFollowing))
                                shareFollowing = JsonSearcher.FindStringValueByKey(item, "shareFollowing");
                            var canmessage = parser.GetJTokenValue(item, "blog", "canMessage");
                            if (string.IsNullOrEmpty(canmessage))
                                canmessage = JsonSearcher.FindStringValueByKey(item, "canMessage");
                            var pageUrl = parser.GetJTokenValue(item, "blog", "blogViewUrl");
                            if (string.IsNullOrEmpty(pageUrl))
                                pageUrl = JsonSearcher.FindStringValueByKey(item, "blogViewUrl");
                            if (string.IsNullOrEmpty(pageUrl) && !string.IsNullOrEmpty(_name))
                                pageUrl = ConstantHelpDetails.BlogViewUrl(_name);
                            LstTumblrUser.Add(new TumblrUser
                            {

                                Username = _name,
                                UserId = _name,
                                PageUrl = pageUrl,
                                FullName = _fullName,
                                ProfilePicUrl = profilePic,
                                IsFollowed = followed.Contains("True") ? true : false,
                                CanFollow = can_Follow.Contains("True") ? true : false,
                                CanMessage = canmessage.Contains("True") ? true : false,
                                Uuid = uuid,
                                UserUuid = uuid,
                                PlacementId = placement_Id,
                                ShareLikes = shareLikes.Contains("True") ? true : false,
                                ShareFollowing = shareFollowing.Contains("True") ? true : false,

                            });
                        }
                        Cursor = parser.GetJTokenValue(jObject, "response", "timeline", "links", "next", "queryParams", "cursor");
                        if (string.IsNullOrEmpty(Cursor))
                            Cursor = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "queryParams"), "cursor");
                    }
                    catch (Exception)
                    {
                    }
                }

                #endregion
            }
        }

        public string TumblrFormKey { get; set; }
        public bool IsPagination { get; set; }
        public string Cursor { get; set; }
    }
}