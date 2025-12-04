using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.DeleteComment
{
    public class DeleteCommentViewModel : BindableBase
    {
        private DeleteCommentModel _deleteCommentModel=new DeleteCommentModel();

        public DeleteCommentModel DeleteCommentModel
        {
            get
            {
                return _deleteCommentModel;
            }
            set
            {
                if (_deleteCommentModel == null & _deleteCommentModel == value)
                    return;
                SetProperty(ref _deleteCommentModel, value);
            }
        }
        public DeleteCommentModel Model => DeleteCommentModel;
    }
}
