using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.Settings
{

    [ProtoContract]
    public class ActivityManager
    {
        [ProtoMember(1)] public ModuleConfiguration FollowModule { get; set; } = new ModuleConfiguration();

        [ProtoMember(2)] public ModuleConfiguration UnfollowModule { get; set; } = new ModuleConfiguration();

        [ProtoMember(3)] public ModuleConfiguration TryModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(4)] public ModuleConfiguration UnlikeModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(5)] public ModuleConfiguration CommentModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(6)] public ModuleConfiguration DeleteCommentModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(7)] public ModuleConfiguration PostingModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(8)] public ModuleConfiguration RepostModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(9)] public ModuleConfiguration DeletePostModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(10)] public ModuleConfiguration MonitorFolderModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(11)] public ModuleConfiguration MessageModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(12)] public ModuleConfiguration UserScraperModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(13)] public ModuleConfiguration PhotoScraperModule { get; set; } = new ModuleConfiguration();


        [ProtoMember(14)] public List<RunningTimes> RunningTime { get; set; }
    }
}