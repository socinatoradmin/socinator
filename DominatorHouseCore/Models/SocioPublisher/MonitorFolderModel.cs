#region

using System;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class MonitorFolderModel
    {
        [ProtoMember(1)] public string FolderId { get; set; }

        [ProtoMember(2)] public string FolderPath { get; set; }

        [ProtoMember(3)] public string FilePath { get; set; }

        [ProtoMember(4)] public string FileGuid { get; set; }

        [ProtoMember(5)] public string FileName { get; set; }

        [ProtoMember(6)] public string FileType { get; set; }

        [ProtoMember(7)] public string FileTitle { get; set; }

        [ProtoMember(8)] public string FileCreationDate { get; set; }

        [ProtoMember(9)] public string FileTags { get; set; }

        [ProtoMember(10)] public string FileSubject { get; set; }

        [ProtoMember(11)] public string FileAuthor { get; set; }

        [ProtoMember(12)] public string FileComment { get; set; }

        [ProtoMember(13)] public DateTime PostAddedDateTime { get; set; }
    }
}