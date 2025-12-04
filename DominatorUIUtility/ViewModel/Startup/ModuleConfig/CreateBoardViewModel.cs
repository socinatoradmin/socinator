using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class BoardInfo : BindableBase
    {
        private string _boardDescription;
        private string _boardName;


        private string _category;

        private int _selectedIndex;

        public string BoardName
        {
            get => _boardName;
            set
            {
                if (_boardName != null && _boardName == value)
                    return;
                SetProperty(ref _boardName, value);
            }
        }

        public string BoardDescription
        {
            get => _boardDescription;
            set
            {
                if (_boardDescription != null && _boardDescription == value)
                    return;
                SetProperty(ref _boardDescription, value);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                if (_category != null && _category == value)
                    return;
                SetProperty(ref _category, value);
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


        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public interface ICreateBoardViewModel
    {
    }

    public class CreateBoardViewModel : StartupBaseViewModel, ICreateBoardViewModel
    {
        private BoardInfo _currentBoard = new BoardInfo();

        private ObservableCollectionBase<BoardInfo> _listBoardInfo = new ObservableCollectionBase<BoardInfo>();

        private ObservableCollection<string> _listCategory = new ObservableCollection<string>();

        public CreateBoardViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.CreateBoard});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBoardsCreatePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBoardsCreatePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBoardsCreatePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBoardsCreatePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBoardsCreatePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AssignNewCategory();
            AddBoardCommand = new BaseCommand<object>(sender => true, AddBoard);
            DeleteBoardCommand = new BaseCommand<object>(sender => true, DeleteBoard);
            ListQueryType.Clear();
        }

        public ICommand AddBoardCommand { get; set; }
        public ICommand DeleteBoardCommand { get; set; }

        public ObservableCollection<string> ListCategory
        {
            get => _listCategory;
            set
            {
                if (_listCategory != null && _listCategory == value)
                    return;
                SetProperty(ref _listCategory, value);
            }
        }

        public BoardInfo CurrentBoard
        {
            get => _currentBoard;
            set
            {
                if (_currentBoard != null && _currentBoard == value)
                    return;
                SetProperty(ref _currentBoard, value);
            }
        }

        public ObservableCollectionBase<BoardInfo> BoardDetails
        {
            get => _listBoardInfo;
            set
            {
                if (_listBoardInfo != null && _listBoardInfo == value)
                    return;
                SetProperty(ref _listBoardInfo, value);
            }
        }

        private void ValidateAndNevigate()
        {
            if (BoardDetails.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one board.");
                return;
            }

            NavigateNext();
        }

        private void AddBoard(object sender)
        {
            try
            {
                if (BoardDetails.Count == 0)
                    BoardDetails = new ObservableCollectionBase<BoardInfo>();
                var boardInfo = sender as BoardInfo;
                var createBoardControl = sender as BoardInfo;
                if (createBoardControl != null && !string.IsNullOrEmpty(createBoardControl.BoardName))
                {
                    if (string.IsNullOrEmpty(createBoardControl.BoardDescription))
                        createBoardControl.BoardDescription = string.Empty;
                    var boardNames = createBoardControl.BoardName.Split('|').Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();
                    var boardDescriptions = createBoardControl.BoardDescription.Split('|')
                        .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                    if (boardNames.Length == 1 && boardDescriptions.Length == 1)
                    {
                        if (createBoardControl.BoardName.Any(char.IsLetterOrDigit))
                        {
                            BoardDetails.Add(new BoardInfo
                            {
                                BoardName = createBoardControl.BoardName,
                                BoardDescription = createBoardControl.BoardDescription,
                                Category = createBoardControl.Category
                            });
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "",
                                ActivityType.CreateBoard,
                                "You are required to have at least one letter or number in a board name");
                            return;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < boardNames.Length; i++)
                        {
                            var currentBoard = new BoardInfo
                            {
                                BoardName = boardNames[i].Trim(),
                                BoardDescription = boardDescriptions.Length >= i + 1
                                    ? Utilities.GetBetween(boardDescriptions[i], "{", "}").Trim()
                                    : string.Empty,
                                Category = createBoardControl.Category
                            };
                            if (boardNames[i].Trim().Any(char.IsLetterOrDigit)) BoardDetails.Add(currentBoard);
                        }
                    }

                    if (BoardDetails.Count == 0)
                        return;

                    createBoardControl.BoardName = string.Empty;
                    createBoardControl.BoardDescription = string.Empty;
                    createBoardControl.SelectedIndex = 1;
                    ListCategory = new ObservableCollection<string>();
                    AssignNewCategory();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteBoard(object sender)
        {
            try
            {
                var boardDetails = sender as BoardInfo;
                BoardDetails.Remove(boardDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AssignNewCategory()
        {
            ListCategory.Add("Animals and pets");
            ListCategory.Add("Architecture");
            ListCategory.Add("Art");
            ListCategory.Add("Cars and motorcycles");
            ListCategory.Add("Celebrities");
            ListCategory.Add("DIY and crafts");
            ListCategory.Add("Design");
            ListCategory.Add("Education");
            ListCategory.Add("Entertainment");
            ListCategory.Add("Food and drink");
            ListCategory.Add("Gardening");
            ListCategory.Add("Geek");
            ListCategory.Add("Hair and beauty");
            ListCategory.Add("Health and fitness");
            ListCategory.Add("History");
            ListCategory.Add("Holidays and events");
            ListCategory.Add("Home decor");
            ListCategory.Add("Humor");
            ListCategory.Add("Illustrations and posters");
            ListCategory.Add("Kids and parenting");
            ListCategory.Add("Men's fashion");
            ListCategory.Add("Outdoors");
            ListCategory.Add("Photography");
            ListCategory.Add("Products");
            ListCategory.Add("Quotes");
            ListCategory.Add("Science and nature");
            ListCategory.Add("Sports");
            ListCategory.Add("Tattoos");
            ListCategory.Add("Technology");
            ListCategory.Add("Travel");
            ListCategory.Add("Weddings");
            ListCategory.Add("Women's fashion");
            ListCategory.Add("Other");
        }
    }
}