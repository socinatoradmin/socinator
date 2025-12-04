using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookMarketPlaceActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(MarketPlaceQueryParameter);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(MarketPlaceQueryParameter)).ToList();
        }
    }
}
