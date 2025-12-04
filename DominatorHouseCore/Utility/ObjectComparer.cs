#region

using ObjectsComparer;

#endregion

namespace DominatorHouseCore.Utility
{
    public class ObjectComparer
    {
        /// <summary>
        ///     it will take two object of same type and compare it
        ///     if both objects are equal then it will return null
        ///     otherwise return changed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static T CompareAndGetChangedObject<T>(T oldModel, T newModel) where T : class
        {
            return Compare(oldModel, newModel) ? null : newModel;
        }

        /// <summary>
        ///     This method will compare two objects
        ///     if both objects all properties values are equals then it will return true
        ///     otherwise it will return false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static bool Compare<T>(T oldModel, T newModel) where T : class
        {
            var objectCompare = new Comparer<T>();
            return objectCompare.Compare(oldModel, newModel);
        }
    }
}