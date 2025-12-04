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
using TumblrDominatorCore.ViewModels.Blog;

namespace TumblrDominatorUI.TumblrView.Activity.Reblog
{
    public class ReblogConfigurationBase : ModuleSettingsUserControl<ReblogViewModel, ReblogModel>
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
                    ObjViewModel.ReblogModel =
                        JsonConvert.DeserializeObject<ReblogModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new ReblogViewModel();
                ObjViewModel.ReblogModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReblogConfiguration.xaml
    /// </summary>
    public partial class ReblogConfiguration
    {
        private ReblogConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Reblog,
                Enums.TmbMainModule.Blog.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReblogSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.ReblogVideoTutorialsLink;
        }

        #region Object creation

        private static ReblogConfiguration CurrentReblog { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static ReblogConfiguration GetSingeltonObjectReblogConfiguration()
        {
            return CurrentReblog ?? (CurrentReblog = new ReblogConfiguration());
        }

        #endregion
    }
}