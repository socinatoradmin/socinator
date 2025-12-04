namespace TumblrDominatorCore.Models
{
    public class UserDetails
    {
        public string AuthorizationToken { get; set; } = string.Empty;
        public string PaginationToken { get; set; } = string.Empty;
        public string UUID { get; set; } = string.Empty;
        private static UserDetails Instance;
        public static UserDetails GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new UserDetails();
                return Instance;
            }
        }
    }
}
