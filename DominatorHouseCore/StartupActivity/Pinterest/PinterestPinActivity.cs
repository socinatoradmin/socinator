#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Pinterest
{
    public class PinterestPinActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PDPinQueries);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(PDPinQueries)).Cast<PDPinQueries>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}