using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.Publisher.AdvancedOptions
{
    /// <summary>
    ///     Interaction logic for InstagramAdvancedOptions.xaml
    /// </summary>
    public partial class InstagramAdvancedOptions : UserControl
    {
        public InstagramAdvancedOptions()
        {
            InitializeComponent();
            var ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            var LocationDetailFilePath =
                ConstantVariable.GetConfigurationDir(SocialNetworks.Instagram) + "\\LocationsDetail.bin";

            //ObjAddPosts.AddPostViewModel.AddPostModel.LocationDetailsCollection =CollectionViewSource.GetDefaultView(
            //    ProtoBuffBase.DeserializeObjects<LocationDetails>(LocationDetailFilePath));
            MainGrid.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel;
        }
    }
}