using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.LDViewModel.Scraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Scraper
{
    /// <summary>
    ///     Interaction logic for MessageConversationScraper.xaml
    /// </summary>
    public class AttachmentsMessageScraperBase : ModuleSettingsUserControl<MessageConversationScraperViewModel,
        MessageConversationScraperModel>
    {
    }

    public partial class AttachmentsMessageScraper : AttachmentsMessageScraperBase
    {
        private static AttachmentsMessageScraper _messageConversationScraper;

        public AttachmentsMessageScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AttachmentsMessageHeader,
                footer: AttachmentsMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AttachmnetsMessageScraper,
                moduleName: LdMainModules.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadcastMessagesKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);

            base.SetDataContext();
        }

        public static AttachmentsMessageScraper GetSingletonMessageConversationScraper()
        {
            return _messageConversationScraper ?? (_messageConversationScraper = new AttachmentsMessageScraper());
        }
    }
}