using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinTryCommenter;

namespace PinDominator.PDViews.Tools.Comments
{
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (ObjViewModel.CommentModel.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one query.");
                return false;
            }

            if (ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one Comment.");
                return false;
            }

            if (ObjViewModel.CommentModel.ChkCommentOnUserLatestPostsChecked &&
                ObjViewModel.CommentModel.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after comment action.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentModel =
                        templateModel.ActivitySettings.GetActivityModel<CommentModel>(ObjViewModel.Model);
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
        private readonly QueryContent _queryContent = new QueryContent {Content = new QueryInfo {QueryValue = "All"}};

        private CommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                Enums.PdMainModule.TryComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentConfigSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.TryVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.TryKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            ObjViewModel.CommentModel.ManageCommentModel.LstQueries.Add(_queryContent);
        }

        private static CommentConfiguration CurrentCommentConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectCommentConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static CommentConfiguration GetSingletonObjectCommentConfiguration()
        {
            return CurrentCommentConfiguration ?? (CurrentCommentConfiguration = new CommentConfiguration());
        }
    }
}