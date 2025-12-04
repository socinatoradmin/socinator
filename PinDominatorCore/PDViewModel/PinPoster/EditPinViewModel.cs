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
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace PinDominatorCore.PDViewModel.PinPoster
{
    public class EditPinViewModel : BindableBase
    {
        private EditPinModel _editPinModel = new EditPinModel();

        public EditPinViewModel()
        {
            EditPinModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfEditPinsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfEditPinsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfEditPinsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfEditPinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyEditPinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddPinCommand = new BaseCommand<object>(sender => true, AddPin);
            DeletePinCommand = new BaseCommand<object>(sender => true, DeletePin);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportFromCsv);
        }

        public ICommand AddPinCommand { get; set; }
        public ICommand DeletePinCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        private ModuleSettingsUserControl<EditPinViewModel, EditPinModel> ModuleSettingsUserControl { get; set; }

        public EditPinModel Model => EditPinModel;

        public EditPinModel EditPinModel
        {
            get => _editPinModel;
            set
            {
                if (_editPinModel == null || _editPinModel == value)
                    return;
                SetProperty(ref _editPinModel, value);
            }
        }

        private void AddPin(object sender)
        {
            try
            {
                ModuleSettingsUserControl = sender as ModuleSettingsUserControl<EditPinViewModel, EditPinModel>;

                if (EditPinModel.PinDetails.Count == 0)
                    EditPinModel.PinDetails = new ObservableCollectionBase<PinInfo>();
                if (ModuleSettingsUserControl != null)
                {
                    var editPinControl = ModuleSettingsUserControl.ObjViewModel.EditPinModel.PinInfo;

                    if (ModuleSettingsUserControl._accountGrowthModeHeader != null)
                        editPinControl.Account = ModuleSettingsUserControl._accountGrowthModeHeader.SelectedItem;

                    editPinControl.PinToBeEdit = editPinControl.PinToBeEdit?.Trim();
                    editPinControl.Account = editPinControl.Account?.Trim();
                    editPinControl.Board = editPinControl.Board?.Trim();
                    editPinControl.Title = editPinControl.Title?.Trim();
                    editPinControl.PinDescription = editPinControl.PinDescription?.Trim();
                    editPinControl.Section = editPinControl.Section?.Trim();
                    editPinControl.WebsiteUrl = editPinControl.WebsiteUrl?.Trim();

                    if (string.IsNullOrEmpty(editPinControl.Board) && string.IsNullOrEmpty(editPinControl.Title) && string.IsNullOrEmpty(editPinControl.PinDescription) &&
                        string.IsNullOrEmpty(editPinControl.Section) && string.IsNullOrEmpty(editPinControl.WebsiteUrl) &&
                        (string.IsNullOrEmpty(editPinControl.PinToBeEdit) || string.IsNullOrEmpty(editPinControl.Account))
                        && EditPinModel.listDetails.Count == 0)
                        return;

                    else if (EditPinModel.listDetails.Count > 0 && string.IsNullOrEmpty(editPinControl.PinToBeEdit))
                        EditPinModel.listDetails.ForEach(pin =>
                        {
                            if (EditPinModel.PinDetails.All(info => info.PinToBeEdit != pin.PinToBeEdit.Trim('/')))
                                EditPinModel.PinDetails.Add(new PinInfo
                                {
                                    Board = pin.Board,
                                    Title = pin.Title,
                                    PinDescription = pin.PinDescription,
                                    Section = pin.Section,
                                    WebsiteUrl = pin.WebsiteUrl,
                                    PinToBeEdit = pin.PinToBeEdit.TrimEnd('/'),
                                    Account = pin.Account
                                });
                        });

                    else if (!EditPinModel.PinDetails.Any(info =>
                        info.Account == editPinControl.Account && info.PinToBeEdit.ToLower() == "all"
                        || info.PinToBeEdit.Contains(editPinControl.PinToBeEdit)))
                        EditPinModel.PinDetails.Add(new PinInfo
                        {
                            Board = editPinControl.Board,
                            Title = editPinControl.Title,
                            PinDescription = editPinControl.PinDescription,
                            Section = editPinControl.Section,
                            WebsiteUrl = editPinControl.WebsiteUrl,
                            PinToBeEdit = editPinControl.PinToBeEdit,
                            Account = editPinControl.Account
                        });

                    else
                        return;

                    editPinControl.Board = string.Empty;
                    editPinControl.Title = string.Empty;
                    editPinControl.PinDescription = string.Empty;
                    editPinControl.Section = string.Empty;
                    editPinControl.WebsiteUrl = string.Empty;
                    editPinControl.PinToBeEdit = string.Empty;
                    editPinControl.SelectedIndex = 1;

                    EditPinModel.listOfSelectedAccounts =
                        EditPinModel.PinDetails.Select(x => x.Account).Distinct().ToList();
                    ModuleSettingsUserControl.FooterControl_OnSelectAccountChanged(EditPinModel.listOfSelectedAccounts);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeletePin(object sender)
        {
            try
            {
                var pindetails = sender as PinInfo;
                EditPinModel.PinDetails.Remove(pindetails);
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
                EditPinModel.listDetails.Clear();
                EditPinModel.listPins.Clear();
                EditPinModel.PinDetails.Clear();
                EditPinModel.listPins.AddRange(FileUtilities.FileBrowseAndReader());
                if (EditPinModel.listPins.Count > 0)
                {
                    GetEditPinsList(EditPinModel.listPins);
                }
                else
                {
                    GlobusLogHelper.log.Info("You did not upload any pins !!");
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        ///method use to add imported data to pinInfo list model 
        /// </summary>
        /// <param name="listPins"></param>
        private void GetEditPinsList(List<string> listPins)
        {
            try
            {
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountFileManager.GetAll(SocialNetworks.Pinterest)
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                    .Select(x => x.AccountBaseModel.UserName).ToList();
                var tempList = new List<PinInfo>();
                foreach (var pin in listPins)
                {
                    try
                    {
                        var pindetails = pin.Split('\t');
                        if (pindetails.Length >= 5 && (pindetails[5].Contains("E") || pindetails[5].Contains(".")))
                        {
                            Dialog.ShowDialog(Application.Current.MainWindow, "Info", "Please change the text type of 6th column.It should be text type. Exponential values cannot be accepted.");
                            return;
                        }
                        if (pindetails[0] == "Board Name") continue;
                        var pinInfo = new PinInfo();
                        pinInfo.Board = pindetails[0];
                        pinInfo.Title = pindetails[1];
                        pinInfo.PinDescription = pindetails[2];
                        pinInfo.Section = pindetails[3];
                        pinInfo.WebsiteUrl = pindetails[4];
                        pinInfo.PinToBeEdit = pindetails[5]?.Trim();
                        pinInfo.Account = pindetails[6];
                        if (!accounts.Contains(pinInfo.Account))
                        {
                            GlobusLogHelper.log.Info(String.Format("LangSpecificAccountIsNotPresent".FromResourceDictionary(), pinInfo.Account));
                            continue;
                        }
                        tempList.Add(pinInfo);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EditPinModel.PinDetails.AddRange(tempList);
                    EditPinModel.listDetails.AddRange(tempList);
                    Dialog.ShowDialog(Application.Current.MainWindow, "Info",
                     "Pins are ready to add !!");
                    GlobusLogHelper.log.Info("Pins Sucessfully uploaded !!");
                });
            }
            catch (Exception e)
            {
                e.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading pins !!");
            }
        }       
    }
}