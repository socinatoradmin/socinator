using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtEngage;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtEngage
{
    public class UnLikerBase : ModuleSettingsUserControl<UnLikeViewModel, UnLikeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!Model.UnLike.IsLikedTweets && !Model.UnLike.IsCustomTweets)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select at least one source type.");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for TwtUnLiker.xaml
    /// </summary>
    public partial class TwtUnLiker : UnLikerBase
    {
        private TwtUnLiker()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnLikerHeader,
                footer: UnLikerFooter,
                // queryControl: UnLikerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Unlike,
                moduleName: TdMainModule.TwtEngage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtUnlikerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtUnlikerKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject creation

        private static TwtUnLiker objUnLike;

        public static TwtUnLiker GetSingletonObjectTwtUnLiker()
        {
            return objUnLike ?? (objUnLike = new TwtUnLiker());
        }

        #endregion
    }
}