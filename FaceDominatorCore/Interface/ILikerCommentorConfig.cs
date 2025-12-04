using DominatorHouseCore.Models.FacebookModels;

namespace FaceDominatorCore.Interface
{
    public interface ILikerCommentorConfig
    {

        /*bool IsLikeTypeFilterChkd { get; set; }

        bool IsLikeFilterChkd { get; set; }

        bool IsLoveFilterChkd { get; set; }

        bool IsHahaFilterChkd { get; set; }

        bool IsWowFilterChkd { get; set; }

        bool IsSadFilterChkd { get; set; }

        bool IsAngryFilterChkd { get; set; }

        bool IsCommentFilterChecked { get; set; }

        bool IsSpintaxChecked { get; set; }

        ObservableCollectionBase<ManageCustomCommentsModel> SavedComments { get; set; }*/

        ManageCustomCommentsModel CurrentCommment { get; set; }

    }
}
