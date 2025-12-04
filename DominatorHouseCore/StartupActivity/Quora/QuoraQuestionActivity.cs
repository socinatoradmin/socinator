#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraQuestionActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(QuestionQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(QuestionQueryParameters)).Cast<QuestionQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}