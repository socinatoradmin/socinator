using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for SelectOptionControl.xaml
    /// </summary>
    public partial class SelectOptionControl
    {
        public static readonly DependencyProperty SelectedInputOptionProperty =
            DependencyProperty.Register("SelectedInputOption", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SelectedOptionDisplayNameProperty =
            DependencyProperty.Register("SelectedOptionDisplayName", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstSelectedInputProperty =
            DependencyProperty.Register("LstSelectedInput", typeof(IEnumerable<string>), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSelectButtonVisibleProperty =
            DependencyProperty.Register("IsSelectButtonVisible", typeof(bool), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(SelectOptionControl));
        public SelectOptionControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            DialogParticipation.SetRegister(this, this);
            SaveCommandBinding = new BaseCommand<object>(sender => true, UserInputOnSaveExecute);
            SelectOptionCommandBinding = new BaseCommand<object>(sender => true, o => { });
            LoadTextBoxes();
        }


        public ICommand SaveCommandBinding { get; set; }
        public ICommand ClearCommand { 
            get=>(ICommand)GetValue(ClearCommandProperty); 
            set=>SetValue(ClearCommandProperty,value); 
        }

        public ICommand SelectOptionCommandBinding { get; set; }

        public string SelectedInputOption
        {
            get => (string)GetValue(SelectedInputOptionProperty);
            set => SetValue(SelectedInputOptionProperty, value);
        }

        public string InputText
        {
            get => (string)GetValue(InputTextProperty);
            set => SetValue(InputTextProperty, value);
        }

        public string SelectedOptionDisplayName
        {
            get => (string)GetValue(SelectedOptionDisplayNameProperty);
            set => SetValue(SelectedOptionDisplayNameProperty, value);
        }

        public IEnumerable<string> LstSelectedInput
        {
            get => (IEnumerable<string>)GetValue(LstSelectedInputProperty);
            set => SetValue(LstSelectedInputProperty, value);
        }

        public bool IsSelectButtonVisible
        {
            get => (bool)GetValue(IsSelectButtonVisibleProperty);
            set => SetValue(IsSelectButtonVisibleProperty, value);
        }

        private void LoadTextBoxes()
        {
            if (LstSelectedInput != null)
                foreach (var str in LstSelectedInput)
                    InputText = InputText + str + "\r\n";
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void UserInputOnSaveExecute(object sender)
        {
            if (!string.IsNullOrEmpty(InputText))
            {
                LstSelectedInput = Regex.Split(InputText, "\r\n").Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
                    .Select(y => y.Trim()).Distinct().ToList();
                DominatorHouseCore.LogHelper.GlobusLogHelper.log.Info(DominatorHouseCore.Utility.Log.CustomMessage, DominatorHouseCore.Enums.SocialNetworks.Facebook, "N/A", "N/A", "LangKeyDataSaved".FromResourceDictionary());
            }
            else
                Dialog.ShowDialog(this, "Error", "There is no data to save.");
        }
    }
}