using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{

    public class EventGuestsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public EventGuestsResponseHandler(IResponseParameter responseParameter, EventGuestType eventGuestType)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            try
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters
                {
                    ListUser = new List<FacebookUser>()
                };


                var jsonResponse = "[" + Regex.Replace(responseParameter.Response, "for \\(;;\\);", string.Empty) + "]";

                GetAllProfileDetails(jsonResponse, eventGuestType);

                Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetAllProfileDetails(string jsonResponse, EventGuestType eventGuestType)
        {
            try
            {
                JArray objArray = JArray.Parse(jsonResponse);

                var guestType = eventGuestType == EventGuestType.Interested ? "watched" : eventGuestType.ToString().ToLower();

                var friendList = objArray[0]["payload"][guestType]["sections"][2][1];

                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["uniqueID"]
                                                           let name = token["title"]
                                                           let photo = token["photo"]
                                                           let isFriend = token["auxiliaryData"]["isFriend"]
                                                           let isFriendRequestSent = token["auxiliaryData"]["friendRequestSent"]
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = FdConstants.FbHomeUrl + userId,
                                                               IsFriendAccount = bool.Parse(isFriend.ToString())

                                                           }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }
    }

}
