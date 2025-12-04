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
    public class ReplyConfigurationBase : ModuleSettingsUserControl<ReplyViewModel, ReplyModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check for query value
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one query");
                return false;
            }

            // Check for reply comment value
            if (Model.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one comment");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.ReplyModel =
                        JsonConvert.DeserializeObject<ReplyModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ReplyViewModel();

                ObjViewModel.ReplyModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReplyConfiguration.xaml
    /// </summary>

    // ReSharper disable once InheritdocConsiderUsage
    public partial class ReplyConfiguration
    {
        private QueryContent _queryContent = new QueryContent { Content = new QueryInfo { QueryValue = "All" } };

        public ReplyConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Reply,
                Enums.RdMainModule.GrowReply.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReplyConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.ReplyVideoTutorialsLink;
        }

        private static ReplyConfiguration CurrentReplyConfiguration { get; set; }

        public static ReplyConfiguration GetSingeltonObjectReplyConfiguration()
        {
            return CurrentReplyConfiguration ?? (CurrentReplyConfiguration = new ReplyConfiguration());
        }
    }
}