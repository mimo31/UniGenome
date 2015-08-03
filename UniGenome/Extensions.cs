using System;
using System.Collections.Generic;

namespace UniGenome
{
    static class Extensions
    {
        public static bool ContainsValue(this List<NodePointer> list, NodePointer value)
        {
            foreach (NodePointer pointer in list)
            {
                if (pointer.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static long NextLong(this Random r)
        {
            byte[] buffer = new byte[sizeof(long)];
            r.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static T[] Clone<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = (T)array[i];
            }
            return newArray;
        }

        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> newList = new List<T>(list.Capacity);
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(list[i]);
            }
            return newList;
        }
    }
}
