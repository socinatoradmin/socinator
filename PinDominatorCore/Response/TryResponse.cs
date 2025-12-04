using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;

namespace PinDominatorCore.Response
{
    public class TryResponse : PdResponseHandler
    {
        public TryResponse(IResponseParameter response) : base(response)
        {
            if(string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            JsonHandler jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);

            try
            {
                if (jsonHand.GetJToken("resource_response", "data").HasValues)
                    Success = true;
                else
                    Success = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}