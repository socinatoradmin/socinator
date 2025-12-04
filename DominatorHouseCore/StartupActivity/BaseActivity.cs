#region

using System;
using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.StartupActivity
{
    public abstract class BaseActivity
    {
        public abstract List<string> GetQueryType();
        public abstract Type GetEnumType();
    }
}