using System.IO;
using System.Reflection;

namespace Dominator.Tests.Utils
{
    public static class TestUtils
    {
        public static string ReadFileFromResources(string resourceName, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static byte[] ReadFileFromResourcesAsBytes(string resourceName, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
