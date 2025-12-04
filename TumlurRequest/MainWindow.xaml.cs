using FirstFloor.ModernUI.Windows.Controls;
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
using DominatorHouseCore.Models;
using Tumblr.TumblrLibrary;
using Tumblr.TumblrRequest;

namespace Tumblr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {


            InitializeComponent();



            DominatorAccountModel dominatorAccountModel = new DominatorAccountModel();
            dominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel();

            dominatorAccountModel.AccountBaseModel.UserName = "vkseng111@gmail.com";
            dominatorAccountModel.AccountBaseModel.Password = "VIsi_0891";
            LoginProcess logInProcess = new LoginProcess();
            logInProcess.LogIn(dominatorAccountModel);




       
        }
    }
}
