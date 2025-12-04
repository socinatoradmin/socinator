using DominatorHouseCore.Models;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors
{
    internal interface IQueryProcessor
    {
        void Start(QueryInfo queryInfo);
    }
}