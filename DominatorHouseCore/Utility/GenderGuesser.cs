#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class GenderGuesser
    {
        private static readonly HashSet<string> FemaleNames = new HashSet<string>(
            DominatorHouseCore.Resources.femaleNames.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries));

        private static readonly HashSet<string> MaleNames = new HashSet<string>(
            DominatorHouseCore.Resources.maleNames.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries));

        private static readonly HashSet<string> UnisexNames = new HashSet<string>(
            DominatorHouseCore.Resources.unisexNames.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries));

        public static Gender GetGender(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can't be empty");
            name = name.ToLower();
            if (MaleNames.Contains(name))
                return Gender.Male;
            if (FemaleNames.Contains(name))
                return Gender.Female;
            return !UnisexNames.Contains(name) ? Gender.Unknown : Gender.Unisex;
        }
    }
}