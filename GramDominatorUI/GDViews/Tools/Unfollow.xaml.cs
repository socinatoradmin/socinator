using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GramDominatorCore.GDViewModel.GrowFollower;
using GramDominatorUI.Annotations;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    /// Interaction logic for Unfollow.xaml
    /// </summary>
    public partial class Unfollow : UserControl, INotifyPropertyChanged
    {
        public Unfollow()
        {
            InitializeComponent();
            MainGrid.DataContext = objUnfollowerViewModel.UnfollowerModel;
        }
        #region Object creation and INotifyPropertyChanged Implementation

        /// <summary>
        /// UnfollowerViewModel Object creation
        /// </summary>

        private UnfollowerViewModel _objUnfollowerViewModel = new UnfollowerViewModel();

        public UnfollowerViewModel objUnfollowerViewModel
        {
            get
            {
                return _objUnfollowerViewModel;
            }
            set
            {
                _objUnfollowerViewModel = value;
                OnPropertyChanged(nameof(objUnfollowerViewModel));
            }
        }

        private static Unfollow CurrentUnfollow { get; set; } = null;

        /// <summary>
        /// GetSingeltonObjectUnfollow is used to get the object of the current user control,
        /// if object is already created then its wont create a new object object, simply it returns already created object,
        /// otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Unfollow GetSingeltonObjectUnfollow()
        {
            return CurrentUnfollow ?? (CurrentUnfollow = new Unfollow());
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


        #endregion

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void accountgrothHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetDataContext();
        }
        private void SetDataContext()
        {
            MainGrid.DataContext = objUnfollowerViewModel.UnfollowerModel;
        }
    }
}
