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

namespace TumblrDominatorUI.TumblrView.Activity.Like
{
    public class LikeConfigurationBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
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
                    ObjViewModel.LikeModel =
                        JsonConvert.DeserializeObject<LikeModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new LikeViewModel();
                ObjViewModel.LikeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for LikeConfiguration.xaml
    /// </summary>
    public partial class LikeConfiguration
    {
        private LikeConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Like,
                Enums.TmbMainModule.Engage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
        }


        #region Object creation

        private static LikeConfiguration CurrentLike { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static LikeConfiguration GetSingeltonObjectLikeConfiguration()
        {
            return CurrentLike ?? (CurrentLike = new LikeConfiguration());
        }

        #endregion
    }
}