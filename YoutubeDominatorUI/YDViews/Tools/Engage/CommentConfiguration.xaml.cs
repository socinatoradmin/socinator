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
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
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

            if (ObjViewModel.CommentModel.CommentAsReplyOnlyChecked)
                if (ObjViewModel.CommentModel.SavedQueries.Any(x =>
                    x.QueryTypeEnum == "CustomUrls" &&
                    (x.QueryValue.ToLower().EndsWith("&lc=") || !x.QueryValue.ToLower().Contains("&lc="))))
                {
                    var dialog = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeyUrlsDontHaveCommentId".FromResourceDictionary(),
                        "LangKeyContinue".FromResourceDictionary(), "LangKeyEdit".FromResourceDictionary());
                    if (dialog == MessageDialogResult.Negative)
                        return false;
                }

            if (ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
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
                    ObjViewModel.CommentModel =
                        JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new CommentViewModel();

                ObjViewModel.CommentModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class CommentConfiguration
    {
        private static CommentConfiguration _currentCommentConfiguration;

        public CommentConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                YdMainModule.Comment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;
        }

        public static CommentConfiguration GetSingeltonObjectCommentConfiguration()
        {
            return _currentCommentConfiguration ?? (_currentCommentConfiguration = new CommentConfiguration());
        }
    }
}