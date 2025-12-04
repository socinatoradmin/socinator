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

namespace TwtDominatorUI.TDViews.Tools.Like
{
    public class LikeConfigurationBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
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
                    ObjViewModel.LikeModel =
                        templateModel.ActivitySettings.GetActivityModel<LikeModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
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
    public partial class LikeConfiguration : LikeConfigurationBase
    {
        public ObservableCollectionBase<string> lstAccounts = null;

        public LikeConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Like,
                Enums.TdMainModule.TwtEngage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtLikerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtLikerVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}