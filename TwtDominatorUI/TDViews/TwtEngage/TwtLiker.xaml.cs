using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtEngage;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtEngage
{
    public class LikerBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for TwtLiker.xaml
    /// </summary>
    public partial class TwtLiker : LikerBase
    {
        private TwtLiker()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: LikerHeader,
                footer: LikerFooter,
                queryControl: LikerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Like,
                moduleName: TdMainModule.TwtEngage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtLikerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtLikerKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject creation

        private static TwtLiker objLike;

        public static TwtLiker GetSingletonObjectTwtLiker()
        {
            return objLike ?? (objLike = new TwtLiker());
        }

        #endregion
    }
}