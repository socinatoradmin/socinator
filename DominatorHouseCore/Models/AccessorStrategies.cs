#region

using System;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Models
{
    public class AccessorStrategies
    {
        public Func<SocialNetworks, bool> _determine_available;
        public Action<string> _inform_warnings;
    }
}