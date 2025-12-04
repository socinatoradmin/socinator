#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraAnswerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(AnswerQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(AnswerQueryParameters)).Cast<AnswerQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}