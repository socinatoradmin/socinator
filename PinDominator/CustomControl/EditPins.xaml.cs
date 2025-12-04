using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for EditPins.xaml
    /// </summary>
    public partial class EditPins
    {
        // Using a DependencyProperty as the backing store for CurrentPin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPinProperty =
            DependencyProperty.Register("CurrentPin", typeof(PinInfo), typeof(EditPins),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(EditPins),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListAccounts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListAccountsTypeProperty =
            DependencyProperty.Register("ListAccounts", typeof(IEnumerable<string>), typeof(EditPins),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });


        //Using a DependencyProperty as the backing store for ListPinInfo.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListPinInfoProperty =
            DependencyProperty.Register("ListPinInfo", typeof(ObservableCollectionBase<PinInfo>), typeof(EditPins),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for AddPinCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddPinCommandProperty =
            DependencyProperty.Register("AddPinCommand", typeof(ICommand), typeof(EditPins));

        // Using a DependencyProperty as the backing store for ImportFromCsvCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportFromCsvProperty =
            DependencyProperty.Register("ImportFromCsvCommand", typeof(ICommand), typeof(EditPins));

        public static readonly DependencyProperty SelectAccountVisibilityProperty =
            DependencyProperty.Register("SelectAccountVisibility", typeof(bool), typeof(EditPins),
                new FrameworkPropertyMetadata(true)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EditPins),
                new FrameworkPropertyMetadata());

        // Using a DependencyProperty as the backing store for DeletePinCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeletePinCommandProperty =
            DependencyProperty.Register("DeletePinCommand", typeof(ICommand), typeof(EditPins));

        public EditPins()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            CurrentPin = new PinInfo();
            IsExpanded = true;
            ListPinInfo = new ObservableCollectionBase<PinInfo>();
        }

        public PinInfo CurrentPin
        {
            get => (PinInfo) GetValue(CurrentPinProperty);
            set => SetValue(CurrentPinProperty, value);
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

        public ObservableCollectionBase<PinInfo> ListPinInfo
        {
            get => (ObservableCollectionBase<PinInfo>) GetValue(ListPinInfoProperty);
            set => SetValue(ListPinInfoProperty, value);
        }

        public ICommand AddPinCommand
        {
            get => (ICommand) GetValue(AddPinCommandProperty);
            set => SetValue(AddPinCommandProperty, value);
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

        public ICommand DeletePinCommand
        {
            get => (ICommand) GetValue(DeletePinCommandProperty);
            set => SetValue(DeletePinCommandProperty, value);
        }
    }
}