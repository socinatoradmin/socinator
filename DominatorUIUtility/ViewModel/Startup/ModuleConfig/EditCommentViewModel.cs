using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class EditCommentInfo : BindableBase
    {
        private string _accounts;
        private string _editCommentUrl;

        private string _message;

        private int _selectedIndex;

        public string EditCommentUrl
        {
            get => _editCommentUrl;
            set => SetProperty(ref _editCommentUrl, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
    }

    public interface IEditCommentViewModel
    {
    }

    public class EditCommentViewModel : StartupBaseViewModel, IEditCommentViewModel
    {
        private ObservableCollectionBase<EditCommentInfo> _commentDetails =
            new ObservableCollectionBase<EditCommentInfo>();

        private EditCommentInfo _editCommentInfo = new EditCommentInfo();

        private List<EditCommentInfo> _lstCommentDetails = new List<EditCommentInfo>();

        private List<string> _lstImportComment = new List<string>();

        public EditCommentViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.EditComment});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNavigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyMessagesToNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyMessagesToNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyMessagesToNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyMessageToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMessagesToMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddCommentCommand = new BaseCommand<object>(sender => true, AddComment);
            DeleteCommentCommand = new BaseCommand<object>(sender => true, DeleteComment);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportFromCsv);
        }

        public ObservableCollectionBase<EditCommentInfo> CommentDetails
        {
            get => _commentDetails;
            set => SetProperty(ref _commentDetails, value);
        }

        public EditCommentInfo EditCommentInfo
        {
            get => _editCommentInfo;
            set
            {
                if (_editCommentInfo != null && _editCommentInfo == value)
                    return;
                SetProperty(ref _editCommentInfo, value);
            }
        }

        public List<EditCommentInfo> LstCommentDetails
        {
            get => _lstCommentDetails;
            set
            {
                if (_lstCommentDetails != null && _lstCommentDetails == value)
                    return;
                SetProperty(ref _lstCommentDetails, value);
            }
        }

        public List<string> LstImportComment
        {
            get => _lstImportComment;
            set => SetProperty(ref _lstImportComment, value);
        }

        private void ValidateAndNavigate()
        {
            if (CommentDetails.Count == 0)
            {
                Dialog.ShowDialog("Input Error", "Please Add Comments Details");
                return;
            }

            NavigateNext();
        }

        #region Commands

        public ICommand AddCommentCommand { get; set; }
        public ICommand DeleteCommentCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }

        #endregion

        #region Methods

        private void AddComment(object sender)
        {
            try
            {
                var editCommentControl = sender as EditCommentInfo;

                if (editCommentControl == null)
                    return;
                if (CommentDetails.Count == 0)
                    CommentDetails = new ObservableCollectionBase<EditCommentInfo>();

                var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
                editCommentControl.Accounts = viewModel.SelectAccount.AccountBaseModel.UserName;

                if (LstCommentDetails.Count == 0 && (string.IsNullOrEmpty(editCommentControl.Accounts) ||
                                                     string.IsNullOrEmpty(editCommentControl.Message?.Trim()) ||
                                                     string.IsNullOrEmpty(editCommentControl.EditCommentUrl?.Trim())))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Error",
                        "Please add atleast one CommentDetails.");
                    return;
                }

                //CommentDetails will strored here by import
                if (LstCommentDetails.Count > 0)
                    LstCommentDetails.ForEach(x =>
                    {
                        if (LstCommentDetails.Any(y => y.Accounts == x.Accounts &&
                                                       y.EditCommentUrl.Contains(x.EditCommentUrl)))
                            CommentDetails.Add(new EditCommentInfo
                            {
                                EditCommentUrl = x.EditCommentUrl,
                                Message = x.Message,
                                Accounts = x.Accounts
                            });
                    });
                //CommentDetails will strored here by user i/p
                else if (!CommentDetails.Any(x => x.Accounts == editCommentControl.Accounts
                                                  && x.EditCommentUrl.Contains(editCommentControl.EditCommentUrl)))
                    CommentDetails.Add(new EditCommentInfo
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
                CommentDetails.Remove(commentDetails);
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
                LstCommentDetails.Clear();
                LstImportComment.Clear();
                LstImportComment.AddRange(FileUtilities.FileBrowseAndReader());
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountFileManager.GetAll(SocialNetworks.Reddit)
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                    .Select(x => x.AccountBaseModel.UserName).ToList();

                if (LstImportComment.Count != 0)
                {
                    foreach (var commentFromFile in LstImportComment)
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

                            LstCommentDetails.Add(commentInfo);
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