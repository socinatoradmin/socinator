using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System;
using System.Collections.Generic;

namespace PinDominatorCore.Response
{
    public class SearchPeopleResponseHandler : PdResponseHandler
    {
        public SearchPeopleResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            if(response.Response.Contains("<!DOCTYPE html><html"))
            {
                getdatafrombrowser(response.Response);
            }
            else
            {
                var jsonHand = new JsonHandler(response.Response);
                var jsonToken = jsonHand.GetJToken("resource_response", "data", "results");


                if (!jsonToken.HasValues)
                {
                    jsonToken = jsonHand.GetJTokenOfJToken(jsonHand.GetJToken("resources", "data", "BaseSearchResource")?
                        .First?.First, "data", "results");
                }

                foreach (JToken token in jsonToken)
                    try
                    {
                        var followerCount = 0;
                        var boards = 0;
                        var pins = 0;
                        int.TryParse(jsonHand.GetJTokenValue(token, "follower_count"), out followerCount);
                        int.TryParse(jsonHand.GetJTokenValue(token, "board_count"), out boards);
                        int.TryParse(jsonHand.GetJTokenValue(token, "pin_count"), out pins);

                        PinterestUser objPinterestUser = new PinterestUser
                        {
                            Username = jsonHand.GetJTokenValue(token, "username"),
                            FollowersCount = followerCount,
                            FullName = jsonHand.GetJTokenValue(token, "full_name"),
                            BoardsCount = boards,
                            PinsCount = pins,
                            ProfilePicUrl = jsonHand.GetJTokenValue(token, "image_xlarge_url"),
                            UserId = jsonHand.GetJTokenValue(token, "id"),
                            IsFollowedByMe = jsonHand.GetJTokenValue(token, "explicitly_followed_by_me") == "True"
                        };

                        UsersList.Add(objPinterestUser);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            

            if (UsersList.Count > 0)
                Success = true;
        }

        private void getdatafrombrowser(string response)
        {
            //get list of data 
            var listdata = HtmlAgilityHelper.GetListInnerHtmlFromClassName(response, "Hvp snW zI7 iyn Hsu");
            foreach(var data in listdata)
            {                
                var isfollowedbyme = HtmlAgilityHelper.GetStringInnerTextFromClassName(data, "tBJ dyH iFc yTZ pBj tg7 mWe");
                bool isfollowed = false;
                if (!isfollowedbyme.Contains("Follow"))
                    isfollowed = true;
                
                PinterestUser objPinterestUser = new PinterestUser
                {
                    Username = Utilities.GetBetween(data, "href=\"/", "/"),
                    IsFollowedByMe=isfollowed,
                    ProfilePicUrl= Utilities.GetBetween(data, "src=\"", "\"><"),
                    FullName= HtmlAgilityHelper.GetStringInnerTextFromClassName(data, "tBJ dyH iFc yTZ pBj zDA IZT mWe z-6")

                };

                UsersList.Add(objPinterestUser);

            }
        }

        public bool HasMoreResults { get; set; }

        public string BookMark { get; set; }

        public List<PinterestUser> UsersList { get; } = new List<PinterestUser>();
    }
}