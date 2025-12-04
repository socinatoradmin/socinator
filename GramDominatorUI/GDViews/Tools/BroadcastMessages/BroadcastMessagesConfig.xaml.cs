using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Instachat;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.BroadcastMessages
{
    public class
        BroadcastMessagesConfigBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error", "Please provide message(s)");
                return false;
            }

            if (ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries.Count > 0)
                foreach (var queryContent in Model.ManageMessagesModel.LstQueries)
                    if (queryContent.Content.QueryValue != "All")
                    {
                        ManageMessagesModel mangeMessagesModel = null;
                        try
                        {
                            mangeMessagesModel = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Where(
                                    x =>
                                        x.SelectedQuery.Any(y =>
                                            y.Content.QueryType == queryContent.Content.QueryType &&
                                            y.Content.QueryValue == queryContent.Content.QueryValue)).ToList()
                                .GetRandomItem();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (mangeMessagesModel == null)
                        {
                            Dialog.ShowDialog(this, "Input Error",
                                $"please input at least one message for Querytype [ {queryContent.Content.QueryType} ] with Queryvalue [ {queryContent.Content.QueryValue} ]");
                            return false;
                        }
                    }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BroadcastMessagesModel =
                        templateModel.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(ObjViewModel.Model,
                            true);
                else
                    ObjViewModel = new BroadcastMessagesViewModel();
                ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BroadcastMessagesTabConfig.xaml
    /// </summary>
    public partial class BroadcastMessagesConfig : BroadcastMessagesConfigBase
    {
        public BroadcastMessagesConfig()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                Enums.GdMainModule.InstaChat.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BroadcastMessagesSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
        }

        private static BroadcastMessagesConfig CurrentBroadcastMessagesConfig { get; set; }

        public static BroadcastMessagesConfig GetSingeltonObjectSendMessageToFollowerConfig()
        {
            return CurrentBroadcastMessagesConfig ?? (CurrentBroadcastMessagesConfig = new BroadcastMessagesConfig());
        }
    }
}