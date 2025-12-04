namespace DominatorHouseCore.Interfaces
{
    public interface IUser
    {
        string UserId { get; set; }

        string Username { get; set; }

        string FullName { get; set; }

        string ProfilePicUrl { get; set; }
    }
}