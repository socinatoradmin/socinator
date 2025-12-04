using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
    public class LikeCommentConfigurationBase : ModuleSettingsUserControl<LikeCommentViewModel, LikeCommentModel>
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

            if (ObjViewModel.LikeCommentModel.IsCheckedLikeSpecificCommentId)
                if (ObjViewModel.LikeCommentModel.SavedQueries.Any(x =>
                    x.QueryTypeEnum == "CustomUrls" &&
                    (x.QueryValue.ToLower().EndsWith("&lc=") || !x.QueryValue.ToLower().Contains("&lc="))))
                {
                    var dialog = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeyUrlsDontHaveCommentId".FromResourceDictionary(),
                        "LangKeyContinue".FromResourceDictionary(), "LangKeyEdit".FromResourceDictionary());
                    if (dialog == MessageDialogResult.Negative)
                        return false;
                }

            if (!ObjViewModel.Model.SavedQueries.Any(x => x.QueryTypeEnum == "Keywords") &&
                Model.VideoFilterModel.SearchVideoFilterModel != null)
            {
                Model.VideoFilterModel.IsCheckedSearchVideoFilter = false;
                Model.VideoFilterModel.SearchVideoFilterModel = null;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.LikeCommentModel =
                        JsonConvert.DeserializeObject<LikeCommentModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new LikeCommentViewModel();

                ObjViewModel.LikeCommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for EngageConfiguration.xaml
    /// </summary>
    public partial class LikeCommentConfiguration
    {
        private static LikeCommentConfiguration _currentLikeCommentConfiguration;

        public LikeCommentConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.LikeComment,
                YdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeCommentSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeCommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource =
                new ObservableCollectionBase<string>(accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.LikeCommentModel;
        }

        public static LikeCommentConfiguration GetSingeltonObjectLikeCommentConfiguration()
        {
            return _currentLikeCommentConfiguration ??
                   (_currentLikeCommentConfiguration = new LikeCommentConfiguration());
        }
    }
}