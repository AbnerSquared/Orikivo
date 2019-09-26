using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class ListExtensions
    {
        // shifts all of the elements of the list left by the specific amount.
        // 
        // items2 = items.ShiftLeft(1);
        // example: items2[0] == items[1]; 
        // referred from stackoverflow
        // https://stackoverflow.com/questions/18180958/does-code-exist-for-shifting-list-elements-to-left-or-right-by-specified-amount
        public static List<T> ShiftLeft<T>(this List<T> list, int count, bool wrap = false) // if the elements wrap around its current capacity.
        {
            T[] arr = list.ToArray();
            Array.Copy(arr, count, arr, 0, arr.Length - count);
            Array.Clear(arr, arr.Length - count, count);
            return arr.ToList();
            // add a wrapping ability
        }
    }
}
