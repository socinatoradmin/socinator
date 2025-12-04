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
    public class RemoveVoteConfigurationBase : ModuleSettingsUserControl<RemoveVoteViewModel, RemoveVoteModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        } //RemoveVoteConfiguration

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.RemoveVoteModel =
                        JsonConvert.DeserializeObject<RemoveVoteModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new RemoveVoteViewModel();

                ObjViewModel.RemoveVoteModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for RemoveVoteConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class RemoveVoteConfiguration
    {
        public RemoveVoteConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.RemoveVote,
                Enums.RdMainModule.GrowRemoveVote.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: RemoveVoteConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.RemoveVoteVideoTutorialsLink;
        }

        private static RemoveVoteConfiguration CurrentRemoveVoteConfiguration { get; set; }

        public static RemoveVoteConfiguration GetSingeltonObjectRemoveVoteConfiguration()
        {
            return CurrentRemoveVoteConfiguration ?? (CurrentRemoveVoteConfiguration = new RemoveVoteConfiguration());
        }
    }
}