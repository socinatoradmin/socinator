using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PinDominator.PDViews.PinPoster
{
    /// <summary>
    /// Interaction logic for Poster.xaml
    /// </summary>
    public partial class Poster : UserControl
    {
        public Poster()
        {
            InitializeComponent();
        }
        private static Poster CurrentPoster { get; set; } = null;
        /// <summary>
        /// GetSingeltonObjectPoster is used to get the object of the current user control,
        /// if object is already created then its wont create a new object object, simply it returns already created object,
        /// otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Poster GetSingeltonObjectPoster()
        {
            return CurrentPoster ?? (CurrentPoster = new Poster());
        }
    }
}
