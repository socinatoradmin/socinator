using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class EditCommentViewModel : BindableBase
    {
        private EditCommentModel _editCommentModel = new EditCommentModel();

        public EditCommentViewModel()
        {
            EditCommentModel.ListQueryType.Clear();

            if (EditCommentModel.ListQueryType.Count == 0)
                EditCommentModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.CustomUrl.GetDescriptionAttr())?.ToString());

            EditCommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            EditCommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "All",
                    QueryValue = "All"
                }
            });

            AddCommentCommand = new BaseCommand<object>(sender => true, AddComment);
            DeleteCommentCommand = new BaseCommand<object>(sender => true, DeleteComment);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportFromCsv);
        }


        public EditCommentModel Model => EditCommentModel;

        public EditCommentModel EditCommentModel
        {
            get => _editCommentModel;
            set
            {
                if (_editCommentModel == null && _editCommentModel == value)
                    return;
                SetProperty(ref _editCommentModel, value);
            }
        }


        #region Commands

        public ICommand AddCommentCommand { get; set; }
        public ICommand DeleteCommentCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }

        private ModuleSettingsUserControl<EditCommentViewModel, EditCommentModel> moduleSettingsUserControl
        {
            get;
            set;
        }

        #endregion

        #region Methods

        private void AddComment(object sender)
        {
            try
            {
                moduleSettingsUserControl = sender as ModuleSettingsUserControl<EditCommentViewModel, EditCommentModel>;

                if (moduleSettingsUserControl == null)
                    return;
                if (EditCommentModel.CommentDetails.Count == 0)
                    EditCommentModel.CommentDetails = new ObservableCollectionBase<EditCommentInfo>();

                var editCommentControl = EditCommentModel.EditCommentInfo;

                if (moduleSettingsUserControl._accountGrowthModeHeader != null)
                    editCommentControl.Accounts = moduleSettingsUserControl._accountGrowthModeHeader.SelectedItem;

                if (EditCommentModel.LstCommentDetails.Count == 0 &&
                    (string.IsNullOrEmpty(editCommentControl.Accounts) ||
                     string.IsNullOrEmpty(editCommentControl.Message?.Trim()) ||
                     string.IsNullOrEmpty(editCommentControl.EditCommentUrl?.Trim())))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Error",
                        "Please add atleast one CommentDetails.");
                    return;
                }

                //CommentDetails will stored here by import
                if (EditCommentModel.LstCommentDetails.Count > 0)
                    EditCommentModel.LstCommentDetails.ForEach(x =>
                    {
                        if (EditCommentModel.LstCommentDetails.Any(y => y.Accounts == x.Accounts &&
                                                                        y.EditCommentUrl.Contains(x.EditCommentUrl)))
                            EditCommentModel.CommentDetails.Add(new EditCommentInfo
                            {
                                EditCommentUrl = x.EditCommentUrl,
                                Message = x.Message,
                                Accounts = x.Accounts
                            });
                    });
                //CommentDetails will stored here by user i/p
                else if (!EditCommentModel.CommentDetails.Any(x => x.Accounts == editCommentControl.Accounts
                                                                   && x.EditCommentUrl.Contains(editCommentControl
                                                                       .EditCommentUrl)))
                    EditCommentModel.CommentDetails.Add(new EditCommentInfo
                    {
                        EditCommentUrl = editCommentControl.EditCommentUrl,
                        Message = editCommentControl.Message,
                        Accounts = editCommentControl.Accounts
                    });
                else
                    return;

                editCommentControl.EditCommentUrl = string.Empty;
                editCommentControl.Message = string.Empty;
                editCommentControl.EditCommentUrl = string.Empty;
                editCommentControl.SelectedIndex = 1;

                var lstAccount = EditCommentModel.CommentDetails.Select(x => x.Accounts).Distinct().ToList();
                moduleSettingsUserControl.FooterControl_OnSelectAccountChanged(lstAccount);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteComment(object sender)
        {
            try
            {
                var commentDetails = sender as EditCommentInfo;
                EditCommentModel.CommentDetails.Remove(commentDetails);
                var lstAccount = EditCommentModel.CommentDetails.Select(x => x.Accounts).Distinct().ToList();
                moduleSettingsUserControl.FooterControl_OnSelectAccountChanged(lstAccount);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ImportFromCsv(object obj)
        {
            try
            {
                EditCommentModel.LstCommentDetails.Clear();
                EditCommentModel.LstImportComment.Clear();
                EditCommentModel.LstImportComment.AddRange(FileUtilities.FileBrowseAndReader());
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountFileManager.GetAll(SocialNetworks.Reddit)
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                    .Select(x => x.AccountBaseModel.UserName).ToList();

                if (EditCommentModel.LstImportComment.Count != 0)
                {
                    foreach (var commentFromFile in EditCommentModel.LstImportComment)
                        try
                        {
                            var commentDetails = commentFromFile.Split('\t').ToList();
                            var commentInfo = new EditCommentInfo();
                            commentInfo.EditCommentUrl = commentDetails[0];
                            commentInfo.Message = commentDetails[1];
                            commentInfo.Accounts = commentDetails[2];

                            if (!accounts.Contains(commentInfo.Accounts))
                            {
                                GlobusLogHelper.log.Info($"Account => {commentInfo.Accounts} is not present.");
                                continue;
                            }

                            EditCommentModel.LstCommentDetails.Add(commentInfo);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    Dialog.ShowDialog(Application.Current.MainWindow, "Info",
                        "Comment Details are ready to add !!");
                    GlobusLogHelper.log.Info("Comment Details Sucessfully Uploaded !!");
                }
                else
                {
                    GlobusLogHelper.log.Info("You did not upload any Comment Details !!");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading Comment Details !!");
            }
        }

        #endregion
    }
}