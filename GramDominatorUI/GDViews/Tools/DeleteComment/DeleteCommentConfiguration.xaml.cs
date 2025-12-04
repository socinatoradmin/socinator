using System;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.DeleteComment;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.Tools.DeleteComment
{
    public class DeleteCommentConfigurationBase : ModuleSettingsUserControl<DeleteCommentViewModel, DeleteCommentModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DeleteCommentModel =
                        JsonConvert.DeserializeObject<DeleteCommentModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new DeleteCommentViewModel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DeleteCommentConfiguration.xaml
    /// </summary>
    public partial class DeleteCommentConfiguration : DeleteCommentConfigurationBase
    {
        public DeleteCommentConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DeleteComment,
                GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            // VideoTutorialLink = ConstantHelpDetails.dele;
        }

        private static DeleteCommentConfiguration CurrentDeleteCommentConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECT OF DeleteCommentConfiguration
        /// </summary>
        /// <returns></returns>
        public static DeleteCommentConfiguration GetSingeltonObjectDeleteCommentConfiguration()
        {
            return CurrentDeleteCommentConfiguration ??
                   (CurrentDeleteCommentConfiguration = new DeleteCommentConfiguration());
        }


        private void AccountGrowthHeader_SelectionChangedEvent(object sender, RoutedEventArgs e)
        {
            // Getting details of account
            var accounts = InstanceProvider.GetInstance<IAccountsFileManager>().GetAll();

            //Getting details of account having the user name  as selected account
            var accountDetails =
                accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == AccountGrowthHeader.SelectedItem);

            if (accountDetails == null)
                return;

            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[accountDetails.AccountId, ActivityType.DeleteComment];
            //var moduleConfiguration = accountDetails.ActivityManager.LstModuleConfiguration
            //       .FirstOrDefault(y => y.ActivityType == ActivityType.DeleteComment);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(moduleConfiguration?.TemplateId);

            SetModuleValues(false, templateDetails);

            MainGrid.DataContext = ObjViewModel.DeleteCommentModel;
        }


        private void AccountGrowthHeader_SaveClick(object sender, RoutedEventArgs e)
        {
            // Getting details of account
            var accounts = InstanceProvider.GetInstance<IAccountsFileManager>().GetAll();
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            //Getting details of account having the user name  as selected account
            var selectedAccountDetails =
                accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == AccountGrowthHeader.SelectedItem);

            if (selectedAccountDetails == null)
                return;

            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[selectedAccountDetails.AccountId, ActivityType.DeleteComment];

            var accountstemplateId = moduleConfiguration?.TemplateId;

            if (string.IsNullOrEmpty(accountstemplateId))
                jobActivityConfigurationManager.AddOrUpdate(selectedAccountDetails.AccountId,
                    ActivityType.DeleteComment, new ModuleConfiguration
                    {
                        ActivityType = ActivityType.DeleteComment,
                        TemplateId = AddNewTemplate(ObjViewModel.Model, AccountGrowthHeader.SelectedItem,
                            ActivityType.DeleteComment, selectedAccountDetails)
                    });

            // Updating existing template
            else
                // TemplatesFileManager.UpdateActivitySettings(accountstemplateId,
                //JsonConvert.SerializeObject((DeleteCommentModel)ObjViewModel.Model));

                templatesFileManager.UpdateActivitySettings(accountstemplateId,
                    JsonConvert.SerializeObject(ObjViewModel.Model));

            selectedAccountDetails.Token.ThrowIfCancellationRequested();
            SocinatorAccountBuilder.Instance(selectedAccountDetails.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(selectedAccountDetails.AccountBaseModel)
                .AddOrUpdateCookies(selectedAccountDetails.Cookies)
                .SaveToBinFile();
            Dialog.ShowDialog(Application.Current.MainWindow, "Success",
                "Successfully Saved !!!");
        }

        private string AddNewTemplate(DeleteCommentModel moduleToSave, string userName, ActivityType moduleType,
            DominatorAccountModel selectedAccount)
        {
            return TemplateModel.SaveTemplate(moduleToSave,
                moduleType.ToString(), SocialNetworks.Instagram,
                userName + "_" + moduleType + "_Template");
        }

        private void TglStatus_OnIsCheckedChanged(object sender, EventArgs e)
        {
            //base.ScheduleJobFromGrowthMode(ObjViewModel.Model.IsAccountGrowthActive, AccountGrowthHeader.SelectedItem,
            //    SocialNetworks.Instagram);
        }
    }
}