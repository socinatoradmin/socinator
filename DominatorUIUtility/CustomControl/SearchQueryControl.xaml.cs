using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Microsoft.Win32;

namespace DominatorUIUtility.CustomControl
{
    public partial class SearchQueryControl : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SearchQueryControl),
                new FrameworkPropertyMetadata());

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SearchQueryControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for CustomFilterCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomFilterCommandProperty =
            DependencyProperty.Register("CustomFilterCommand", typeof(ICommand), typeof(SearchQueryControl));

        // Using a DependencyProperty as the backing store for DeleteQueryCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteQueryCommandProperty =
            DependencyProperty.Register("DeleteQueryCommand", typeof(ICommand), typeof(SearchQueryControl));

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteMulipleCommandProperty =
            DependencyProperty.Register("DeleteMulipleCommand", typeof(ICommand), typeof(SearchQueryControl));

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuryTypeSelectionChangedCommandProperty =
            DependencyProperty.Register("QuryTypeSelectionChangedCommand", typeof(ICommand),
                typeof(SearchQueryControl));

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionChangedCommandParameterProperty =
            DependencyProperty.Register("SelectionChangedCommandParameter", typeof(object), typeof(SearchQueryControl));

        private bool _isEnable = true;

        public SearchQueryControl()
        {
            InitializeComponent();
            CurrentQuery = new QueryInfo();
            MainGrid.DataContext = this;
            IsExpanded = true;
            // AddQueryCommand = new BaseCommand<object>(CanExecute, Execute);
            SelectedIndex = 0;
            ListQueryType = new List<string>();
            ListQueryInfo = new ObservableCollection<QueryInfo>();
            LstNonQueryType.Add("LangKeyOwnFollowers".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyOwnFollowings".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyNewsfeed".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyJoinedCommunityMembers".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyMyConnectionsPostS".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapUsersWhoMessagedUs".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapAllLikes".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyNewsFeedPosts".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyOwnFriends".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapUserWhomWeMessaged".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyPeopleConnectedInMessenger".FromResourceDictionary());
            LstNonQueryType.Add("LangKeySuggestedFriends".FromResourceDictionary());
            LstNonQueryType.Add("LangKeySuggestedUsers".FromResourceDictionary());
            LstNonQueryType.Add("LangKeySuggestedUsersPosts".FromResourceDictionary());

            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQueryExecute);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMulipleExecute);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public HashSet<string> LstNonQueryType { get; set; } = new HashSet<string>();

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        public ActivityType ActivityType { get; set; }
        public string Network { get; set; }

        public ICommand CustomFilterCommand
        {
            get => (ICommand)GetValue(CustomFilterCommandProperty);
            set => SetValue(CustomFilterCommandProperty, value);
        }

        public ICommand DeleteQueryCommand
        {
            get => (ICommand)GetValue(DeleteQueryCommandProperty);
            set => SetValue(DeleteQueryCommandProperty, value);
        }

        public ICommand DeleteMulipleCommand
        {
            get => (ICommand)GetValue(DeleteMulipleCommandProperty);
            set => SetValue(DeleteMulipleCommandProperty, value);
        }

        public ICommand QuryTypeSelectionChangedCommand
        {
            get => (ICommand)GetValue(QuryTypeSelectionChangedCommandProperty);
            set => SetValue(QuryTypeSelectionChangedCommandProperty, value);
        }


        public object SelectionChangedCommandParameter
        {
            get => GetValue(SelectionChangedCommandParameterProperty);
            set => SetValue(SelectionChangedCommandParameterProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnQuerySelect(object sender, RoutedEventArgs e)
        {
            if (ListQueryInfo.All(x => x.IsQuerySelected))
            {
                IsAllQuerySelected = true;
            }
            else
            {
                if (IsAllQuerySelected)
                    IsCheckFromList = true;
                IsAllQuerySelected = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CurrentQuery.QueryType = ListQueryType.ToList()[SelectedIndex];
                if (LstNonQueryType.Contains(CurrentQuery.QueryType))
                {
                    CurrentQuery.QueryValue = "NA";
                    IsEnable = false;
                    BtnImportQuery.IsEnabled = false;
                }
                else
                {
                    IsEnable = true;
                    BtnImportQuery.IsEnabled = true;
                    CurrentQuery.QueryValue = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DeleteQueryExecute(object sender)
        {
            try
            {
                var QueryToDelete = sender as QueryInfo;
                DeleteQueryEventHandler();
                if (ListQueryInfo.Any(x => QueryToDelete != null && x.Id == QueryToDelete.Id))
                {
                    QueryCollection.Remove(QueryToDelete.QueryValue);
                    ListQueryInfo.Remove(QueryToDelete);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void DeleteMulipleExecute(object obj)
        {
            try
            {
                if (IsAllQuerySelected)
                {
                    QueryCollection.Clear();
                    ListQueryInfo.Clear();
                    IsAllQuerySelected = false;
                    return;
                }

                foreach (var queryInfo in ListQueryInfo.ToList())
                    if (queryInfo.IsQuerySelected)
                    {
                        QueryCollection.Remove(queryInfo.QueryValue);
                        ListQueryInfo.Remove(queryInfo);
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ExportSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ListQueryInfo.Any(x => x.IsQuerySelected))
                {
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                        "LangKeySelectAtleastOneQuery".FromResourceDictionary());
                    return;
                }

                // var network = SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ? SocinatorInitialize.AccountModeActiveSocialNetwork : SocinatorInitialize.ActiveSocialNetwork;
                var saveFiledialog = new SaveFileDialog
                {
                    Filter = "CSV file (.csv)|*.csv",
                    FileName = Network + "-" + ActivityType + "-Query-" + DateTime.Now.GetCurrentEpochTime()
                };

                if (saveFiledialog.ShowDialog() == true)
                {
                    var filename = saveFiledialog.FileName;
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        ListQueryInfo.ForEach(x =>
                        {
                            if (x.IsQuerySelected)
                                streamWriter.WriteLine(Network + "," + ActivityType.ToString() + "," + x.QueryType +
                                                       "," + x.QueryValue);
                        });
                        ToasterNotification.ShowSuccess("LangKeyQueriesUploaded".FromResourceDictionary());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #region Variables

        private int _selectedIndex;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        private bool _isAllQuerySelected;

        public bool IsAllQuerySelected
        {
            get => _isAllQuerySelected;
            set
            {
                if (_isAllQuerySelected == value)
                    return;
                _isAllQuerySelected = value;
                OnPropertyChanged(nameof(IsAllQuerySelected));
                SelectAll(_isAllQuerySelected);
                IsCheckFromList = false;
            }
        }

        public bool IsCheckFromList { get; set; }

        private void SelectAll(bool _isAllQuerySelected)
        {
            if (IsCheckFromList)
                return;
            ListQueryInfo.Select(x =>
            {
                x.IsQuerySelected = _isAllQuerySelected;
                return x;
            }).ToList();
        }


        public List<Enum> LstQueryType
        {
            get => (List<Enum>)GetValue(LstQueryTypeProperty);
            set => SetValue(LstQueryTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for LstQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstQueryTypeProperty =
            DependencyProperty.Register("LstQueryType", typeof(List<Enum>),
                typeof(SearchQueryControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public IEnumerable<string> ListQueryType
        {
            get => (IEnumerable<string>)GetValue(ListQueryTypeProperty);
            set => SetValue(ListQueryTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryTypeProperty =
            DependencyProperty.Register("ListQueryType", typeof(IEnumerable<string>), typeof(SearchQueryControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public QueryInfo CurrentQuery
        {
            get => (QueryInfo)GetValue(CurrentQueryProperty);
            set => SetValue(CurrentQueryProperty, value);
        }

        // Using a DependencyProperty as the backing store for CurrentQuery.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentQueryProperty =
            DependencyProperty.Register("CurrentQuery", typeof(QueryInfo), typeof(SearchQueryControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public ObservableCollection<QueryInfo> ListQueryInfo
        {
            get => (ObservableCollection<QueryInfo>)GetValue(ListQueryInfoProperty);
            set => SetValue(ListQueryInfoProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListQueryInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryInfoProperty =
            DependencyProperty.Register("ListQueryInfo", typeof(ObservableCollection<QueryInfo>),
                typeof(SearchQueryControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public List<string> QueryCollection { get; set; } = new List<string>();

        #endregion

        #region Import Query Details

        /// <summary>
        ///     Create a routed event which is registered to event manager with the characteristics
        /// </summary>
        private static readonly RoutedEvent GetQueryClickEvent = EventManager.RegisterRoutedEvent("GetQueryClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

        /// <summary>
        ///     Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler GetQueryClick
        {
            add => AddHandler(GetQueryClickEvent, value);
            remove => RemoveHandler(GetQueryClickEvent, value);
        }

        private void GetQueryClickEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(GetQueryClickEvent);
            RaiseEvent(routedEventArgs);
        }


        /// <summary>
        ///     To Read the Query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImportQuery_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                QueryCollection.Clear();
                QueryCollection.AddRange(FileUtilities.FileBrowseAndReader());

                if (QueryCollection.Count != 0)
                {
                    Dialog.ShowDialog("LangKeyInfo".FromResourceDictionary(),
                        "LangKeyQueriesReadyToAdd".FromResourceDictionary());
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "",
                        ActivityType, "LangKeyQueriesUploaded".FromResourceDictionary());
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "",
                        ActivityType, "LangKeyNoQueryUploaded".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "", ActivityType,
                    "LangKeyErrorOccuredWhileQueryUploading".FromResourceDictionary());
            }

            GetQueryClickEventHandler();
        }

        #endregion

        #region CustomFilters

        private static readonly RoutedEvent CustomFilterChangedEvent =
            EventManager.RegisterRoutedEvent("CustomFilterChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(SearchQueryControl));

        public event RoutedEventHandler CustomFilterChanged
        {
            add => AddHandler(CustomFilterChangedEvent, value);
            remove => RemoveHandler(CustomFilterChangedEvent, value);
        }

        private void CustomFilterEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(CustomFilterChangedEvent);
            RaiseEvent(routedEventArgs);
        }

        #endregion

        #region Add current query to query list 

        private static readonly RoutedEvent AddQueryEvent = EventManager.RegisterRoutedEvent("AddQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

        public event RoutedEventHandler AddQuery
        {
            add => AddHandler(AddQueryEvent, value);
            remove => RemoveHandler(AddQueryEvent, value);
        }

        private static readonly DependencyProperty AddQueryCommandProperty
            = DependencyProperty.Register("AddQueryCommand", typeof(ICommand), typeof(SearchQueryControl));

        public ICommand AddQueryCommand
        {
            get => (ICommand)GetValue(AddQueryCommandProperty);
            set => SetValue(AddQueryCommandProperty, value);
        }

        #endregion


        #region Delete the query from query list

        private static readonly RoutedEvent DeleteQueryEvent = EventManager.RegisterRoutedEvent("DeleteQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

        public event RoutedEventHandler DeleteQuery
        {
            add => AddHandler(DeleteQueryEvent, value);
            remove => RemoveHandler(DeleteQueryEvent, value);
        }

        private void DeleteQueryEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(DeleteQueryEvent);
            RaiseEvent(routedEventArgs);
        }

        #endregion
    }
}