using System;
using System.Collections.Generic;

namespace JMath
{
    public static class ListExtentioner
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rand = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
