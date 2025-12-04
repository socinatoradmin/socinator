using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class FollowResponseHandler : TdResponseHandler
    {
        public FollowResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success)
                    return;

                Success = !string.IsNullOrEmpty(TdUtility.GetTweetOrUserId(response.Response));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}