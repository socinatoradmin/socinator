using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    public class AutoReplyConfigurationBase : ModuleSettingsUserControl<AutoReplyViewModel, AutoReplyModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.MessageModel =
                        JsonConvert.DeserializeObject<AutoReplyModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new AutoReplyViewModel();

                ObjViewModel.MessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
    /// <summary>
    /// Interaction logic for AutoReplyConfiguration.xaml
    /// </summary>
    public partial class AutoReplyConfiguration
    {
        public AutoReplyConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                Enums.RdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
        }
        private static AutoReplyConfiguration CurrentAutoReplyConfiguration { get; set; }

        public static AutoReplyConfiguration GetSingeltonObjectAutoReplyConfiguration()
        {
            return CurrentAutoReplyConfiguration ??
                   (CurrentAutoReplyConfiguration = new AutoReplyConfiguration());
        }
    }
}
