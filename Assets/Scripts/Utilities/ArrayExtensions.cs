using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

namespace GlueGames.Utilities
{
    // These are useful for writing code which operates on arrays and lists alike.
    public interface IArrayLike<T>
    {
        T this[int i] { get; }
        int Length { get; }
    }

    public struct ArrayWrapper<T> : IArrayLike<T>
    {
        private T[] array;

        public T this[int i] => array[i];
        public int Length => array.Length;

        public ArrayWrapper(T[] array)
        {
            this.array = array;
        }
    }

    public struct ListWrapper<T> : IArrayLike<T>
    {
        private List<T> list;

        public T this[int i] => list[i];
        public int Length => list.Count;

        public ListWrapper(List<T> list)
        {
            this.list = list;
        }
    }

    public static class ArrayExtensions
    {
        public static T RandomElement<T>(this T[] array)
        {
            return array[array.RandomIndex()];
        }

        public static int RandomIndex<T>(this T[] array)
        {
            return Random.Range(0, array.Length);
        }

        public static T RandomElement<T>(this List<T> list)
        {
            return list[list.RandomIndex()];
        }

        public static int RandomIndex<T>(this List<T> list)
        {
            return Random.Range(0, list.Count);
        }

        // I can't think of a good name for this method. It returns the length and treats null as 0-length.
        public static int NullableLength<T>(this T[] array)
        {
            return array?.Length ?? 0;
        }

        // Interesting discovery about extension methods - it's perfectly fine to call an extension method on a null instance.
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array.NullableLength() == 0;
        }

        public static int NullableCount<T>(this List<T> list)
        {
            return list?.Count ?? 0;
        }

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list.NullableCount() == 0;
        }

        public static void OrderByInPlace<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            list.Sort((elem1, elem2) => keySelector(elem1).CompareTo(keySelector(elem2)));
        }

        public static void OrderByInPlace<TSource, TKey>(this TSource[] array, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            Array.Sort(array, (elem1, elem2) => keySelector(elem1).CompareTo(keySelector(elem2)));
        }

        public static Dictionary<TKey, int> IndexBy<TSource, TKey>(this TSource[] array, Func<TSource, TKey> indexSelector)
        {
            var indexIntoArray = new Dictionary<TKey, int>();
            for (int i = 0; i < array.Length; i++)
            {
                indexIntoArray.Add(indexSelector(array[i]), i);
            }
            return indexIntoArray;
        }
    }

}
