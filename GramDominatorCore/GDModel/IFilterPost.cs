namespace GramDominatorCore.GDModel
{
    public interface IFilterPost
    {
       // ObservableCollectionCustom<string> CaptionBlackList { get; set; }

       // int CaptionMinChars { get; set; }

        RangeHelper CommentRange { get; set; }

//bool FilterCaptionBlacklist { get; set; }

        bool FilterComments { get; set; }

     //   bool FilterLikes { get; set; }

//bool FilterPostAge { get; set; }

      //  RangeHelper LikeRange { get; set; }

       // int MaxPostAge { get; set; }

       // FilterPostType PostType { get; set; }
    }
}
