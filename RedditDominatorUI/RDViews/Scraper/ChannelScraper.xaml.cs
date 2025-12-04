using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.UrlScraper
{
    public class ChannelScraperBase : ModuleSettingsUserControl<ChannelScraperViewModel, ChannelScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog("Error", "Please add atleast one query");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for ChannelScraper.xaml
    /// </summary>
    public sealed partial class ChannelScraper : ChannelScraperBase
    {
        public ChannelScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ChannelScraperHeader,
                footer: ChannelScraperFooter,
                queryControl: ChannelScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ChannelScraper,
                moduleName: RdMainModule.ChannelScraper.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.ChannelScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.ChannelScraperVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static ChannelScraper CurrentChannelScraper { get; set; }

        public static ChannelScraper GetSingletonObjectChannelScraper()
        {
            return CurrentChannelScraper ?? (CurrentChannelScraper = new ChannelScraper());
        }
    }
}