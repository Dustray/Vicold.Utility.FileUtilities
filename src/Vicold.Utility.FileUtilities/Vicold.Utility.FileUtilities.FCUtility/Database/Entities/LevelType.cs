using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Database.Entities
{
    public enum LevelType
    {
        Unset = 0,
        SSR = 1,
        SS = 2,
        S = 3,
        A = 4,
        B = 5,
        C = 6,
        D = 7,
    }

    public static class LevelTypeInfo
    {
        public static int MinTypeIndex = 1;
        public static int MaxTypeIndex = 7;

        public static LevelType GetType(int index)
        {
            if (index < MinTypeIndex || index > MaxTypeIndex)
            {
                throw new ArgumentOutOfRangeException($"index must be between {MinTypeIndex} and {MaxTypeIndex}");
            }

            var result = (LevelType)index;
            return result;
        }

        public static string GetTypeString(int index)
        {
            return GetType(index).ToString();
        }
    }

    public enum TypeType
    {
        Unknown = 0,
        Clean = 1,
        CleanMask = 2,
        Mosaic = 3,
    }

    public static class TypeTypeInfo
    {
        public static string GetTypeName(TypeType type)
        {
            switch (type)
            {
                case TypeType.Unknown:
                    return "Unknown";
                case TypeType.Clean:
                    return "Clean";
                case TypeType.CleanMask:
                    return "CleanMask";
                case TypeType.Mosaic:
                    return "Mosaic";
                default:
                    return "Unknown";
            }
        }
    }
}
