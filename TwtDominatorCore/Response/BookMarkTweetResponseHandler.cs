using DominatorHouseCore.Interfaces;
using System;

namespace TwtDominatorCore.Response
{
    public class BookMarkTweetResponseHandler: TdResponseHandler
    {
        public BookMarkTweetResponseHandler(IResponseParameter response):base(response)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                Success = handler.GetJTokenValue(obj, "data", "tweet_bookmark_put")?.ToLower() == "done";
            }
            catch { }
        }
    }
}
