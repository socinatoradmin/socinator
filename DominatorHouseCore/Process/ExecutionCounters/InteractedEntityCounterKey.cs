#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Process.ExecutionCounters
{
    public class InteractedEntityCounterKey
    {
        public Type InteractedEntityType { get; }
        public SocialNetworks SocialNetworks { get; }
        public string AccountId { get; }

        public InteractedEntityCounterKey(Type interactedEntityType, SocialNetworks socialNetworks, string accountId)
        {
            InteractedEntityType = interactedEntityType;
            SocialNetworks = socialNetworks;
            AccountId = accountId;
        }

        public override bool Equals(object obj)
        {
            return obj is InteractedEntityCounterKey key &&
                   EqualityComparer<Type>.Default.Equals(InteractedEntityType, key.InteractedEntityType) &&
                   SocialNetworks == key.SocialNetworks &&
                   AccountId == key.AccountId;
        }

        public override int GetHashCode()
        {
            var hashCode = 1322340231;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(InteractedEntityType);
            hashCode = hashCode * -1521134295 + SocialNetworks.GetHashCode();
            hashCode = hashCode * -1521134295 + AccountId.GetHashCode();
            return hashCode;
        }
    }
}