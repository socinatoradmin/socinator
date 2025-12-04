using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ILikeCommentViewModel
    {
    }

    public class LikeCommentViewModel : StartupBaseViewModel, ILikeCommentViewModel
    {
        public bool _isActionasOwnAccountChecked = true;

        public bool _isActionasPageChecked;

        public LikeCommentViewModel(IRegionManager region) : base(region)
        {
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMuliple);

            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.LikeComment});

            NextCommand = new DelegateCommand(LikeCommentValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        public LikerCommentorConfigModel LikerCommentorConfigModel { get; set; }
            = new LikerCommentorConfigModel();

        public bool IsActionasPageChecked
        {
            get => _isActionasPageChecked;
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        public bool IsActionasOwnAccountChecked
        {
            get => _isActionasOwnAccountChecked;
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        public string OwnPageUrl { get; set; }
        public List<string> ListOwnPageUrl { get; set; } = new List<string>();

        private void LikeCommentValidate()
        {
            if (!IsActionasPageChecked && !IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog("Warning", "Please Select Reaction Type");
                return;
            }

            if (IsActionasPageChecked && ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please Select PageUrls");
                return;
            }

            if (SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (LikerCommentorConfigModel.ListReactionType.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one reaction type.");
                return;
            }

            NavigateNext();
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}