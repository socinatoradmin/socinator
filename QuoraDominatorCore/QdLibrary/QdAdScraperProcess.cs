using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Models;

namespace QuoraDominatorCore.QdLibrary
{
    public interface IQdAdScraperProcess
    {
        Task<bool> ScrapeAdsAsync(DominatorAccountModel account, CancellationToken token);
    }

    public class QdAdScraperProcess : IQdAdScraperProcess
    {
        public IQuoraFunctions _quoraFunct;

        public QdAdScraperProcess(IQuoraFunctions quoraFunc)
        {
            _quoraFunct = quoraFunc;
        }

        public DominatorAccountModel Account { get; set; }

        public CancellationToken Token { get; set; }

        public async Task<bool> ScrapeAdsAsync(DominatorAccountModel account, CancellationToken token)
        {
            //var count = RandomUtilties.GetRandomNumber(20, 10);
            //var ipDetails = await _quoraFunct.GetIpDetails(account);
            ////await _quoraFunct.SaveUserDetailsInDb(account, ipDetails);
            //var scrapeAds = await _quoraFunct.StartScrapingAds(account, count);

            //foreach (var singleAds in scrapeAds) await _quoraFunct.SaveDetailsInDb(account, singleAds, ipDetails);
            return true;
        }
    }
}