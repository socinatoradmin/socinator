using System.ComponentModel;
using System.Windows;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for MigrationProgress.xaml
    /// </summary>
    public partial class MigrationProgress
    {
        public MigrationProgress()
        {
            InitializeComponent();
        }

        private void MigrationProgress_OnLoaded(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            var worker = new BackgroundWorker();
            worker.DoWork += Migrating;
            worker.RunWorkerCompleted += MigratingCompleted;
            worker.RunWorkerAsync();
        }

        private void MigratingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressRing.IsActive = false;
            Dialog.CloseDialog(this);
        }

        private void Migrating(object sender, DoWorkEventArgs e)
        {
            for (var i = 0; i < 2000000000; i++)
            {
            }
        }
    }
}