#region

using System.Text;

#endregion

namespace DominatorHouseCore.Utility
{
    public class CacheText
    {
        private readonly StringBuilder _sb;

        public CacheText()
        {
            _sb = new StringBuilder();
        }

        private int Counter { get; set; }

        public int Limit { get; set; } = 500;

        public bool AddToCache(string text)
        {
            if (Counter >= Limit) return false;
            _sb.Append(text);
            Counter++;
            return true;

        }

        public string GetCacheText()
        {
            var tmpSbText = _sb.ToString();
            _sb.Clear();
            Counter = 0;
            return tmpSbText;
        }
    }
}