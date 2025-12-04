using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinkedDominatorUI.LDViews.GrowConnection
{
    /// <summary>
    /// Interaction logic for RemoveConnections.xaml
    /// </summary>
    public partial class RemoveConnections : UserControl
    {
        public RemoveConnections()
        {
            InitializeComponent();
            SetDataContext();
        }
        private static RemoveConnections CurrentRemoveConnections { get; set; } = null;
        /// <summary>
        /// GetSingeltonObjectFollower is used to get the object of the current user control,
        /// if object is already created then its won't create a new object, simply it returns already created object,
        /// otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static RemoveConnections GetSingeltonObjectRemoveConnections()
        {
            return CurrentRemoveConnections ?? (CurrentRemoveConnections = new RemoveConnections());
        }

        /// <summary>
        /// Implement the INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        #region IHeaderControl

        private string _campaignName;

        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private bool _isEditCampaignName = true;

        public bool IsEditCampaignName
        {
            get
            {
                return _isEditCampaignName;
            }
            set
            {
                _isEditCampaignName = value;
                OnPropertyChanged(nameof(IsEditCampaignName));
            }
        }


        private Visibility _cancelEditVisibility = Visibility.Collapsed;

        public Visibility CancelEditVisibility
        {
            get
            {
                return _cancelEditVisibility;
            }
            set
            {
                _cancelEditVisibility = value;
                OnPropertyChanged(nameof(CancelEditVisibility));
            }
        }
        private string _templateId;
        public string TemplateId
        {
            get
            {
                return _templateId;
            }

            set
            {
                _templateId = value;
                OnPropertyChanged(nameof(_templateId));
            }
        }

        #endregion

        #region IFooterControl

        private string _selectedAccountCount = ConstantVariable.NoAccountSelected;

        public string SelectedAccountCount
        {
            get
            {
                return _selectedAccountCount;
            }
            set
            {
                _selectedAccountCount = value;
                OnPropertyChanged(nameof(SelectedAccountCount));
            }
        }
        private string _campaignButtonContent = ConstantVariable.CreateCampaign;
        public string CampaignButtonContent
        {
            get
            {
                return _campaignButtonContent;
            }
            set
            {
                _campaignButtonContent = value;
                OnPropertyChanged(nameof(CampaignButtonContent));
            }
        }

        #endregion
        private RemoveConnectionsViewModel _objRemoveConnectionsViewModel;

        public RemoveConnectionsViewModel ObjRemoveConnectionsViewModel
        {
            get
            {
                return _objRemoveConnectionsViewModel;
            }
            set
            {
                _objRemoveConnectionsViewModel = value;
                OnPropertyChanged(nameof(ObjRemoveConnectionsViewModel));
            }
        }
        private void SetDataContext()
        {
            this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
            ObjRemoveConnectionsViewModel = new RemoveConnectionsViewModel();
            RemoveConnectionFooter.list_SelectedAccounts = new List<string>();
            MainGrid.DataContext = ObjRemoveConnectionsViewModel.RemoveConnectionsModel;
            HeaderGrid.DataContext = FooterGrid.DataContext = this;
            CampaignName = $"Remove Connections [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }
        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {

        }
        private void InputBoxControl_OnGetInputClick(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveConnectionFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var objSelectAccountControl = new SelectAccountControl(RemoveConnectionFooter.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    if (objSelectAccountControl.GetSelectedAccount().Count > 0)
                    {
                        RemoveConnectionFooter.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        this.SelectedAccountCount = RemoveConnectionFooter.list_SelectedAccounts.Count + " Account Selected";
                    }
                    else
                    {
                        this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
                    }
                    window.Close();
                };

                objSelectAccountControl.btnCancel.Click += (senders, events) => window.Close();
                window.Height = 300;
                window.Height = 500;
                window.ShowDialog();
            }
            catch (Exception Ex)
            { }
        }

        private void RemoveConnectionFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveConnectionFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
