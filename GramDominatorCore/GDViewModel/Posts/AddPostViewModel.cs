//using DominatorHouseCore.Utility;
//using GramDominatorCore.GDModel;
//using System.Collections.ObjectModel;

//namespace GramDominatorCore.GDViewModel.Posts
//{
//   public class AddPostViewModel : BindableBase
//    {
//        private AddPostModel _addPostModel=new AddPostModel();

//        public AddPostModel AddPostModel
//        {
//            get
//            {
//                return _addPostModel;
//            }
//            set
//            {
//                if (_addPostModel == null & _addPostModel == value)
//                    return;
//                SetProperty(ref _addPostModel, value);
//            }
//        }
//        private ObservableCollection<AddPostModel> _lstAddPostModel;// = new ObservableCollection<AddPostModel>(GdBinFileHelper.GetBinFileDetails<AddPostModel>());

//        public ObservableCollection<AddPostModel> LstAddPostModel
//        {
//            get
//            {
//                return _lstAddPostModel;
//            }
//            set
//            {
//                if (_lstAddPostModel == null & _lstAddPostModel == value)
//                    return;
//                SetProperty(ref _lstAddPostModel, value);
//            }
//        }
//    }
//}
