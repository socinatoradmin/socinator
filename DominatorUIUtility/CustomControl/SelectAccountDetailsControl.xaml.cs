using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectAccountDetailsControl.xaml
    /// </summary>
    public partial class SelectAccountDetailsControl : INotifyPropertyChanged
    {
        private static SelectAccountDetailsControl _indexPage;

        public static readonly RoutedEvent SaveDetailsChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SaveDetailsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(SelectAccountDetailsControl));

        private SelectAccountDetailsViewModel _selectAccountDetailsViewModel = new SelectAccountDetailsViewModel();
        private List<FbEntityTypes> hiddenColumnList;
        private SelectAccountDetailsModel model;

        public SelectAccountDetailsControl()
        {
            InitializeComponent();
            SelectDetails.DataContext = SelectAccountDetailsViewModel;
        }

        public SelectAccountDetailsControl(List<FbEntityTypes> listColumnName, string selectedAccount,
            bool isSingleAccount, string pageHeaderText, bool isFanpage = false)
        {
            InitializeComponent();

            if (isSingleAccount)
            {
                SelectAccountDetailsViewModel.IsSelectedSingleAccount = true;
                SelectAccountDetailsViewModel.DisplayAccount = selectedAccount;
            }

            SelectAccountDetailsViewModel.InitializeDestinationList();

            if (listColumnName.Contains(FbEntityTypes.Group))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.GroupColWidth = "0";
                SelectAccountDetailsViewModel.GroupColWidth = "0";
            }

            if (listColumnName.Contains(FbEntityTypes.Page))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.PageColWidth = "0";
                SelectAccountDetailsViewModel.PageColWidth = "0";
            }

            if (listColumnName.Contains(FbEntityTypes.Friend))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.FriendColWidth = "0";
                SelectAccountDetailsViewModel.FriendColWidth = "0";
            }

            if (listColumnName.Contains(FbEntityTypes.CustomDestination))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.CustomDestinationColWidth = "0";
                SelectAccountDetailsViewModel.CustomColumnWidth = "0";
            }

            if (!string.IsNullOrEmpty(pageHeaderText))
                SelectAccountDetailsViewModel.InviteForPagesText = "Select Pages";

            SelectAccountDetailsViewModel.IsFanpage = isFanpage;


            SelectDetails.DataContext = SelectAccountDetailsViewModel;
        }

        public SelectAccountDetailsControl(SelectAccountDetailsModel selctAccountDetailsModel, bool isFanpage = false)
        {
            InitializeComponent();

            //SelectAccountDetailsViewModel = new SelectAccountDetailsViewModel();
            SelectAccountDetailsViewModel.SelectAccountDetailsModel = selctAccountDetailsModel;

            if (selctAccountDetailsModel.IsDisplaySingleAccount)
                SelectAccountDetailsViewModel.RemoveUnnecessaryDestinationList();

            SelectAccountDetailsViewModel.GroupColWidth = selctAccountDetailsModel.GroupColWidth;
            SelectAccountDetailsViewModel.FriendColWidth = selctAccountDetailsModel.FriendColWidth;
            SelectAccountDetailsViewModel.PageColWidth = selctAccountDetailsModel.PageColWidth;
            SelectAccountDetailsViewModel.CustomColumnWidth = selctAccountDetailsModel.CustomDestinationColWidth;
            SelectAccountDetailsViewModel.IsFanpage = isFanpage;
            SelectAccountDetailsViewModel.EditDestination();
            SelectDetails.DataContext = SelectAccountDetailsViewModel;
        }

        public SelectAccountDetailsControl(List<FbEntityTypes> hiddenColumnList, SelectAccountDetailsModel model
            , bool isFanpage = false)
        {
            InitializeComponent();
            SelectAccountDetailsViewModel.SelectAccountDetailsModel = model;

            if (hiddenColumnList.Contains(FbEntityTypes.Group))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.GroupColWidth = "0";
                SelectAccountDetailsViewModel.GroupColWidth = "0";
            }

            if (hiddenColumnList.Contains(FbEntityTypes.Page))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.PageColWidth = "0";
                SelectAccountDetailsViewModel.PageColWidth = "0";
            }

            if (hiddenColumnList.Contains(FbEntityTypes.Friend))
            {
                SelectAccountDetailsViewModel.SelectAccountDetailsModel.FriendColWidth = "0";
                SelectAccountDetailsViewModel.FriendColWidth = "0";
            }

            SelectAccountDetailsViewModel.InviteForPagesText = "Select Pages";


            SelectAccountDetailsViewModel.IsFanpage = isFanpage;
            SelectAccountDetailsViewModel.EditDestination();
            SelectAccountDetailsViewModel.IsSelectedSingleAccount = false;

            SelectDetails.DataContext = SelectAccountDetailsViewModel;
        }

        public SelectAccountDetailsViewModel SelectAccountDetailsViewModel
        {
            get => _selectAccountDetailsViewModel;
            set
            {
                _selectAccountDetailsViewModel = value;
                OnPropertyChanged(nameof(SelectAccountDetailsViewModel));
            }
        }

        public static SelectAccountDetailsControl Instance { get; set; }
            = _indexPage ?? (_indexPage = new SelectAccountDetailsControl());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PublisherCreateDestination_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!SelectAccountDetailsViewModel.IsSavedDestination)
                // ReSharper disable once RedundantJumpStatement
                return;
        }

        public event RoutedEventHandler SaveDetailsChanged
        {
            add => AddHandler(SaveDetailsChangedRoutedEvent, value);
            remove => RemoveHandler(SaveDetailsChangedRoutedEvent, value);
        }

        public void SaveDetailsChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(SaveDetailsChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        private void Btn_SaveClick(object sender, RoutedEventArgs e)
        {
            SaveDetailsChangedEventHandler();
        }


        public SelectAccountDetailsModel GetSelectAccountModel()
        {
            return SelectAccountDetailsViewModel.SelectAccountDetailsModel;
        }
    }
}