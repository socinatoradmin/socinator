using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookCommentLikerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(CommentLikerParameter);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(CommentLikerParameter)).ToList();
        }
    }
}
