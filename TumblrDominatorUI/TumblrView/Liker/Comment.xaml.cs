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
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error", "Please provide comment(s)");
                return false;
            }

            return true;
        }

        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Comment.xaml
    /// </summary>
    public sealed partial class Comment
    {
        private Comment()
        {
            InitializeComponent();
            InitializeBaseClass(
                header: CommentHeader,
                footer: CommentFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Comment,
                moduleName: Enums.TmbMainModule.Engage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            //////  ObjViewModel.CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
            ////  {
            ////      Content = new QueryInfo
            ////      {
            ////          QueryValue = "All",
            ////          QueryType = "All"
            ////      }
            ////  });

            DialogParticipation.SetRegister(this, this);
        }

        #region singleton method Creation

        private static Comment CurrentComment { get; set; }

        public static Comment GetSingeltonObjectComment()
        {
            return CurrentComment ?? (CurrentComment = new Comment());
        }

        #endregion
    }
}