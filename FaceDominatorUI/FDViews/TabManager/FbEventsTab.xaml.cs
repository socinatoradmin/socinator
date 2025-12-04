namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbEventsTab.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class FbEventsTab
    {
        public FbEventsTab()
        {
            InitializeComponent();
        }

        //        private static FbEventsTab CurrentFbInviterTab { get; set; }

        // ReSharper disable once IdentifierTypo
        /*
                public static FbEventsTab GetSingeltonObjectFbInviterTab()
                {
                    return CurrentFbInviterTab ?? (CurrentFbInviterTab = new FbEventsTab());
                }
        */

        internal void setIndex(int tabIndex)
        {
            FbEventsTabs.SelectedIndex = tabIndex;
        }
    }
}