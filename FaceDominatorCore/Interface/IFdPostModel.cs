using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace FaceDominatorCore.Interface
{
    public interface IFdPostModel
    {
        bool IsActionasOwnAccountChecked { get; set; }

        bool IsActionasPageChecked { get; set; }

        List<string> ListOwnPageUrl { get; set; }

        RangeUtilities MaximumCountPerEntity { get; set; }

        bool IsPerEntityRangeChecked { get; set; }

        bool IschkAllowMultipleComment { get; set; }

        LikerCommentorConfigModel LikerCommentorConfigModel { get; set; }

        RangeUtilities MaximumCommentPerPost { get; set; }

        //        string OwnPageUrl { get; set; }


    }
}
