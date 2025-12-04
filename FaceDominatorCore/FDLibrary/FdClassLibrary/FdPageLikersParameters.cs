using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdPageLikersParameters
    {

        public bool IsPagination { get; set; }

        public string FinalEncodedQuery { get; set; }

        public string PaginationData { get; set; }

        /*
                public string AjaxToken { get; set; }
        */

        public List<FacebookUser> LstFacebookUser { get; set; } = new List<FacebookUser>();

    }
}
