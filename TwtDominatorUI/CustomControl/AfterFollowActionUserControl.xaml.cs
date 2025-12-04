using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using TwtDominatorCore.TDModels;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AfterFollowAction.xaml
    /// </summary>
    public partial class AfterFollowActionUserControl : UserControl
    {
        public AfterFollowActionUserControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            InitializeCommands();
        }

        #region commands

        public ICommand CommentWordCommand { get; set; }

        #endregion

        private void InitializeCommands()
        {
            try
            {
                CommentWordCommand = new BaseCommand<object>(sender => true, CommentWordExecute);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region command execute

        private void CommentWordExecute(object obj)
        {
            try
            {
                AfterFollowAction.CommentOnUsersTweetInput = CommentOnUsersTweetInputBox.InputText;
                AfterFollowAction.LstCommentOnUsersTweetInput =
                    Regex.Split(CommentOnUsersTweetInputBox.InputText, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion


        #region Setting up dependency property

        public AfterFollowActionModel AfterFollowAction
        {
            get => (AfterFollowActionModel) GetValue(AfterFollowActionProperty);
            set => SetValue(AfterFollowActionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AfterFollowActionProperty =
            DependencyProperty.Register("AfterFollowAction", typeof(AfterFollowActionModel),
                typeof(AfterFollowActionUserControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        #endregion
    }
}