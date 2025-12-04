using System;
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
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.Boards
{
    public class SendBoardInvitationViewModel : BindableBase
    {
        private SendBoardInvitationModel _sendBoardInvitationModel = new SendBoardInvitationModel();

        public SendBoardInvitationViewModel()
        {
            SendBoardInvitationModel.JobConfiguration = new JobConfiguration
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

        private ModuleSettingsUserControl<SendBoardInvitationViewModel, SendBoardInvitationModel>
            ModuleSettingsUserControl { get; set; }

        public SendBoardInvitationModel Model => SendBoardInvitationModel;

        public SendBoardInvitationModel SendBoardInvitationModel
        {
            get => _sendBoardInvitationModel;
            set
            {
                if (_sendBoardInvitationModel == null || _sendBoardInvitationModel == value)
                    return;
                SetProperty(ref _sendBoardInvitationModel, value);
            }
        }

        private void AddBoardCollaborator(object sender)
        {
            try
            {
                ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<SendBoardInvitationViewModel, SendBoardInvitationModel>;

                if (SendBoardInvitationModel.BoardCollaboratorDetails.Count == 0)
                    SendBoardInvitationModel.BoardCollaboratorDetails =
                        new ObservableCollectionBase<BoardCollaboratorInfo>();
                if (ModuleSettingsUserControl != null)
                {
                    var sendBoardInvitationControl = ModuleSettingsUserControl.ObjViewModel.SendBoardInvitationModel
                        .BoardCollaboratorInfo;

                    if (SendBoardInvitationModel.ListBoardCollaboratorDetails.Count > 0)
                    {
                        SendBoardInvitationModel.ListBoardCollaboratorDetails.ForEach(board =>
                        {
                            if (!string.IsNullOrEmpty(board.Account) && !string.IsNullOrEmpty(board.BoardUrl) &&
                                !string.IsNullOrEmpty(board.Email))
                                SendBoardInvitationModel.BoardCollaboratorDetails.Add(new BoardCollaboratorInfo
                                {
                                    BoardUrl = board.BoardUrl,
                                    Email = board.Email,
                                    Account = board.Account
                                });
                        });
                    }
                    else
                    {
                        if (ModuleSettingsUserControl._accountGrowthModeHeader != null)
                            sendBoardInvitationControl.Account =
                                ModuleSettingsUserControl._accountGrowthModeHeader.SelectedItem;
                        if (!string.IsNullOrEmpty(sendBoardInvitationControl.Account) &&
                            !string.IsNullOrEmpty(sendBoardInvitationControl.BoardUrl) &&
                            !string.IsNullOrEmpty(sendBoardInvitationControl.Email))
                            SendBoardInvitationModel.BoardCollaboratorDetails.Add(new BoardCollaboratorInfo
                            {
                                BoardUrl = sendBoardInvitationControl.BoardUrl,
                                Email = sendBoardInvitationControl.Email,
                                Account = sendBoardInvitationControl.Account
                            });
                        else
                            return;
                    }

                    SendBoardInvitationModel.ListBoardCollaboratorDetails.Clear();
                    sendBoardInvitationControl.BoardUrl = string.Empty;
                    sendBoardInvitationControl.Email = string.Empty;
                    sendBoardInvitationControl.SelectedIndex = 1;


                    SendBoardInvitationModel.listOfSelectedAccounts = SendBoardInvitationModel.BoardCollaboratorDetails
                        .Select(x => x.Account).Distinct().ToList();
                    ModuleSettingsUserControl.FooterControl_OnSelectAccountChanged(SendBoardInvitationModel
                        .listOfSelectedAccounts);
                }

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
                SendBoardInvitationModel.BoardCollaboratorDetails.Remove(boardCollaboratorInfo);
                SendBoardInvitationModel.listOfSelectedAccounts = SendBoardInvitationModel.BoardCollaboratorDetails
                    .Select(x => x.Account).Distinct().ToList();
                ModuleSettingsUserControl.FooterControl_OnSelectAccountChanged(SendBoardInvitationModel
                    .listOfSelectedAccounts);
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
                SendBoardInvitationModel.ListBoardCollaboratorDetails.Clear();
                SendBoardInvitationModel.listBoardCollaborators.Clear();
                SendBoardInvitationModel.listBoardCollaborators.AddRange(FileUtilities.FileBrowseAndReader());
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountFileManager.GetAll(SocialNetworks.Pinterest)
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                    .Select(x => x.AccountBaseModel.UserName).ToList();

                if (SendBoardInvitationModel.listBoardCollaborators.Count != 0)
                {
                    foreach (var board in SendBoardInvitationModel.listBoardCollaborators)
                        try
                        {
                            var boardCollaboratorsdetails = board.Split('\t');
                            var boardCollaboratorInfo = new BoardCollaboratorInfo();
                            boardCollaboratorInfo.BoardUrl = boardCollaboratorsdetails[0];
                            boardCollaboratorInfo.Email = boardCollaboratorsdetails[1];
                            boardCollaboratorInfo.Account = boardCollaboratorsdetails[2];

                            if (!accounts.Contains(boardCollaboratorInfo.Account))
                            {
                                GlobusLogHelper.log.Info(String.Format("LangSpecificAccountIsNotPresent".FromResourceDictionary(), boardCollaboratorInfo.Account));
                                continue;
                            }

                            SendBoardInvitationModel.ListBoardCollaboratorDetails.Add(boardCollaboratorInfo);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyInfo".FromResourceDictionary(),
                        "LangKeyBoardCollaboratorsAreReadyToAdd".FromResourceDictionary());
                    GlobusLogHelper.log.Info("LangKeyBoardCollaboratorsSucessfullyUploaded".FromResourceDictionary());
                }
                else
                {
                    GlobusLogHelper.log.Info("LangKeyYouDidNotUploadAnyBoardCollaborators".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("LangKeyThereIsErrorInUploadingBoardCollaborators".FromResourceDictionary());
            }
        }
    }
}