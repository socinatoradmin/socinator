using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;

namespace TwtDominatorUI.TDViews.Tools.Tweet
{
    public class TweetToConfigBase : ModuleSettingsUserControl<TweetToViewModel, TweetToModel>
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
                    ObjViewModel.TweetToModel =
                        templateModel.ActivitySettings.GetActivityModel<TweetToModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new TweetToViewModel();

                ObjViewModel.TweetToModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for TweetToConfiguration.xaml
    /// </summary>
    public partial class TweetToConfiguration : TweetToConfigBase
    {
        public TweetToConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            //ListQueryType

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.TweetTo,
                Enums.TdMainModule.TwtBlaster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: TweetToConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TweetToVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TweetToKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }

        private void menu_DeleteSingleImage_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.RemoveSelectedMediaExecute(sender);
        }
    }
}