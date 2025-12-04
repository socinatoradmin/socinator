using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookGroupJoinerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GroupJoinerParameter);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(GroupJoinerParameter)).ToList();
        }
    }
}
