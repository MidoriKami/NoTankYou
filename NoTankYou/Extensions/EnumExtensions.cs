using System;
using System.ComponentModel;
using Dalamud.Utility;
using Microsoft.VisualBasic;

namespace NoTankYou.Extensions;

public static class EnumExtensions {
    extension(Enum enumValue) {
        public string Description => enumValue.GetDescription();

        private string GetDescription() {
            var attribute = enumValue.GetAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }
    }
    
    public static T ParseAsEnum<T>(this string stringValue, T defaultValue) where T : Enum {
        foreach (Enum enumValue in Enum.GetValues(typeof(T))) {
            if (enumValue.Description == stringValue) {
                return (T)enumValue;
            }
        }

        return defaultValue;
    }
}
