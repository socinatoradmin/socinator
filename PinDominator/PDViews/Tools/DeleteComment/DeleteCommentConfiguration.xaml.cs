using PinDominatorCore.PDViewModel.DeleteComment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PinDominator.PDViews.Tools.DeleteComment
{
    /// <summary>
    /// Interaction logic for DeleteCommentConfiguration.xaml
    /// </summary>
    public partial class DeleteCommentConfiguration : UserControl, INotifyPropertyChanged
    {
        public DeleteCommentConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = ObjDeleteCommentViewModel.ObjDeleteCommentModel;
        }
        private static DeleteCommentConfiguration CurrentDeleteCommentConfiguration { get; set; } = null;

        /// <summary>
        /// USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF DeleteCommentConfiguration
        /// </summary>
        /// <returns></returns>
        public static DeleteCommentConfiguration GetSingeltonObjectDeleteCommentConfiguration()
        {
            return CurrentDeleteCommentConfiguration ?? (CurrentDeleteCommentConfiguration = new DeleteCommentConfiguration());
        }


        /// <summary>
        /// Implement the INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            private void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private DeleteCommentViewModel _objDeleteCommentViewModel = new DeleteCommentViewModel();

        public DeleteCommentViewModel ObjDeleteCommentViewModel
        {
            get
            {
                return _objDeleteCommentViewModel;
            }
            set
            {
                _objDeleteCommentViewModel = value;
                OnPropertyChanged(nameof(_objDeleteCommentViewModel));
            }
        }
    }
}
