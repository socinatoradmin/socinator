using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.GrowFollower;

namespace QuoraDominatorUI.QDViews.Activity.Follow
{
    public class FollowerConfigurationBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateExtraProperty();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowerModel =
                        JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new FollowerViewModel();


                ObjViewModel.FollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    public partial class FollowConfiguration
    {
        public FollowConfiguration()
        {
            InitializeComponent();
            
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                QdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowerSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
        }

        private static FollowConfiguration CurrentFollowConfiguration { get; set; }

        public static FollowConfiguration GetSingeltonObjectFollowConfiguration()
        {
            return CurrentFollowConfiguration ?? (CurrentFollowConfiguration = new FollowConfiguration());
        }
    }
}