#region

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        //static EnumDescriptionConverter _instance;
        //static EnumDescriptionConverter Instance => _instance ?? (_instance = new EnumDescriptionConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Enum))
                return null;

            var enumValue = value as Enum;

            var discription = GetDescription(enumValue).FromResourceDictionary();


            return discription;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Enum.ToObject(targetType, value);
            }
            catch
            {
                return value;
            }
        }

        public static string GetDescription(Enum en)
        {
            var type = en.GetType();
            var memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0) return ((DescriptionAttribute) attrs[0]).Description;
            }

            return en.ToString();
        }
    }
}