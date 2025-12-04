using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookProfileActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(FdProfileQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(FdProfileQueryParameters)).ToList();
        }
    }
}
