using DominatorHouseCore.Enums;
using System;

namespace GramDominatorCore.GDModel
{
    public class InteractedPost
    {
        public int Id { get; set; }

        public DateTime InteractionDate { get; set; }

        public MediaType MediaType { get; set; }

        public ActivityType ActivityType { get; set; }

       // public string PkOwner { get; set; }

       // public int TakenAt { get; set; }

      //  public string UsernameOwner { get; set; }

        public string AccountUsername { get; set; }

        public string Comment { get; set; }
    }
}
