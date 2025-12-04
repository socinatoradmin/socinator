using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.LDViewModel.Scraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Scraper
{
    /// <summary>
    ///     Interaction logic for MessageConversationScraperConfiguration.xaml
    /// </summary>
    public class MessageConversationScraperConfigurationBase : ModuleSettingsUserControl<
        MessageConversationScraperViewModel, MessageConversationScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MessageConversationScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<MessageConversationScraperModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new MessageConversationScraperViewModel();
                ObjViewModel.MessageConversationScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }

    public partial class MessageConversationScraperConfiguration : MessageConversationScraperConfigurationBase
    {
        public MessageConversationScraperConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AttachmnetsMessageScraper,
                LdMainModules.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.MessageConversationAttachmentVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.MessageConversationAttachmentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static MessageConversationScraperConfiguration CurrentMessageConversationScraperConfiguration
        {
            get;
            set;
        }

        public static MessageConversationScraperConfiguration
            GetSingeltonObjectMessageConversationScraperConfiguration()
        {
            return CurrentMessageConversationScraperConfiguration ?? (CurrentMessageConversationScraperConfiguration =
                       new MessageConversationScraperConfiguration());
        }
    }
}