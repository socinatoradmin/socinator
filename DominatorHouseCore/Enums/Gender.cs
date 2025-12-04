namespace DominatorHouseCore.Enums
{
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Unknown = 3,
        Unisex = 4
    }

    public enum ChatMessage
    {
        Sent = 1,
        Received = 2
    }

    public enum ChatMessageType
    {
        Text,
        Media,
        Link,
        TextAndMedia,
        ReelShare,
        Mediashare,
        RavenMedia,
        LiveViewerInvite,
        StoryShare,
        Null
    }
}