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
    public class BoardCollaboratorInfo : BindableBase
    {
        private string _account;
        private string _boardUrl;

        private string _email;

        private int _selectedIndex;

        public string BoardUrl
        {
            get => _boardUrl;
            set
            {
                if (_boardUrl != null && _boardUrl == value)
                    return;
                SetProperty(ref _boardUrl, value);
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != null && _email == value)
                    return;
                SetProperty(ref _email, value);
            }
        }

        public string Account
        {
            get => _account;
            set
            {
                if (_account != null && _account == value)
                    return;
                SetProperty(ref _account, value);
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }
    }

    public interface ISendBoardInvitationViewModel
    {
    }

    public class SendBoardInvitationViewModel : StartupBaseViewModel, ISendBoardInvitationViewModel
    {
        private ObservableCollectionBase<BoardCollaboratorInfo> _boardCollaboratorDetails =
            new ObservableCollectionBase<BoardCollaboratorInfo>();

        private BoardCollaboratorInfo _currentBoardCollaborator = new BoardCollaboratorInfo();

        private List<BoardCollaboratorInfo> _listBoardCollaboratorDetails = new List<BoardCollaboratorInfo>();

        private List<string> _listBoardCollaborators = new List<string>();

        public ObservableCollectionBase<BoardCollaboratorInfo> ListBoardCollaboratorInfo =
            new ObservableCollectionBase<BoardCollaboratorInfo>();

        public SendBoardInvitationViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.SendBoardInvitation});
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendBoardInvitationPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfSendBoardInvitationPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfSendBoardInvitationPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfSendBoardInvitationPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeySendBoardInvitationPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddBoardCollaboratorCommand = new BaseCommand<object>(sender => true, AddBoardCollaborator);
            DeleteBoardCollaboratorCommand = new BaseCommand<object>(sender => true, DeleteBoardCollaborator);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportFromCsv);
        }

        public ICommand AddBoardCollaboratorCommand { get; set; }
        public ICommand DeleteBoardCollaboratorCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }

        public ObservableCollectionBase<BoardCollaboratorInfo> BoardCollaboratorDetails
        {
            get => _boardCollaboratorDetails;
            set
            {
                if (_boardCollaboratorDetails != null && _boardCollaboratorDetails == value)
                    return;
                SetProperty(ref _boardCollaboratorDetails, value);
            }
        }

        public List<BoardCollaboratorInfo> ListBoardCollaboratorDetails
        {
            get => _listBoardCollaboratorDetails;
            set
            {
                if (_listBoardCollaboratorDetails != null && _listBoardCollaboratorDetails == value)
                    return;
                SetProperty(ref _listBoardCollaboratorDetails, value);
            }
        }

        public BoardCollaboratorInfo CurrentBoardCollaborator
        {
            get => _currentBoardCollaborator;
            set
            {
                if (_currentBoardCollaborator != null && _currentBoardCollaborator == value)
                    return;
                SetProperty(ref _currentBoardCollaborator, value);
            }
        }

        public List<string> listBoardCollaborators
        {
            get => _listBoardCollaborators;
            set
            {
                if (_listBoardCollaborators != null && _listBoardCollaborators == value)
                    return;
                SetProperty(ref _listBoardCollaborators, value);
            }
        }

        private void ValidateAndNevigate()
        {
            if (BoardCollaboratorDetails.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one board Collaborator.");
                return;
            }

            NavigateNext();
        }

        private void AddBoardCollaborator(object sender)
        {
            try
            {
                if (BoardCollaboratorDetails.Count == 0)
                    BoardCollaboratorDetails = new ObservableCollectionBase<BoardCollaboratorInfo>();

                var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
                CurrentBoardCollaborator.Account = viewModel.SelectAccount.AccountBaseModel.UserName;

                if (ListBoardCollaboratorDetails.Count > 0)
                {
                    ListBoardCollaboratorDetails.ForEach(board =>
                    {
                        if (!string.IsNullOrEmpty(board.Account) && !string.IsNullOrEmpty(board.BoardUrl) &&
                            !string.IsNullOrEmpty(board.Email))
                            BoardCollaboratorDetails.Add(new BoardCollaboratorInfo
                            {
                                BoardUrl = board.BoardUrl,
                                Email = board.Email,
                                Account = board.Account
                            });
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(CurrentBoardCollaborator.BoardUrl) &&
                        !string.IsNullOrEmpty(CurrentBoardCollaborator.Email))
                        BoardCollaboratorDetails.Add(new BoardCollaboratorInfo
                        {
                            BoardUrl = CurrentBoardCollaborator.BoardUrl,
                            Email = CurrentBoardCollaborator.Email,
                            Account = CurrentBoardCollaborator.Account
                        });
                    else
                        return;
                }

                ListBoardCollaboratorDetails.Clear();
                CurrentBoardCollaborator.BoardUrl = string.Empty;
                CurrentBoardCollaborator.Email = string.Empty;
                CurrentBoardCollaborator.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteBoardCollaborator(object sender)
        {
            try
            {
                var boardCollaboratorInfo = sender as BoardCollaboratorInfo;
                BoardCollaboratorDetails.Remove(boardCollaboratorInfo);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ImportFromCsv(object sender)
        {
            try
            {
                ListBoardCollaboratorDetails.Clear();
                listBoardCollaborators.Clear();
                listBoardCollaborators.AddRange(FileUtilities.FileBrowseAndReader());
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountFileManager.GetAll(SocialNetworks.Pinterest)
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                    .Select(x => x.AccountBaseModel.UserName).ToList();

                if (listBoardCollaborators.Count != 0)
                {
                    foreach (var board in listBoardCollaborators)
                        try
                        {
                            var boardCollaboratorsdetails = board.Split('\t');
                            var boardCollaboratorInfo = new BoardCollaboratorInfo();
                            boardCollaboratorInfo.BoardUrl = boardCollaboratorsdetails[0];
                            boardCollaboratorInfo.Email = boardCollaboratorsdetails[1];
                            boardCollaboratorInfo.Account = boardCollaboratorsdetails[2];

                            if (!accounts.Contains(boardCollaboratorInfo.Account))
                            {
                                GlobusLogHelper.log.Info($"Account => {boardCollaboratorInfo.Account} is not present.");
                                continue;
                            }

                            ListBoardCollaboratorDetails.Add(boardCollaboratorInfo);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    Dialog.ShowDialog(Application.Current.MainWindow, "Info",
                        "Board Collaborators are ready to add !!");
                    GlobusLogHelper.log.Info("Board Collaborators sucessfully uploaded !!");
                }
                else
                {
                    GlobusLogHelper.log.Info("You did not upload any Board Collaborators !!");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading Board Collaborators !!");
            }
        }
    }
}