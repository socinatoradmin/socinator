using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class GroupJoinerResponseHandler : LdResponseHandler
    {
        public GroupJoinerResponseHandler(IResponseParameter response)
            : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"viewerGroupMembership\"")|| response.Response.Contains("\"groupPostNotificationsEdgeSettingUrn\"") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;
            try
            {
                if (RespJ == null && response.Response.Contains("<!DOCTYPE html>"))
                {
                    if (Response.Response.Contains("upload-IMAGE"))
                        Status = "MEMBER";
                    else if (Response.Response.Contains("Cancel Request"))
                        Status = "REQUEST_PENDING";
                    else if (response.Response.Contains("Withdraw Request"))
                        Status = "Withdraw_Request";
                    else
                        Status = "N/A";

                    return;
                }

                var checkStatusArray = JArray.Parse(RespJ["included"].ToString());
                Status = checkStatusArray[0]["status"].ToString();
            }
            catch (Exception)
            {
                //ex.DebugLog();
                Status = "N/A";
            }
        }


        public string Status { get; }
    }
}