#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Pinterest
{
    public class PinterestUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PDUsersQueries);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(PDUsersQueries)).Cast<PDUsersQueries>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}