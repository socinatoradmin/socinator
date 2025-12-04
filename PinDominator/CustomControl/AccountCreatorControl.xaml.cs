using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountCreatorControl.xaml
    /// </summary>
    public partial class AccountCreatorControl
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CreateAccountInfoProperty =
            DependencyProperty.Register("CreateAccountInfo", typeof(CreateAccountInfo), typeof(AccountCreatorControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstCreateAccountInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstCreateAccountInfoProperty =
            DependencyProperty.Register("LstCreateAccountInfo", typeof(ObservableCollectionBase<CreateAccountInfo>),
                typeof(AccountCreatorControl), new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for AddCreateAccountCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCreateAccountCommandProperty =
            DependencyProperty.Register("AddCreateAccountCommand", typeof(ICommand), typeof(AccountCreatorControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportFromCsvCommandProperty =
            DependencyProperty.Register("ImportFromCsvCommand", typeof(ICommand), typeof(AccountCreatorControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AccountCreatorControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for DeleteAccountCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteAccountCommandProperty =
            DependencyProperty.Register("DeleteAccountCommand", typeof(ICommand), typeof(AccountCreatorControl),
                new PropertyMetadata());
        public static readonly DependencyProperty EditAccountCommandProperty =
            DependencyProperty.Register("EditAccountCommand", typeof(ICommand), typeof(AccountCreatorControl),
                new PropertyMetadata());
        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(AccountCreatorControl),
                new PropertyMetadata());
        public static readonly DependencyProperty SelectedTabProperty =
            DependencyProperty.Register("SelectedTab", typeof(int), typeof(AccountCreatorControl), new FrameworkPropertyMetadata
            {
                DefaultValue = 0
            });
        public AccountCreatorControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            IsExpanded = true;
            CreateAccountInfo = new CreateAccountInfo();
            LstCreateAccountInfo = new ObservableCollectionBase<CreateAccountInfo>();
        }
        public int SelectedTab
        {
            get => (int)GetValue(SelectedTabProperty);
            set => SetValue(SelectedTabProperty, value);
        }
        public CreateAccountInfo CreateAccountInfo
        {
            get => (CreateAccountInfo) GetValue(CreateAccountInfoProperty);
            set => SetValue(CreateAccountInfoProperty, value);
        }


        public ObservableCollectionBase<CreateAccountInfo> LstCreateAccountInfo
        {
            get => (ObservableCollectionBase<CreateAccountInfo>) GetValue(LstCreateAccountInfoProperty);
            set => SetValue(LstCreateAccountInfoProperty, value);
        }


        public ICommand AddCreateAccountCommand
        {
            get => (ICommand) GetValue(AddCreateAccountCommandProperty);
            set => SetValue(AddCreateAccountCommandProperty, value);
        }


        public ICommand ImportFromCsvCommand
        {
            get => (ICommand) GetValue(ImportFromCsvCommandProperty);
            set => SetValue(ImportFromCsvCommandProperty, value);
        }


        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }


        public ICommand DeleteAccountCommand
        {
            get => (ICommand) GetValue(DeleteAccountCommandProperty);
            set => SetValue(DeleteAccountCommandProperty, value);
        }
        public ICommand EditAccountCommand
        {
            get => (ICommand) GetValue(EditAccountCommandProperty);
            set => SetValue(EditAccountCommandProperty, value);
        }


        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}