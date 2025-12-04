namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FacebookAdsDetails : FacebookPostDetails
    {
        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string LowerAge { get; set; } = string.Empty;

        public string UpperAge { get; set; } = string.Empty;


        public string AdViewerLocation { get; set; } = string.Empty;

        public string AdType { get; set; } = string.Empty;

        public string OwnerCategory { get; set; } = string.Empty;

        public string OwnerLocation { get; set; } = string.Empty;


        public string ComposerId { get; set; } = string.Empty;
        public string CallActionType { get; internal set; } = string.Empty;

        public string MaxAge { get; internal set; }

        public string MinAge { get; internal set; }
        public string AdId { get; set; }
        public string[] CommentData { get; set; }

        public int PostIndex { get; set; }

        public bool IsOwnerVerified { get; set; }

    }

    public enum AdType
    {
        FEED = 1,
        SIDE = 2
    }
}
