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
                if (pointer.Type == value.Type && pointer.IsNumber == value.IsNumber && pointer.Index == value.Index)
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

        public static T[] Clone<T>(this T[] array) where T : ICloneable
        {
            T[] newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    newArray[i] = array[i];
                }
                else
                {
                    newArray[i] = (T)array[i].Clone();
                }
            }
            return newArray;
        }

        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            List<T> newList = new List<T>(list.Capacity);
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add((T)list[i].Clone());
            }
            return newList;
        }

        public static List<long> Clone(this List<long> list)
        {
            List<long> newList = new List<long>(list.Capacity);
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(list[i]);
            }
            return newList;
        }

        public static List<bool> Clone(this List<bool> list)
        {
            List<bool> newList = new List<bool>(list.Capacity);
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(list[i]);
            }
            return newList;
        }
    }
}
