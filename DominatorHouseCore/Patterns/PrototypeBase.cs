#region

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace DominatorHouseCore.Patterns
{
    [Serializable]
    public static class PrototypeBase
    {
        //Deep Clone
        public static T DeepClone<T>(this T obj) where T : class
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    var copy = (T) formatter.Deserialize(stream);
                    stream.Close();
                    return copy;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }
    }
}