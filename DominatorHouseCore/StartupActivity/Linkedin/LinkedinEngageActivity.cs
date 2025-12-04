#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    internal class LinkedinEngageActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDEngageQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(LDEngageQueryParameters)).Cast<LDEngageQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}