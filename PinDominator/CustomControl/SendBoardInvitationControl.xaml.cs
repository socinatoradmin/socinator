using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for SendBoardInvitationControl.xaml
    /// </summary>
    public partial class SendBoardInvitationControl
    {
        // Using a DependencyProperty as the backing store for CurrentBoardCollaborator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentBoardCollaboratorProperty =
            DependencyProperty.Register("CurrentBoardCollaborator", typeof(BoardCollaboratorInfo),
                typeof(SendBoardInvitationControl), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SendBoardInvitationControl),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListAccounts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListAccountsTypeProperty =
            DependencyProperty.Register("ListAccounts", typeof(IEnumerable<string>), typeof(SendBoardInvitationControl),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });


        //Using a DependencyProperty as the backing store for ListBoardCollaboratorInfo.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListBoardCollaboratorInfoProperty =
            DependencyProperty.Register("ListBoardCollaboratorInfo",
                typeof(ObservableCollectionBase<BoardCollaboratorInfo>), typeof(SendBoardInvitationControl),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for AddBoardCollaboratorCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddBoardCollaboratorProperty =
            DependencyProperty.Register("AddBoardCollaboratorCommand", typeof(ICommand),
                typeof(SendBoardInvitationControl));

        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportFromCsvProperty =
            DependencyProperty.Register("ImportFromCsvCommand", typeof(ICommand), typeof(SendBoardInvitationControl));

        public static readonly DependencyProperty SelectAccountVisibilityProperty =
            DependencyProperty.Register("SelectAccountVisibility", typeof(bool), typeof(SendBoardInvitationControl),
                new FrameworkPropertyMetadata(true)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SendBoardInvitationControl),
                new FrameworkPropertyMetadata());

        // Using a DependencyProperty as the backing store for DeleteBoardCollaboratorCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteBoardCollaboratorProperty =
            DependencyProperty.Register("DeleteBoardCollaboratorCommand", typeof(ICommand),
                typeof(SendBoardInvitationControl));

        public SendBoardInvitationControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            CurrentBoardCollaborator = new BoardCollaboratorInfo();
            IsExpanded = true;
            ListBoardCollaboratorInfo = new ObservableCollectionBase<BoardCollaboratorInfo>();
        }

        public BoardCollaboratorInfo CurrentBoardCollaborator
        {
            get => (BoardCollaboratorInfo) GetValue(CurrentBoardCollaboratorProperty);
            set => SetValue(CurrentBoardCollaboratorProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }


        public IEnumerable<string> ListAccounts
        {
            get => (IEnumerable<string>) GetValue(ListAccountsTypeProperty);
            set => SetValue(ListAccountsTypeProperty, value);
        }

        public ObservableCollectionBase<BoardCollaboratorInfo> ListBoardCollaboratorInfo
        {
            get => (ObservableCollectionBase<BoardCollaboratorInfo>) GetValue(ListBoardCollaboratorInfoProperty);
            set => SetValue(ListBoardCollaboratorInfoProperty, value);
        }

        public ICommand AddBoardCollaboratorCommand
        {
            get => (ICommand) GetValue(AddBoardCollaboratorProperty);
            set => SetValue(AddBoardCollaboratorProperty, value);
        }

        public ICommand ImportFromCsvCommand
        {
            get => (ICommand) GetValue(ImportFromCsvProperty);
            set => SetValue(ImportFromCsvProperty, value);
        }

        public bool SelectAccountVisibility
        {
            get => (bool) GetValue(SelectAccountVisibilityProperty);
            set => SetValue(SelectAccountVisibilityProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ICommand DeleteBoardCollaboratorCommand
        {
            get => (ICommand) GetValue(DeleteBoardCollaboratorProperty);
            set => SetValue(DeleteBoardCollaboratorProperty, value);
        }
    }
}