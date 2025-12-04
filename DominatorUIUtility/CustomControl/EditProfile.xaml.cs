using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : INotifyPropertyChanged
    {
        private EditProfileViewModel _editProfileViewModel = new EditProfileViewModel();

        public EditProfile()
        {
            InitializeComponent();
        }

        public EditProfile(EditProfileModel editProfileModel) : this()
        {
            EditProfileViewModel.EditProfileModel = editProfileModel;
            DataContext = EditProfileViewModel;
        }

        public EditProfileViewModel EditProfileViewModel
        {
            get => _editProfileViewModel;
            set
            {
                _editProfileViewModel = value;
                OnPropertyChanged(nameof(EditProfileViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnSubmit.IsDefault = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}