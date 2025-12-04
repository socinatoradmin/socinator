using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Engage;

namespace TumblrDominatorUI.TumblrView.Activity.UnLike
{
    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    public class UnLikeConfigurationBase : ModuleSettingsUserControl<UnLikeViewModel, UnLikeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            //if (Model.SavedQueries.Count == 0)
            //{
            //    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(), "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            //    return false;
            //}
            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnLikeModel =
                        JsonConvert.DeserializeObject<UnLikeModel>(templateModel.ActivitySettings);
                else
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
    public partial class UnLikeConfiguration
    {
        public UnLikeConfiguration()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unlike,
                Enums.TmbMainModule.Engage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            VideoTutorialLink = ConstantHelpDetails.UnlikeVideoTutorialLink;
            DialogParticipation.SetRegister(this, this);
        }

        #region Object creation and INotifyPropertyChanged Implementation

        #endregion
    }
}