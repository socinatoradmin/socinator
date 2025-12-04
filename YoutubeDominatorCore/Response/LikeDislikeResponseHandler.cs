using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorCore.Response
{
    public class LikeDislikeResponseHandler : YdResponseHandler
    {
        public LikeDislikeResponseHandler()
        {
        }

        public LikeDislikeResponseHandler(IResponseParameter response)
        {
            var isLikeResponse = !response.Response.Contains("Dislike");

            if (isLikeResponse)
                Utilities.UpdateTestResponseDataFile(response.Response,
                    YdStatic.MyCoreLocation() + @".UnitTests\TestData\LikedPostResponse.json");
            else
                Utilities.UpdateTestResponseDataFile(response.Response,
                    YdStatic.MyCoreLocation() + @".UnitTests\TestData\DislikedPostResponse.json");

            Success = new JsonHandler(response.Response).GetElementValue("code").Contains("SUCCESS");
            if (!Success)
                if (isLikeResponse)
                    Utilities.UpdateTestResponseDataFile(response.Response,
                        YdStatic.MyCoreLocation() + @".UnitTests\TestData\LikePostWrongResponse.json");
        }
    }
}