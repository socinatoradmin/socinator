#region

using System;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums.SocioPublisher;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherPostFetchModel
    {
        /// <summary>
        ///     To specify the campaign Id
        /// </summary>
        [ProtoMember(1)]
        public string CampaignId { get; set; } = string.Empty;

        /// <summary>
        ///     To specify the campaign Name
        /// </summary>
        [ProtoMember(2)]
        public string CampaignName { get; set; } = string.Empty;

        /// <summary>
        ///     To Specify the post source
        /// </summary>
        [ProtoMember(3)]
        public PostSource PostSource { get; set; } = PostSource.ScrapedPost;

        /// <summary>
        ///     To Specify the filter details each post source
        /// </summary>
        [ProtoMember(4)]
        public string PostDetailsWithFilters { get; set; } = string.Empty;

        /// <summary>
        ///     To Specify the expire date of the post
        /// </summary>
        [ProtoMember(5)]
        public DateTime? ExpireDate { get; set; } = DateTime.Now.AddYears(1);

        /// <summary>
        ///     To Specify the next fetching delay
        /// </summary>
        [ProtoMember(6)]
        public int DelayForNext { get; set; } = 30;


        /// <summary>
        ///     To Specify the maximum capacity of the post items per campaign
        /// </summary>
        [ProtoMember(7)]
        public int MaximumPostLimitToStore { get; set; }


        /// <summary>
        ///     To Specify all selected destinations
        /// </summary>
        [ProtoMember(8)]
        public ObservableCollection<string> SelectedDestinations { get; set; } = new ObservableCollection<string>();

        /// <summary>
        ///     To Specify the notification count post contails lesser than specific range
        /// </summary>
        [ProtoMember(9)]
        public int NotifyCount { get; set; }


        [ProtoMember(10)] public int ScrapeCount { get; set; }
    }
}