using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.Boards
{
    public class BoardViewModel : BindableBase
    {
        private BoardModel _boardModel = new BoardModel();

        public BoardViewModel()
        {
            BoardModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBoardsCreatePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBoardsCreatePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBoardsCreatePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBoardsCreatePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBoardsCreatePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            // BindCategory();
            AssignNewCategory();
            AddBoardCommand = new BaseCommand<object>(sender => true, AddBoard);
            DeleteBoardCommand = new BaseCommand<object>(sender => true, DeleteBoard);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportFromCsv);
            LearnMoreExecuteCommand = new BaseCommand<object>(sender => true, LearnMoreExecute);
        }

        public ICommand AddBoardCommand { get; set; }
        public ICommand DeleteBoardCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        public ICommand LearnMoreExecuteCommand { get; set; }
        public BoardModel Model => BoardModel;

        public BoardModel BoardModel
        {
            get => _boardModel;
            set
            {
                if ((_boardModel == null) & (_boardModel == value))
                    return;
                SetProperty(ref _boardModel, value);
            }
        }

        private void AddBoard(object sender)
        {
            try
            {
                if (BoardModel.BoardDetails.Count == 0)
                    BoardModel.BoardDetails = new ObservableCollectionBase<BoardInfo>();
                var createBoardControl = sender as BoardInfo;

                if (createBoardControl != null && BoardModel.listBoardsDetails.Count > 0 && string.IsNullOrEmpty(createBoardControl.BoardName))
                {
                    BoardModel.listBoardsDetails.ForEach(pin =>
                    {
                        if (BoardModel.BoardDetails.All(x => !x.BoardName.Equals(pin.BoardName)))
                            BoardModel.BoardDetails.Add(new BoardInfo
                            {
                                BoardName = pin.BoardName,
                                BoardDescription = pin.BoardDescription,
                                Category = pin.Category,
                                Section=pin.Section,
                                KeepBoardSecret=pin.KeepBoardSecret,
                                SectionList = GetSections(pin.Section)
                            });
                    });
                }

                else if (createBoardControl != null && createBoardControl.ChkSpintax && !string.IsNullOrEmpty(createBoardControl.BoardName))
                {
                    var lstBoardName = SpinTexHelper.GetSpinMessageCollection(createBoardControl.BoardName);
                    var lstBoardDescription = SpinTexHelper.GetSpinMessageCollection(createBoardControl.BoardDescription);
                    foreach (var boardName in lstBoardName)
                    {
                        if (BoardModel.BoardDetails.Any(x => x.BoardName.Equals(boardName)))
                            continue;

                        string category;
                        if (createBoardControl.UseRandomCategory)
                            category = BoardModel.ListCategory.GetRandomItem();
                        else
                            category = string.IsNullOrEmpty(createBoardControl.Category) ? string.Empty : createBoardControl.Category;

                        BoardModel.BoardDetails.Add(new BoardInfo
                        {
                            BoardName = boardName,
                            BoardDescription = lstBoardDescription.Count == 0 ? string.Empty : lstBoardDescription?.GetRandomItem(),
                            Category = category,
                            KeepBoardSecret = createBoardControl.KeepBoardSecret,
                            Section =createBoardControl.Section,
                            SectionList=GetSections(createBoardControl.Section)
                        });

                        if (createBoardControl.SkipCommonCategory)
                        {
                            ToasterNotification.ShowSuccess("Board Details Added");
                            break;
                        }
                    }
                }

                else if (createBoardControl != null && !string.IsNullOrEmpty(createBoardControl.BoardName))
                {
                    if (string.IsNullOrEmpty(createBoardControl.BoardDescription))
                        createBoardControl.BoardDescription = string.Empty;
                    var boardNames = createBoardControl.BoardName.Split('|').Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();
                    var boardDescriptions = createBoardControl.BoardDescription.Split('|')
                        .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    var sectionsDescriptions = createBoardControl.Section.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (boardNames.Length == 1 && boardDescriptions.Length == 1 && sectionsDescriptions.Length == 1)
                    {
                        if (createBoardControl.BoardName.Any(char.IsLetterOrDigit))
                        {
                            var category = string.Empty;
                            if (createBoardControl.UseRandomCategory)
                                category = BoardModel.ListCategory.GetRandomItem();
                            else
                                category = string.IsNullOrEmpty(createBoardControl.Category) ? string.Empty : createBoardControl.Category;

                            if (BoardModel.BoardDetails.All(x => !x.BoardName.Equals(createBoardControl.BoardName)))
                                BoardModel.BoardDetails.Add(new BoardInfo
                                {
                                    BoardName = createBoardControl.BoardName,
                                    BoardDescription = createBoardControl.BoardDescription,
                                    Category = category,
                                    Section=createBoardControl.Section,
                                    KeepBoardSecret=createBoardControl.KeepBoardSecret,
                                    SectionList=GetSections(createBoardControl.Section)
                                });
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "",
                                ActivityType.CreateBoard,
                                "LangKeyYouAreRequiredToHaveAtLeastOneLetterOrNumberInABoardName".FromResourceDictionary());
                            return;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < boardNames.Length; i++)
                        {
                            var category = string.Empty;
                            if (createBoardControl.UseRandomCategory)
                                category = BoardModel.ListCategory.GetRandomItem();
                            else
                                category = string.IsNullOrEmpty(createBoardControl.Category) ? string.Empty : createBoardControl.Category;

                            var section = sectionsDescriptions.Length >= i + 1
                                    ? sectionsDescriptions[i].Trim('(', ')')
                                    : string.Empty;
                            var currentBoard = new BoardInfo
                            {
                                BoardName = boardNames[i].Trim('(', ')'),
                                BoardDescription = boardDescriptions.Length >= i + 1
                                    ? boardDescriptions[i].Trim('(', ')')
                                    : string.Empty,
                                Category = category,
                                Section = section,
                                KeepBoardSecret=createBoardControl.KeepBoardSecret,
                                SectionList = boardNames.Length > 1 ? GetSections(createBoardControl.Section) : new List<string> { section }
                            };
                            if (boardNames[i].Trim().Any(char.IsLetterOrDigit))
                                if (BoardModel.BoardDetails.All(x => !x.BoardName.Equals(currentBoard.BoardName)))
                                    BoardModel.BoardDetails.Add(currentBoard);
                        }
                    }
                }

                if (BoardModel.BoardDetails.Count == 0)
                    return;

                if (!createBoardControl.SkipCommonCategory)
                {
                    createBoardControl.BoardName = string.Empty;
                    createBoardControl.BoardDescription = string.Empty;
                    createBoardControl.SelectedIndex = 1;
                    BoardModel.ListCategory = new List<string>();
                    AssignNewCategory();
                    createBoardControl.ChkSpintax = false;
                    createBoardControl.UseRandomCategory = false;
                    createBoardControl.KeepBoardSecret = false;
                    createBoardControl.Section = string.Empty;
                    createBoardControl.SectionList.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private List<string> GetSections(string Section)
        {
            var sectionList = new List<string>();
            if (string.IsNullOrEmpty(Section))
                return sectionList;
            if (!string.IsNullOrEmpty(Section) && Section.Contains("|"))
                Section.Split('|').ForEach(x => sectionList.Add(x));
            else
                sectionList.Add(Section);
            sectionList.RemoveAll(x => x == string.Empty);
            return sectionList;
        }
        private void DeleteBoard(object sender)
        {
            try
            {
                var boardDetails = sender as BoardInfo;
                BoardModel.BoardDetails.Remove(boardDetails);
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
                BoardModel.listBoardsDetails.Clear();
                BoardModel.listBoards.Clear();
                BoardModel.listBoards.AddRange(FileUtilities.FileBrowseAndReader());
                var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                if (BoardModel.listBoards.Count != 0)
                {
                    foreach (var board in BoardModel.listBoards)
                        try
                        {
                            var boarddetails = board.Split('\t');
                            if (boarddetails[0] == "Board Name") continue;

                            var lstBoardName = SpinTexHelper.GetSpinMessageCollection(boarddetails[0]);
                            var lstBoardDescription = SpinTexHelper.GetSpinMessageCollection(boarddetails[1]);

                            foreach (var boardName in lstBoardName)
                            {
                                if (BoardModel.BoardDetails.Any(x => x.BoardName.Equals(boardName)))
                                    continue;

                                var boardInfo = new BoardInfo();
                                boardInfo.BoardName = boardName;
                                boardInfo.BoardDescription = lstBoardDescription.GetRandomItem();
                                boardInfo.Category = boarddetails[2];
                                boardInfo.Section = boarddetails.Length > 2 ?boarddetails[3]: string.Empty;
                                BoardModel.listBoardsDetails.Add(boardInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyInfo".FromResourceDictionary(),
                        "LangKeyBoardsAreReadyToAdd".FromResourceDictionary());
                    GlobusLogHelper.log.Info("LangKeyBoardsSucessfullyUploaded".FromResourceDictionary());
                }
                else
                {
                    GlobusLogHelper.log.Info("LangKeyYouDidNotUploadAnyBoard".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("LangKeyThereIsErrorInUploadingBoards".FromResourceDictionary());
            }
        }
        private void LearnMoreExecute(object sender)
        {
            try
            {
                Process.Start(BoardModel.BoardInfo.LearnMoreBoardPolicy);
            }
            catch { }
        }
        public void AssignNewCategory()
        {
            var lstCategory = new List<string>
            {
                "Animals and pets",
                "Architecture",
                "Art",
                "Cars and motorcycles",
                "Celebrities",
                "DIY and crafts",
                "Design",
                "Education",
                "Entertainment",
                "Food and drink",
                "Gardening",
                "Geek",
                "Hair and beauty",
                "Health and fitness",
                "History",
                "Holidays and events",
                "Home decor",
                "Humor",
                "Illustrations and posters",
                "Kids and parenting",
                "Men's fashion",
                "Outdoors",
                "Photography",
                "Products",
                "Quotes",
                "Science and nature",
                "Sports",
                "Tattoos",
                "Technology",
                "Travel",
                "Weddings",
                "Women's fashion",
                "Other"
            };

            BoardModel.ListCategory = lstCategory;
        }
    }
}