using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    public class EditCommentConfigurationBase : ModuleSettingsUserControl<EditCommentViewModel, EditCommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check for comment value
            if (Model.CommentDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.EditCommentModel =
                        JsonConvert.DeserializeObject<EditCommentModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new EditCommentViewModel();

                ObjViewModel.EditCommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for EditCommentConfiguration.xaml
    /// </summary>
    public partial class EditCommentConfiguration : EditCommentConfigurationBase
    {
        public EditCommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.EditComment,
                Enums.RdMainModule.GrowComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            //VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
        }

        public static EditCommentConfiguration CurrentEditCommentConfiguration { get; set; }

        public static EditCommentConfiguration GetSingeltonObjectEditCommentConfiguration()
        {
            return CurrentEditCommentConfiguration ??
                   (CurrentEditCommentConfiguration = new EditCommentConfiguration());
        }
    }
}