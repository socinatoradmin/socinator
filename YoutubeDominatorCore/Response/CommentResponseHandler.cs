using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorCore.Response
{
    public class CommentResponseHandler : YdResponseHandler
    {
        public CommentResponseHandler()
        {
        }

        public CommentResponseHandler(IResponseParameter response)
        {
            Utilities.UpdateTestResponseDataFile(response.Response,
                YdStatic.MyCoreLocation() + @".UnitTests\TestData\CommentDoneResponse.json");

            Success = response.Response.Contains("\"status\":\"STATUS_SUCCEEDED\"");
            CommentId = Utilities.GetBetween(response.Response, "\"commentId\":\"", "\"");
            if (!Success)
            {
                /*Just For Checking*/
            }
        }

        public string CommentId { get; set; }
    }
}