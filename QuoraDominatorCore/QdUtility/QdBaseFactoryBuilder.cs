using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.QdUtility
{
    public class QdBaseFactoryBuilder
    {
        public QdBaseFactoryBuilder(IQdUtilityFactory qdUtilityFactory)
        {
            QdUtilityFactory = qdUtilityFactory;
        }

        public IQdUtilityFactory QdUtilityFactory { get; set; }

        public QdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            QdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public QdBaseFactoryBuilder AddReportFactory(IQdReportFactory reportFactory)
        {
            QdUtilityFactory.QdReportFactory = reportFactory;
            return this;
        }

        public QdBaseFactoryBuilder AddViewCampaignFactory(IViewCampaignsFactory qdViewCampaign)
        {
            QdUtilityFactory.QdViewCampaign = qdViewCampaign;
            return this;
        }
    }
}