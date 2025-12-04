using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Engage;

namespace TumblrDominatorUI.TumblrView.Liker
{
    public class UnLikeBase : ModuleSettingsUserControl<UnLikeViewModel, UnLikeModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!Model.IsChkPostLikedBySoftwareChecked && !Model.IsChkCustomPostsListChecked &&
                !Model.IsChkPostLikedOutsideSoftware)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Unlike source");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public sealed partial class UnLike
    {
        private UnLike()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnLikeHeader,
                footer: UnLikeFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unlike,
                moduleName: Enums.TmbMainModule.Engage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region Object creation 

        private static UnLike CurrentUnLike { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UnLike GetSingeltonObjectUnLike()
        {
            return CurrentUnLike ?? (CurrentUnLike = new UnLike());
        }

        #endregion
    }
}