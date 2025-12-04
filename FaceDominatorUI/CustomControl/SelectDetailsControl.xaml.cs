using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.AccountSelectorModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectOptionControl.xaml
    /// </summary>
    public partial class SelectDetailsControl
    {
        public static readonly DependencyProperty SelectedDetailsOptionProperty =
            DependencyProperty.Register("SelectedDetailsOption", typeof(string), typeof(SelectDetailsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SelectOptionModelProperty =
            DependencyProperty.Register("SelectOptionModel", typeof(SelectOptionModel), typeof(SelectDetailsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSelectOptionVisibleProperty =
            DependencyProperty.Register("IsSelectOptionVisible", typeof(bool), typeof(SelectDetailsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public SelectDetailsControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            SaveCommandBinding = new BaseCommand<object>(sender => true, UserInputOnSaveExecute);
            SelectOptionCommandBinding = new BaseCommand<object>(sender => true, SelectOptionCommandExecute);
        }

        public ICommand SaveCommandBinding { get; set; }

        public ICommand SelectOptionCommandBinding { get; set; }


        /*private static readonly RoutedEvent SelectInputClickEvent = EventManager.RegisterRoutedEvent("SelectInputClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectDetailsControl));

        /// <summary>
        /// Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler SelectInputClick
        {
            add { AddHandler(SelectInputClickEvent, value); }
            remove { RemoveHandler(SelectInputClickEvent, value); }
        }

        void SelectInputClickEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(SelectInputClickEvent);
            RaiseEvent(routedEventArgs);
        }*/

        public string SelectedDetailsOption
        {
            get => (string)GetValue(SelectedDetailsOptionProperty);
            set => SetValue(SelectedDetailsOptionProperty, value);
        }

        public SelectOptionModel SelectOptionModel
        {
            get => (SelectOptionModel)GetValue(SelectOptionModelProperty);
            set => SetValue(SelectOptionModelProperty, value);
        }


        public bool IsSelectOptionVisible
        {
            get => (bool)GetValue(IsSelectOptionVisibleProperty);
            set => SetValue(IsSelectOptionVisibleProperty, value);
        }


        private void SelectOptionCommandExecute(object obj)
        {
            if (SelectedDetailsOption == Application.Current.FindResource("LangKeySelectFriends")?.ToString())
                LoadFriends();
            else if (SelectedDetailsOption == Application.Current.FindResource("LangKeySelectPages")?.ToString())
                LoadPages();
        }

        private void LoadPages()
        {
            var model = SelectOptionModel.SelectAccountDetailsModel;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Friend, FbEntityTypes.Group, FbEntityTypes.CustomDestination
            };


            var selectAccountDetailsControl = SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Count == 0
                ? new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages", true)
                : new SelectAccountDetailsControl(hiddenColumnList, model, true);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    SelectOptionModel.AccountPagesPair.Clear();
                    SelectOptionModel.ListCustomDetailsUrl.Clear();

                    model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountPagespair = model.AccountPagesBoardsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            SelectOptionModel.AccountPagesPair.AddRange(accountPagespair);
                            SelectOptionModel.ListCustomDetailsUrl.AddRange(accountPagespair.Select(z => z.Value)
                                .ToList());
                            SelectOptionModel.ListCustomDetailsUrl =
                                SelectOptionModel.ListCustomDetailsUrl.Distinct().ToList();
                            SelectOptionModel.ListCustomDetailsUrl.ForEach(z =>
                            {
                                if (!SelectOptionModel.CustomDetailsText.Contains(z))
                                    SelectOptionModel.CustomDetailsText += z + "\r\n";
                            });
                        }
                        else if (model.AccountPagesBoardsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "",
                                "Destiation is selected but Account is not selected");
                        }
                    });


                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();

            SelectOptionModel.SelectAccountDetailsModel = selectAccountDetailsControl.GetSelectAccountModel();
        }

        private void LoadFriends()
        {
            var model = SelectOptionModel.SelectAccountDetailsModel;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Page, FbEntityTypes.Group, FbEntityTypes.CustomDestination
            };


            var selectAccountDetailsControl = SelectOptionModel.AccountFriendsPair.Count == 0
                ? new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages")
                : new SelectAccountDetailsControl(hiddenColumnList, model);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    SelectOptionModel.AccountFriendsPair.Clear();
                    SelectOptionModel.ListCustomDetailsUrl.Clear();

                    model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountFriendspair = model.AccountFriendsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            SelectOptionModel.AccountFriendsPair.AddRange(accountFriendspair);
                            SelectOptionModel.ListCustomDetailsUrl.AddRange(accountFriendspair.Select(z => z.Value)
                                .ToList());
                            SelectOptionModel.ListCustomDetailsUrl =
                                SelectOptionModel.ListCustomDetailsUrl.Distinct().ToList();
                            SelectOptionModel.ListCustomDetailsUrl.ForEach(z =>
                            {
                                if (!SelectOptionModel.CustomDetailsText.Contains(z))
                                    SelectOptionModel.CustomDetailsText += z + "\r\n";
                            });
                        }
                        else if (model.AccountFriendsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "",
                                "Destiation is selected but Account is not selected");
                        }
                    });


                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();

            SelectOptionModel.SelectAccountDetailsModel = selectAccountDetailsControl.GetSelectAccountModel();
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }


        private void UserInputOnSaveExecute(object sender)
        {
            if (SelectedDetailsOption == Application.Current.FindResource("LangKeySelectFriends")?.ToString()
                || !string.IsNullOrEmpty(SelectOptionModel.CustomDetailsText)
                || !string.IsNullOrEmpty(SelectOptionModel.CustomDetailsText))
                DominatorHouseCore.LogHelper.GlobusLogHelper.log.Info(DominatorHouseCore.Utility.Log.CustomMessage
                    , DominatorHouseCore.Enums.SocialNetworks.Facebook, "N/A", "N/A", "LangKeyDataSaved".FromResourceDictionary());

            if (SelectedDetailsOption == Application.Current.FindResource("LangKeySelectFriends")?.ToString())
                SaveFriends();
            else if (SelectedDetailsOption == Application.Current.FindResource("LangKeySelectPages")?.ToString())
                SavePages();
            else if (!string.IsNullOrEmpty(SelectOptionModel.CustomDetailsText))
                SelectOptionModel.ListCustomDetailsUrl =
                    Regex.Split(SelectOptionModel.CustomDetailsText, "\r\n").ToList();
            else
                Dialog.ShowDialog(this, "Error", "There is no data to save.");
        }

        private void SavePages()
        {
            try
            {
                var pageUrlList = Regex.Split(SelectOptionModel.CustomDetailsText, "\r\n");
                pageUrlList.ForEach(x => { SelectOptionModel.ListCustomDetailsUrl.Add(x); });

                SelectOptionModel.ListCustomDetailsUrl = SelectOptionModel.ListCustomDetailsUrl.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveFriends()
        {
            try
            {
                var pageUrlList = Regex.Split(SelectOptionModel.CustomDetailsText, "\r\n");
                pageUrlList.ForEach(x => { SelectOptionModel.ListCustomDetailsUrl.Add(x); });

                SelectOptionModel.ListCustomDetailsUrl = SelectOptionModel.ListCustomDetailsUrl.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}