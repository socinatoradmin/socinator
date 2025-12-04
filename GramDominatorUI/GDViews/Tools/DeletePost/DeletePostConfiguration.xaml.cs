using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.DeletePost;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.DeletePost
{
    public class DeletePostConfigurationBase : ModuleSettingsUserControl<DeletePostViewModel, DeletePostModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check AutoFollow.Unfollow
            if (!Model.ChkDeletePostWhichIsPostedBySoftware && !Model.ChkDeletePostWhichIsPostedByOutsideSoftware)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please select atleast one option");
                return false;
            }

            return true;
            // return base.ValidateCampaign();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DeletePostModel =
                        templateModel.ActivitySettings.GetActivityModel<DeletePostModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new DeletePostViewModel();
                ObjViewModel.DeletePostModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DeletePostConfiguration.xaml
    /// </summary>
    public partial class DeletePostConfiguration : DeletePostConfigurationBase
    {
        private DeletePostConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DeletePost,
                Enums.GdMainModule.Poster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.DeletePostVideoTutorialsLink;
        }

        private static DeletePostConfiguration CurrentDeletePostConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF DeletePostConfiguration
        /// </summary>
        /// <returns></returns>
        public static DeletePostConfiguration GetSingeltonObjectDeletePostConfiguration()
        {
            return CurrentDeletePostConfiguration ?? (CurrentDeletePostConfiguration = new DeletePostConfiguration());
        }
    }
}