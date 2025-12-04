using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Utility;
using System;

namespace PinDominatorCore.Response
{
    public class BoardInfoPtResponseHandler : PdResponseHandler
    {
        public BoardInfoPtResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {  
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(response.Response);
                var boards = RequestHeaderDetails.jToken;
                if (!boards.HasValues)
                {
                    Success = false;
                    return;
                }
                var followerCount = handler.GetJTokenValue(boards, "follower_count");    
                var n = 0;
                int.TryParse(followerCount, out n);
                FollowerCount = n;
                var pinsCount = handler.GetJTokenValue(boards, "pin_count");
                int.TryParse(pinsCount, out n);
                PinsCount = n;                
                BoardId = PdUtility.AssignNA(handler.GetJTokenValue(boards, "id"));                                         
                Url = PdUtility.AssignNA(handler.GetJTokenValue(boards, "url"));
                BoardName = PdUtility.AssignNA(handler.GetJTokenValue(boards, "name")?.Replace("  ", " "));
                BoardDescription = PdUtility.AssignNA(handler.GetJTokenValue(boards, "description"));
                UserName = PdUtility.AssignNA(handler.GetJTokenValue(boards, "owner", "username"));
                UserId = PdUtility.AssignNA(handler.GetJTokenValue(boards, "owner", "id"));
                FullName = PdUtility.AssignNA(handler.GetJTokenValue(boards, "owner", "full_name"));
                ProfilePicUrl =PdUtility.AssignNA(handler.GetJTokenValue(boards, "owner", "image_medium_url"));
                HasProfilePicture = ProfilePicUrl != "N/A";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public int FollowerCount { get; set; }

        public int PinsCount { get; set; }

        public string Url { get; set; }

        public string BoardName { get; set; }

        public string BoardDescription { get; set; }

        public string BoardId { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }
        public string FullName { get; set; }
        public bool HasProfilePicture { get; set; }
        public string ProfilePicUrl { get; set; }

        public static implicit operator PinterestBoard(BoardInfoPtResponseHandler board)
        {
            var pinterestBoard = new PinterestBoard
            {
                FollowersCount = board.FollowerCount,
                PinsCount = board.PinsCount,
                BoardUrl = board.Url,
                Id = board.BoardId,
                BoardDescription = board.BoardDescription,
                BoardName = board.BoardName,
                UserId = board.UserId,
                UserName = board.UserName,
                FullName=board.FullName,
                ProfilePicUrl=board.ProfilePicUrl,
                HasProfilePicture=board.HasProfilePicture
            };
            return pinterestBoard;
        }
    }
}