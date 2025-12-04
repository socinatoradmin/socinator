using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using PinDominator.PDViews.Boards;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for BoardsTab.xaml
    /// </summary>
    public partial class BoardsTab
    {
        private static BoardsTab _objBoardsTab;

        public BoardsTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCreateBoard") == null
                        ? "Create Board"
                        : Application.Current.FindResource("LangKeyCreateBoard")?.ToString(),
                    Content = new Lazy<UserControl>(CreateBoard.GetSingletonObjectCreateBoard)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAcceptBoardInvitation") == null
                        ? "Accept Board Invitation"
                        : Application.Current.FindResource("LangKeyAcceptBoardInvitation")?.ToString(),
                    Content = new Lazy<UserControl>(AcceptBoardInvitation.GetSingletonObjectAcceptBoardInvitation)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySendBoardInvitation") == null
                        ? "Send Board Invitation"
                        : Application.Current.FindResource("LangKeySendBoardInvitation")?.ToString(),
                    Content = new Lazy<UserControl>(SendBoardInvitation.GetSingletonObjectSendBoardInvitation)
                }
            };
            BoardTabs.ItemsSource = tabItems;
        }

        public static BoardsTab GetSingletonObjectCreateBoardTab()
        {
            return _objBoardsTab ?? (_objBoardsTab = new BoardsTab());
        }

        public void SetIndex(int index)
        {
            BoardTabs.SelectedIndex = index;
        }
    }
}