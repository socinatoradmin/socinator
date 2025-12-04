using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookPlaceActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PlaceQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(PlaceQueryParameters)).Cast<PlaceQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
