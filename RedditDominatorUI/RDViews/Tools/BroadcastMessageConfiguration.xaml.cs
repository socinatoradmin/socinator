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
    public class
        BroadcastMessageConfigurationBase : ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>
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
                    ObjViewModel.BrodcastMessageModel =
                        JsonConvert.DeserializeObject<BrodcastMessageModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new BrodcastMessageViewModel();

                ObjViewModel.BrodcastMessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BroadcastMessageConfiguration.xaml
    /// </summary>

    // ReSharper disable once InheritdocConsiderUsage
    public partial class BroadcastMessageConfiguration
    {
        public BroadcastMessageConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                Enums.RdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BroadcastMessageConfigurationSearchControl
            );
        }

        private static BroadcastMessageConfiguration CurrentBroadcastMessageConfiguration { get; set; }

        public static BroadcastMessageConfiguration GetSingeltonObjectBroadcastMessageConfiguration()
        {
            return CurrentBroadcastMessageConfiguration ??
                   (CurrentBroadcastMessageConfiguration = new BroadcastMessageConfiguration());
        }
    }
}