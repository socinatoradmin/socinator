using System;
using System.Linq;
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
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
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

            if (ObjViewModel.CommentModel.ManageCommentModel.LstQueries.Count > 0)
                foreach (var item in ObjViewModel.CommentModel.ManageCommentModel.LstQueries)
                    if (item.Content.QueryValue != "All")
                    {
                        string Comment = null;
                        try
                        {
                            Comment = ObjViewModel.CommentModel.LstDisplayManageCommentModel.FirstOrDefault(x =>
                                    x.LstQueries.FirstOrDefault(y =>
                                        y.Content.QueryType == item.Content.QueryType &&
                                        y.Content.QueryValue == item.Content.QueryValue) != null)
                                ?.CommentText;
                        }

                        catch (Exception ex)
                        {
                        }

                        if (Comment == null)
                        {
                            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                                string.Format("LangKeyPleaseInputAtleastOneCommentForQuery".FromResourceDictionary(),
                                    item.Content.QueryType));
                            return false;
                        }
                    }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Comment.xaml
    /// </summary>
    public partial class Comment : CommentBase
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
                moduleName: LdMainModules.Engage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
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