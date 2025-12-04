#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(UserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(UserQueryParameters)).Cast<UserQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}