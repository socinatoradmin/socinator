using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Linq;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorCore.YoutubeViewModel.EngageViewModel;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.Engage
{
    public class ReportVideoConfigurationBase : ModuleSettingsUserControl<ReportVideoViewModel, ReportVideoModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.ReportVideoModel.ListReportDetailsModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneReportDetail".FromResourceDictionary());
                return false;
            }

            if (!ObjViewModel.Model.SavedQueries.Any(x => x.QueryTypeEnum == "Keywords") &&
                Model.VideoFilterModel.SearchVideoFilterModel != null)
            {
                Model.VideoFilterModel.IsCheckedSearchVideoFilter = false;
                Model.VideoFilterModel.SearchVideoFilterModel = null;
            }

            return base.ValidateCampaign();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ReportVideoModel =
                        JsonConvert.DeserializeObject<ReportVideoModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ReportVideoViewModel();

                ObjViewModel.ReportVideoModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReportVideoConfiguration.xaml
    /// </summary>
    public partial class ReportVideoConfiguration
    {
        private static ReportVideoConfiguration _currentReportVideoConfiguration;

        public ReportVideoConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ReportVideo,
                YdMainModule.ReportVideo.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReportVideoSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ReportVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ReportVideoKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;
        }

        public static ReportVideoConfiguration GetSingeltonObjectReportVideoConfiguration()
        {
            return _currentReportVideoConfiguration ??
                   (_currentReportVideoConfiguration = new ReportVideoConfiguration());
        }
    }
}