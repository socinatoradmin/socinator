using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDViewModel.Engage;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Engage
{
    public class LikeBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class Like : LikeBase
    {
        private Like()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: LikeHeader,
                footer: LikeFooter,
                queryControl: LikeSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Like,
                moduleName: LdMainModules.Engage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region Object creation 

        private static Like CurrentLike { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Like GetSingeltonObjectLike()
        {
            return CurrentLike ?? (CurrentLike = new Like());
        }

        #endregion
    }
}