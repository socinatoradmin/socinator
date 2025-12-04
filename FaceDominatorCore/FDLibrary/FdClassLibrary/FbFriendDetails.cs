using ProtoBuf;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    [ProtoContract]
    public class FbFriendDetails
    {
        [ProtoMember(1)]
        public string AccountName { get; internal set; } = string.Empty;

        [ProtoMember(2)]
        public string FriendId { get; internal set; } = string.Empty;

        [ProtoMember(3)]
        public string FullName { get; internal set; } = string.Empty;

        [ProtoMember(4)]
        public string ProfileUrl { get; internal set; } = string.Empty;

        [ProtoMember(5)]
        public string InteractionDate { get; internal set; } = string.Empty;

    }
}
