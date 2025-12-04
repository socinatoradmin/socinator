using DominatorHouseCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RedditDominatorCore.RDLibrary
{
    public interface IRdAdScrapperProcess
    {
        Task<bool> ScrapeAdsAsync(DominatorAccountModel account, CancellationToken token);
    }
    public class RdAdScrapperProcess : IRdAdScrapperProcess
    {
        public IRedditFunction redditFunction;
        public RdAdScrapperProcess(IRedditFunction function)
        {
            redditFunction = function;
        }
        public DominatorAccountModel Account { get; set; }

        public CancellationToken Token { get; set; }
        public async Task<bool> ScrapeAdsAsync(DominatorAccountModel account, CancellationToken token)
        {
            //var count = RandomUtilties.GetRandomNumber(20,10);
            //await redditFunction.StartScrapingAds(account, count);
            return true;
        }
    }
}
