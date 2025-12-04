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

namespace TwtDominatorUI.TDViews.Tools.UnLike
{
    /// <summary>
    ///     Interaction logic for UnLikeConfiguration.xaml
    /// </summary>
    public class UnLikeConfigurationBase : ModuleSettingsUserControl<UnLikeViewModel, UnLikeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!Model.UnLike.IsLikedTweets && !Model.UnLike.IsCustomTweets)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select at least one source type.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnLikeModel =
                        templateModel.ActivitySettings.GetActivityModel<UnLikeModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new UnLikeViewModel();

                ObjViewModel.UnLikeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for UnLikeConfiguration.xaml
    /// </summary>
    public partial class UnLikeConfiguration : UnLikeConfigurationBase
    {
        public ObservableCollectionBase<string> lstAccounts = null;

        public UnLikeConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unlike,
                Enums.TdMainModule.TwtEngage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtUnlikerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtUnlikerVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}