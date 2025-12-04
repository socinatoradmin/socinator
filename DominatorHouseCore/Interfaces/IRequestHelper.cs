namespace DominatorHouseCore.Interfaces
{
    public interface IRequestHelper
    {
        void SetRequestParameter(IRequestParameters requestParameters);
        IRequestParameters GetRequestParameter();
    }
}