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
            //if (index < MinTypeIndex || index > MaxTypeIndex)
            //{
            //    throw new ArgumentOutOfRangeException($"index must be between {MinTypeIndex} and {MaxTypeIndex}");
            //}

            var result = (LevelType)index;
            return result;
        }

        public static string GetTypeName(int index)
        {
            return GetType(index).ToString();
        }

        public static string GetLevelName(LevelType level)
        {
            return GetTypeName((int)level);
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
                    return "未知";
                case TypeType.Clean:
                    return "无码";
                case TypeType.CleanMask:
                    return "无码遮脸";
                case TypeType.Mosaic:
                    return "有码";
                default:
                    return "未知";
            }
        }
    }
}
