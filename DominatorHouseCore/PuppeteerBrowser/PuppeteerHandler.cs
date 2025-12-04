using PuppeteerSharp;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public class PuppeteerHandler:IPuppeteerPage
    {
        public IPage PuppeteerPage { get; set; }
        public PuppeteerHandler(IPage page)
        {
            PuppeteerPage = page;
        }
    }
}
