namespace DominatorHouseCore.Interfaces
{
    public interface IPostComment
    {
        string CommentId { get; set; }
        string Text { get; set; }
        string ContentType { get; set; }
        string UserId { get; set; }
        string Status { get; set; }
    }
}