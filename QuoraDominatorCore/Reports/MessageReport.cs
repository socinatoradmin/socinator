using System;

namespace QuoraDominatorCore.Reports
{
    public class MessageReport
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime InteractionDateTime { get; set; }
        public string Account { get; set; }
    }
}