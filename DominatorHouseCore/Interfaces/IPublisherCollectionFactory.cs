namespace DominatorHouseCore.Interfaces
{
    public interface IPublisherCollectionFactory
    {
        /// <summary>
        ///     To holds the objects of publisher core factory
        /// </summary>
        /// <returns></returns>
        IPublisherCoreFactory GetPublisherCoreFactory();
    }
}