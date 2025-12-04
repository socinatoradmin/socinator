#region

using System;
using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class CreateDestination
    {
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public DateTime DestinationCreatedDate { get; set; }

        public IEnumerable<DestinationCollection> DestinationCollections { get; set; }

        public bool IsDestinationSelected { get; set; }
    }
}