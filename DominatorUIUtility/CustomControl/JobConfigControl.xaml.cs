using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using ProtoBuf;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfigControl : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        // Using a DependencyProperty as the backing store for PerJobActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerJobActivityProperty =
            DependencyProperty.Register("PerJobActivity", typeof(string), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerHourActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerHourActivityProperty =
            DependencyProperty.Register("PerHourActivity", typeof(string), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerDayActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerDayActivityProperty =
            DependencyProperty.Register("PerDayActivity", typeof(string), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerWeekActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerWeekActivityProperty =
            DependencyProperty.Register("PerWeekActivity", typeof(string), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });



        public static readonly DependencyProperty PerJobEnableProperty= DependencyProperty.Register("PerJobEnable", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue=Visibility.Visible
                });
        public static readonly DependencyProperty PerHourEnableProperty = DependencyProperty.Register("PerHourEnable", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty PerDayEnableProperty = DependencyProperty.Register("PerDayEnable", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty PerWeekEnableProperty = DependencyProperty.Register("PerWeekEnable", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableAdvanceProperty = DependencyProperty.Register("EnableAdvance", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableIncreasePerDayProperty = DependencyProperty.Register("EnableIncreasePerDay", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableFavTimeProperty = DependencyProperty.Register("EnableFavTime", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableOperationDelayProperty = DependencyProperty.Register("EnableOperationDelay", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableAccountDelayProperty = DependencyProperty.Register("EnableAccountDelay", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        public static readonly DependencyProperty EnableJobDelayProperty = DependencyProperty.Register("EnableJobDelay", typeof(Visibility), typeof(JobConfigControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = Visibility.Visible
                });
        private readonly IGenericFileManager _genericFileManager;

        private ObservableCollection<FavoriteTime> _lstFavoriteTime = new ObservableCollection<FavoriteTime>();

        public JobConfigControl()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

            InitializeComponent();
            MainGrid.DataContext = this;
            InitilizeFavoriteTime();
            SelectFavoriteTime = new BaseCommand<object>(sender => true, SelectFavoriteTimeExecute);
            RemoveFavoriteTimeCommand = new BaseCommand<object>(sender => true, RemoveFavoriteTimeExecute);
        }
        public Visibility PerJobEnable
        {
            get =>(Visibility) GetValue(PerJobEnableProperty);
            set => SetValue(PerJobEnableProperty, value);
        }
        public Visibility PerHourEnable
        {
            get => (Visibility)GetValue(PerHourEnableProperty);
            set => SetValue(PerHourEnableProperty, value);
        }
        public Visibility PerDayEnable
        {
            get => (Visibility)GetValue(PerDayEnableProperty);
            set => SetValue(PerDayEnableProperty, value);
        }
        public Visibility PerWeekEnable
        {
            get => (Visibility)GetValue(PerWeekEnableProperty);
            set => SetValue(PerWeekEnableProperty, value);
        }
        public Visibility EnableAdvance
        {
            get => (Visibility)GetValue(EnableAdvanceProperty);
            set => SetValue(EnableAdvanceProperty, value);
        }
        public Visibility EnableIncreasePerDay
        {
            get => (Visibility)GetValue(EnableIncreasePerDayProperty);
            set => SetValue(EnableIncreasePerDayProperty, value);
        }
        public Visibility EnableFavTime
        {
            get => (Visibility)GetValue(EnableFavTimeProperty);
            set => SetValue(EnableFavTimeProperty, value);
        }
        public Visibility EnableOperationDelay
        {
            get => (Visibility)GetValue(EnableOperationDelayProperty);
            set => SetValue(EnableOperationDelayProperty, value);
        }
        public Visibility EnableJobDelay
        {
            get => (Visibility)GetValue(EnableJobDelayProperty);
            set => SetValue(EnableJobDelayProperty, value);
        }
        public Visibility EnableAccountDelay
        {
            get => (Visibility)GetValue(EnableAccountDelayProperty);
            set => SetValue(EnableAccountDelayProperty, value);
        }
        public JobConfiguration JobConfiguration
        {
            get => (JobConfiguration)GetValue(JobConfigurationProperty);
            set => SetValue(JobConfigurationProperty, value);
        }


        public string PerJobActivity
        {
            get => (string)GetValue(PerJobActivityProperty);
            set => SetValue(PerJobActivityProperty, value);
        }


        public string PerHourActivity
        {
            get => (string)GetValue(PerHourActivityProperty);
            set => SetValue(PerHourActivityProperty, value);
        }


        public string PerDayActivity
        {
            get => (string)GetValue(PerDayActivityProperty);
            set => SetValue(PerDayActivityProperty, value);
        }


        public string PerWeekActivity
        {
            get => (string)GetValue(PerWeekActivityProperty);
            set => SetValue(PerWeekActivityProperty, value);
        }

        public ObservableCollection<FavoriteTime> LstFavoriteTime
        {
            get => _lstFavoriteTime;
            set
            {
                _lstFavoriteTime = value;
                OnPropertyChanged(nameof(LstFavoriteTime));
            }
        }

        public ICommand SelectFavoriteTime { get; set; }
        public ICommand RemoveFavoriteTimeCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RemoveFavoriteTimeExecute(object sender)
        {
            var itemTodelete = sender as string;
            LstFavoriteTime.Remove(LstFavoriteTime.FirstOrDefault(x => x.FavoriteTimeName == itemTodelete));
            _genericFileManager.UpdateModuleDetails(LstFavoriteTime.ToList(), ConstantVariable.GetFavoriteTimeFile());
        }


        public void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JobConfiguration.IsAdvanceSetting)
                return;
            try
            {
                var model = ((dynamic)((FrameworkElement)((FrameworkElement)sender).DataContext).DataContext).Model;

                if (JobConfiguration.SelectedItem == "Slow")
                {
                    var slowSpeed = model.SlowSpeed;
                    JobConfiguration.ActivitiesPerDay = slowSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = slowSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = slowSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = slowSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = slowSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = slowSpeed.DelayBetweenActivity;
                    JobConfiguration.DelayBetweenAccounts = slowSpeed.DelayBetweenAccounts;
                }
                else if (JobConfiguration.SelectedItem == "Medium")
                {
                    var mediumSpeed = model.MediumSpeed;
                    JobConfiguration.ActivitiesPerDay = mediumSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = mediumSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = mediumSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = mediumSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = mediumSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = mediumSpeed.DelayBetweenActivity;
                    JobConfiguration.DelayBetweenAccounts = mediumSpeed.DelayBetweenAccounts;
                }
                else if (JobConfiguration.SelectedItem == "Fast")
                {
                    var fastSpeed = model.FastSpeed;
                    JobConfiguration.ActivitiesPerDay = fastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = fastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = fastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = fastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = fastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = fastSpeed.DelayBetweenActivity;
                    JobConfiguration.DelayBetweenAccounts = fastSpeed.DelayBetweenAcounts;
                }
                else if (JobConfiguration.SelectedItem == "Superfast")
                {
                    var superfastSpeed = model.SuperfastSpeed;
                    JobConfiguration.ActivitiesPerDay = superfastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = superfastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = superfastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = superfastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = superfastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = superfastSpeed.DelayBetweenActivity;
                    JobConfiguration.DelayBetweenAccounts = superfastSpeed.DelayBetweenAccounts;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnCreateFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            var favoriteTimeName = "LangKeyFavoriteTime".FromResourceDictionary();
            while (true)
                try
                {
                    favoriteTimeName = Dialog.GetInputDialog("LangKeyFavoriteTime".FromResourceDictionary(),
                        "LangKeyEnterFavoriteTime".FromResourceDictionary(), favoriteTimeName,
                        "LangKeySave".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                    if (!string.IsNullOrEmpty(favoriteTimeName?.Trim()))
                    {
                        if (!LstFavoriteTime.Any(x => x.FavoriteTimeName == favoriteTimeName))
                        {
                            var favoriteTime = new FavoriteTime
                            {
                                FavoriteTimeName = favoriteTimeName,
                                LstFavoriteTimes = JobConfiguration.RunningTime.DeepCloneObject()
                            };
                            _genericFileManager.AddModule(favoriteTime, ConstantVariable.GetFavoriteTimeFile());
                            LstFavoriteTime.Add(favoriteTime);

                            break;
                        }

                        var result = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                            string.Format("LangKeyFavoriteTimeWithNameExistWannaOverride".FromResourceDictionary(),
                                favoriteTimeName), "LangKeyYes".FromResourceDictionary(),
                            "LangKeyNo".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var oldLstFavoriteTime =
                                LstFavoriteTime.FirstOrDefault(x => x.FavoriteTimeName == favoriteTimeName);
                            oldLstFavoriteTime.LstFavoriteTimes = JobConfiguration.RunningTime;
                            _genericFileManager.UpdateModuleDetails(LstFavoriteTime.ToList(),
                                ConstantVariable.GetFavoriteTimeFile());
                            break;
                        }
                    }
                    else if (favoriteTimeName == null)
                    {
                        break;
                    }
                    else
                    {
                        var result = Dialog.ShowCustomDialog("LangKeyError".FromResourceDictionary(),
                            "LangKeyFavoriteTimeShountBeEmpty".FromResourceDictionary(),
                            "LangKeyOk".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                            continue;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        private void SelectFavoriteTimeExecute(object sender)
        {
            try
            {
                var selectedFavoriteTime = sender as FavoriteTime;

                JobConfiguration.RunningTime = selectedFavoriteTime.LstFavoriteTimes.DeepCloneObject();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void InitilizeFavoriteTime()
        {
            try
            {
                var lstFavoriteTimes =
                    _genericFileManager.GetModuleDetails<FavoriteTime>(ConstantVariable.GetFavoriteTimeFile());

                Application.Current.Dispatcher.Invoke(delegate
                {
                    LstFavoriteTime.Clear();
                    lstFavoriteTimes.ForEach(x => { LstFavoriteTime.Add(x); });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void JobConfigControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitilizeFavoriteTime();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ProtoContract]
    public class FavoriteTime : INotifyPropertyChanged
    {
        [ProtoMember(2)] public List<RunningTimes> _lstFavoriteTimes = new List<RunningTimes>();

        [ProtoMember(1)] private string favoriteTimeName = string.Empty;

        public string FavoriteTimeName
        {
            get => favoriteTimeName;

            set
            {
                if (value == favoriteTimeName) return;
                favoriteTimeName = value;
                OnPropertyChanged(nameof(FavoriteTimeName));
            }
        }

        public List<RunningTimes> LstFavoriteTimes
        {
            get => _lstFavoriteTimes;

            set
            {
                if (value == _lstFavoriteTimes) return;
                _lstFavoriteTimes = value;
                OnPropertyChanged(nameof(LstFavoriteTimes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}