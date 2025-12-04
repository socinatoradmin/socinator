#region

using System.Windows.Controls;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PublisherHomeModel : BindableBase
    {
        private UserControl _selectedUserControl;

        public UserControl SelectedUserControl
        {
            get => _selectedUserControl;
            set
            {
                if (_selectedUserControl != null && Equals(_selectedUserControl, value))
                    return;

                SetProperty(ref _selectedUserControl, value);
            }
        }
    }
}