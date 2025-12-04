using DominatorHouseCore.Interfaces;
using System;
namespace QuoraDominatorCore.Models
{
    public class PostDetails:IUser
    {
        public string PostId {  get; set; }
        public string ActivityType {  get; set; }
        public string PostUrl { get; set; }
        public string PostTitle { get; set; }
        public int CommentCount {  get; set; }
        public int ShareCount {  get; set; }
        public int UpvoteCount {  get; set; }
        public int ViewsCount {  get; set; }
        public string ObjectId {  get; set; }
        public DateTime Created { get; set; }
        public string ViewerVoteType { get; set; }
        public string PostAuthorProfileUrl { get; set; }
        public int PostAuthorFollowerCount { get; set; }
        public string PostAuthorId { get; set; }
        public bool PostAuthorIsFollowing { get; set; }
        public string PostType { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ProfilePicUrl { get; set; }
    }
}
