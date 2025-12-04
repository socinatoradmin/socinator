using PinDominatorCore.PDEnums;

namespace PinDominatorCore.PDModel
{
    public class ScrapeResult
    {
        public ScrapeResult(PinterestAction action, ScrapingParameters? filterType, string filterArgument)
        {
            Action = action;
            FilterType = filterType;
            FilterArgument = filterArgument;
        }

        public PinterestAction Action { get; }

        public string FilterArgument { get; }

        public ScrapingParameters? FilterType { get; }
    }
}