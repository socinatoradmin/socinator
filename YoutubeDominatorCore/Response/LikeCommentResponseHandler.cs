using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorCore.Response
{
    public class LikeCommentResponseHandler : YdResponseHandler
    {
        public LikeCommentResponseHandler()
        {
        }

        public LikeCommentResponseHandler(IResponseParameter response)
        {
            Utilities.UpdateTestResponseDataFile(response.Response,
                YdStatic.MyCoreLocation() + @".UnitTests\TestData\LikingCommentResponse.json");

            Success = response.Response?.Contains("\"code\":\"SUCCESS\"") ?? false;
        }
    }
}