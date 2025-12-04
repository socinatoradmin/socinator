using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for AddBoards.xaml
    /// </summary>
    public partial class AddBoards
    {
        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(AddBoards),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });


        //Using a DependencyProperty as the backing store for ListQueryInfo.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListBoardInfoProperty =
            DependencyProperty.Register("ListBoardInfo", typeof(ObservableCollectionBase<BoardInfo>), typeof(AddBoards),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListCategoryTypeTypeProperty =
            DependencyProperty.Register("ListCategoryType", typeof(IEnumerable<string>), typeof(AddBoards),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for CurrentBoard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentBoardProperty =
            DependencyProperty.Register("CurrentBoard", typeof(BoardInfo), typeof(AddBoards),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportFromCsvProperty =
            DependencyProperty.Register("ImportFromCsvCommand", typeof(ICommand), typeof(AddBoards));
        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LearnMorePrivacyProperty =
            DependencyProperty.Register("LearnMoreExecuteCommand", typeof(ICommand), typeof(AddBoards));
        // Using a DependencyProperty as the backing store for AddBoardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddBoardCommandProperty =
            DependencyProperty.Register("AddBoardCommand", typeof(ICommand), typeof(AddBoards));

        // Using a DependencyProperty as the backing store for DeleteBoardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteBoardCommandProperty =
            DependencyProperty.Register("DeleteBoardCommand", typeof(ICommand), typeof(AddBoards));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AddBoards),
                new FrameworkPropertyMetadata());

        public AddBoards()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            CurrentBoard = new BoardInfo();
            IsExpanded = true;
            ListBoardInfo = new ObservableCollectionBase<BoardInfo>();
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public ObservableCollectionBase<BoardInfo> ListBoardInfo
        {
            get => (ObservableCollectionBase<BoardInfo>) GetValue(ListBoardInfoProperty);
            set => SetValue(ListBoardInfoProperty, value);
        }

        public IEnumerable<string> ListCategoryType
        {
            get => (IEnumerable<string>) GetValue(ListCategoryTypeTypeProperty);
            set => SetValue(ListCategoryTypeTypeProperty, value);
        }

        public BoardInfo CurrentBoard
        {
            get => (BoardInfo) GetValue(CurrentBoardProperty);
            set => SetValue(CurrentBoardProperty, value);
        }

        public ICommand ImportFromCsvCommand
        {
            get => (ICommand) GetValue(ImportFromCsvProperty);
            set => SetValue(ImportFromCsvProperty, value);
        }
        public ICommand LearnMoreExecuteCommand
        {
            get => (ICommand)GetValue(LearnMorePrivacyProperty);
            set=>SetValue(LearnMorePrivacyProperty, value);
        }
        public ICommand AddBoardCommand
        {
            get => (ICommand) GetValue(AddBoardCommandProperty);
            set => SetValue(AddBoardCommandProperty, value);
        }


        public ICommand DeleteBoardCommand
        {
            get => (ICommand) GetValue(DeleteBoardCommandProperty);
            set => SetValue(DeleteBoardCommandProperty, value);
        }


        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public void Clear()
        {
            TxtBoardName.Clear();
            TxtBoardDescription.Clear();
        }


        #region Delete the query from query list

        private static readonly RoutedEvent DeleteQueryEvent = EventManager.RegisterRoutedEvent("DeleteQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddBoards));

        public event RoutedEventHandler DeleteQuery
        {
            add => AddHandler(DeleteQueryEvent, value);
            remove => RemoveHandler(DeleteQueryEvent, value);
        }

        #endregion
    }
}