using System.Windows.Controls;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    ///     Interaction logic for ErrorHandling.xaml
    /// </summary>
    public partial class ErrorHandling : UserControl
    {
        private static ErrorHandling ObjErrorHandling;

        private ErrorHandling()
        {
            InitializeComponent();
        }

        public static ErrorHandling GetSingeltonErrorHandlingObject()
        {
            if (ObjErrorHandling == null)
                ObjErrorHandling = new ErrorHandling();
            return ObjErrorHandling;
        }
    }
}