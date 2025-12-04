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
    public class SubscriberConfigurationBase : ModuleSettingsUserControl<SubscribeViewModel, SubscribeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.SubscribeModel =
                        JsonConvert.DeserializeObject<SubscribeModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new SubscribeViewModel();

                ObjViewModel.SubscribeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for SubscriberConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class SubscriberConfiguration
    {
        public SubscriberConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Subscribe,
                Enums.RdMainModule.GrowSubscribe.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: SubScriberConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.SubscribeVideoTutorialsLink;
        }

        private static SubscriberConfiguration CurrentSubscriberConfiguration { get; set; }

        public static SubscriberConfiguration GetSingeltonObjectSubscriberConfiguration()
        {
            return CurrentSubscriberConfiguration ?? (CurrentSubscriberConfiguration = new SubscriberConfiguration());
        }
    }
}