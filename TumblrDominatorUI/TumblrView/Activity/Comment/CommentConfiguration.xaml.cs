using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Engage;

namespace TumblrDominatorUI.TumblrView.Activity.Comment
{
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
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
    ///     Interaction logic for CommentConfiguration.xaml
    /// </summary>
    public partial class CommentConfiguration
    {
        private CommentConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                Enums.TmbMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentConfigSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
        }

        #region Object creation and INotifyPropertyChanged Implementation

        private static CommentConfiguration CurrentCommentConfiguration { get; set; }

        public static CommentConfiguration GetSingeltonObjectCommentConfiguration()
        {
            return CurrentCommentConfiguration ?? (CurrentCommentConfiguration = new CommentConfiguration());
        }

        #endregion
    }
}