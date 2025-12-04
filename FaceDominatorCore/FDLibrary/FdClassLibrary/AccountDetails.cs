using ProtoBuf;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    [ProtoContract]
    public class FdAccountDetails
    {
        [ProtoMember(1)]
        public string AccountNetwork { get; set; }

        [ProtoMember(2)]
        public string Category { get; set; }

        [ProtoMember(3)]
        public string AccountId { get; set; }

        [ProtoMember(4)]
        public string AccountName { get; set; }

        /*
                [ProtoMember(5)]
                public string DetailsId { get; set; }
        */

        /*
                [ProtoMember(6)]
                public string DetailsName { get; set; }
        */
    }
}
