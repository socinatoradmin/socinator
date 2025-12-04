namespace DominatorHouseCore.Interfaces
{
    public interface IHelpControl
    {
        // To provide the video tutorial link for the particular module
        string VideoTutorialLink { get; set; }

        // To provide the knowledge base link for the particular module
        string KnowledgeBaseLink { get; set; }

        // To provide the contact support link for the particular module
        string ContactSupportLink { get; set; }
    }
}