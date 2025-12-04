#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Process.ExecutionCounters
{
    public class InteractionPerActivityCounterKey : InteractedEntityCounterKey
    {
        public string ActivityType { get; }

        public InteractionPerActivityCounterKey(Type interactedEntityType, SocialNetworks socialNetworks,
            string accountId,
            string activityType) : base(interactedEntityType, socialNetworks, accountId)
        {
            ActivityType = activityType;
        }

        public override bool Equals(object obj)
        {
            return obj is InteractionPerActivityCounterKey key &&
                   base.Equals(obj) &&
                   ActivityType == key.ActivityType;
        }

        public override int GetHashCode()
        {
            var hashCode = 679152378;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ActivityType);
            return hashCode;
        }
    }
}