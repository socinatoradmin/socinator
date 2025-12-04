using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtEngage;

namespace TwtDominatorUI.TDViews.Tools.Comment
{
    public class CommentConfigBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
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
    public partial class CommentConfiguration : CommentConfigBase
    {
        public CommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                Enums.TdMainModule.TwtEngage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}