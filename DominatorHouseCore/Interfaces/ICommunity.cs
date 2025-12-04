namespace DominatorHouseCore.Interfaces
{
    public interface ICommunity
    {
        string UserId { get; set; }

        string Username { get; set; }

        string FullName { get; set; }

        string ProfilePicUrl { get; set; }
    }
}