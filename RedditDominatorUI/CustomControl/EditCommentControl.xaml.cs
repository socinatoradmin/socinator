using DominatorHouseCore.Utility;
using RedditDominatorCore.RDModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RedditDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for EditComment.xaml
    /// </summary>
    public partial class EditCommentControl : UserControl
    {
        // Using a DependencyProperty as the backing store for CurrentComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentEditCommentProperty =
            DependencyProperty.Register("EditCommentInfo", typeof(EditCommentInfo), typeof(EditCommentControl),
                new PropertyMetadata(OnAvailableItemsChanged));

        // Using a DependencyProperty as the backing store for ListAccounts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListAccountsTypeProperty =
            DependencyProperty.Register("ListAccounts", typeof(IEnumerable<string>), typeof(EditCommentControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        //Using a DependencyProperty as the backing store for ListEditCommentInfo.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListEditCommentInfoProperty =
            DependencyProperty.Register("ListEditCommentInfo", typeof(ObservableCollectionBase<EditCommentInfo>),
                typeof(EditCommentControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for AddCommentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommentCommandProperty =
            DependencyProperty.Register("AddCommentCommand", typeof(ICommand), typeof(EditCommentControl));

        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportFromCsvProperty =
            DependencyProperty.Register("ImportFromCsvCommand", typeof(ICommand), typeof(EditCommentControl));

        public static readonly DependencyProperty SelectAccountVisibilityProperty =
            DependencyProperty.Register("SelectAccountVisibility", typeof(bool), typeof(EditCommentControl),
                new FrameworkPropertyMetadata(true)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EditCommentControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        // Using a DependencyProperty as the backing store for DeletePinCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteCommentCommandProperty =
            DependencyProperty.Register("DeleteCommentCommand", typeof(ICommand), typeof(EditCommentControl));

        public EditCommentControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public EditCommentInfo EditCommentInfo
        {
            get => (EditCommentInfo)GetValue(CurrentEditCommentProperty);
            set => SetValue(CurrentEditCommentProperty, value);
        }

        public IEnumerable<string> ListAccounts
        {
            get => (IEnumerable<string>)GetValue(ListAccountsTypeProperty);
            set => SetValue(ListAccountsTypeProperty, value);
        }

        public ObservableCollectionBase<EditCommentInfo> ListEditCommentInfo
        {
            get => (ObservableCollectionBase<EditCommentInfo>)GetValue(ListEditCommentInfoProperty);
            set => SetValue(ListEditCommentInfoProperty, value);
        }

        public ICommand AddCommentCommand
        {
            get => (ICommand)GetValue(AddCommentCommandProperty);
            set => SetValue(AddCommentCommandProperty, value);
        }

        public ICommand ImportFromCsvCommand
        {
            get => (ICommand)GetValue(ImportFromCsvProperty);
            set => SetValue(ImportFromCsvProperty, value);
        }

        public bool SelectAccountVisibility
        {
            get => (bool)GetValue(SelectAccountVisibilityProperty);
            set => SetValue(SelectAccountVisibilityProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ICommand DeleteCommentCommand
        {
            get => (ICommand)GetValue(DeleteCommentCommandProperty);
            set => SetValue(DeleteCommentCommandProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
    }
}