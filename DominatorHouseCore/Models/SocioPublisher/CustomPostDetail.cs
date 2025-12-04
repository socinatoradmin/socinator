using DominatorHouseCore.Enums;
using System;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class CustomPostDetail
    {
        public PostType PostType {  get; set; }
        public int LikesCount {  get; set; }
        public DateTime PostedDate { get; set; }
    }
}
