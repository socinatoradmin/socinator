#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    internal class LinkedinScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDScraperUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(LDScraperUserQueryParameters)).Cast<LDScraperUserQueryParameters>().ForEach(query =>
            {
                if (query != LDScraperUserQueryParameters.Input)
                    listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }

    internal class LinkedinUserScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDScraperUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>
            {
                LDScraperUserQueryParameters.Keyword.GetDescriptionAttr()?.FromResourceDictionary(),
                LDScraperUserQueryParameters.ProfileUrl.GetDescriptionAttr()?.FromResourceDictionary(),
                LDScraperUserQueryParameters.SearchUrl.GetDescriptionAttr()?.FromResourceDictionary()
            };


            return listQueryType;
        }
    }
}