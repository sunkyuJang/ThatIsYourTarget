using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace JExtentioner
{
    public static class EnumExtentioner
    {
        public static T? GetEnumVal<T>(this int index) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), index))
            {
                return (T)Enum.ToObject(typeof(T), index);
            }

            return null;
        }

        public static int GetEnumSize<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}
