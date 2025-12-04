using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtBlaster
{
    public class DeleteBase : ModuleSettingsUserControl<DeleteViewModel, DeleteModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.DeleteSetting.IsChkDeleteTweet && !Model.DeleteSetting.IsChkDeleteComment
                                                      && !Model.DeleteSetting.IsChkUndoRetweet)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Delete source");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for Delete.xaml
    /// </summary>
    public partial class Delete
    {
        private Delete()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                header: DeleteHeader,
                footer: DeleteFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Delete,
                moduleName: TdMainModule.TwtBlaster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.DeleteVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.DeletetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
            SetDataContext();
        }

        private static Delete ObjDelete { get; set; }

        public static Delete GetSingletonObjectDelete()
        {
            return ObjDelete ?? (ObjDelete = new Delete());
        }
    }
}