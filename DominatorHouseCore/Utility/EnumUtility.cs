#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class EnumUtility
    {
        public static List<ActivityType> GetEnums(string requiredDescriptionData)
        {
            // gets the Type that contains all the info required    
            // to manipulate this type    
            var enumType = typeof(ActivityType);

            var enumValues = Enum.GetValues(typeof(ActivityType));

            return (
                    from ActivityType value in enumValues
                    let memberInfo = enumType.GetMember(value.ToString()).First()
                    let descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>()
                    where descriptionAttribute != null &&
                          descriptionAttribute.Description.Contains(requiredDescriptionData)
                    select value)
                .ToList();
        }

        // how to use
        // MyEnum x = MyEnum.NeedMoreCoffee;
        // string description = x.GetDescriptionAttr();            
        public static string GetDescriptionAttr(this Enum value)
        {
            try
            {
                var type = value.GetType();
                var name = Enum.GetName(type, value);
                if (name != null)
                {
                    var field = type.GetField(name);
                    if (field != null)
                    {
                        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) return attr.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public static string GetDescriptionAttribute(this string value, Type type)
        {
            try
            {
                if (value != null)
                {
                    var field = type.GetField(value);
                    if (field != null)
                    {
                        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) return attr.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public static string GetQueryFromEnum(Enum queryType)
        {
            return Application.Current?.FindResource(queryType.GetDescriptionAttr())?.ToString();
        }
    }
}