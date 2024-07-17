using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlueGames.Utilities
{
    public interface IWeightedObject<T>
    {
        int Weight { get; }
        T Value { get; }
    }

    [Serializable]
    public class WeightedObject<T> : IWeightedObject<T>
    {
        [SerializeField] private int weight;
        [SerializeField] private T value;

        public int Weight => weight;
        public T Value => value;

        public WeightedObject(int weight, T value)
        {
            this.weight = weight;
            this.value = value;
        }

        public WeightedObject(WeightedObject<T> other) : this(other.weight, other.value)
        {
        }

        public static T GetRandomValue<TWeighted>(TWeighted[] pairs, Func<int, T, bool> Predicate = null) where TWeighted : IWeightedObject<T>
        {
            return GetRandomValue<ArrayWrapper<TWeighted>, TWeighted>(new ArrayWrapper<TWeighted>(pairs), Predicate);
        }

        /// <summary>
        /// Randomly chooses an item based on the weights, while ignoring items for which the predicate returns false.
        /// </summary>
        /// <param name="Predicate">A predicate of an item's index inside pairs, and the item itself.</param>
        /// <returns>The index of the chosen item inside pairs or -1 if no item fulfilled the predicate.</returns>
        public static int GetRandomIndex<TWeighted>(TWeighted[] pairs, Func<int, T, bool> Predicate = null) where TWeighted : IWeightedObject<T>
        {
            return GetRandomIndex<ArrayWrapper<TWeighted>, TWeighted>(new ArrayWrapper<TWeighted>(pairs), Predicate);
        }

        public static T GetRandomValue<TWeighted>(List<TWeighted> pairs, Func<int, T, bool> Predicate = null) where TWeighted : IWeightedObject<T>
        {
            return GetRandomValue<ListWrapper<TWeighted>, TWeighted>(new ListWrapper<TWeighted>(pairs), Predicate);
        }

        /// <summary>
        /// Randomly chooses an item based on the weights, while ignoring items for which the predicate returns false.
        /// </summary>
        /// <param name="Predicate">A predicate of an item's index inside pairs, and the item itself.</param>
        /// <returns>The index of the chosen item inside pairs or -1 if no item fulfilled the predicate.</returns>
        public static int GetRandomIndex<TWeighted>(List<TWeighted> pairs, Func<int, T, bool> Predicate = null) where TWeighted : IWeightedObject<T>
        {
            return GetRandomIndex<ListWrapper<TWeighted>, TWeighted>(new ListWrapper<TWeighted>(pairs), Predicate);
        }

        private static T GetRandomValue<TArr, TWeighted>(TArr pairs, Func<int, T, bool> Predicate)
            where TArr : IArrayLike<TWeighted>
            where TWeighted : IWeightedObject<T>
        {
            int index = GetRandomIndex<TArr, TWeighted>(pairs, Predicate);
            return index == -1 ? default(T) : pairs[index].Value;
        }

        private static int GetRandomIndex<TArr, TWeighted>(TArr pairs, Func<int, T, bool> Predicate)
            where TArr : IArrayLike<TWeighted>
            where TWeighted : IWeightedObject<T>
        {
            int totalWeight = 0;

            for (int i = 0; i < pairs.Length; i++)
            {
                IWeightedObject<T> pair = pairs[i];

                if (!(Predicate?.Invoke(i, pair.Value) ?? true))
                {
                    continue;
                }

                totalWeight += pair.Weight;
            }

            int randomVal = UnityEngine.Random.Range(0, totalWeight);
            int cumulativeWeight = 0;
            int returnVal = -1;

            for (int i = 0; i < pairs.Length; i++)
            {
                IWeightedObject<T> pair = pairs[i];

                if (!(Predicate?.Invoke(i, pair.Value) ?? true))
                {
                    continue;
                }

                cumulativeWeight += pair.Weight;

                if (randomVal < cumulativeWeight)
                {
                    returnVal = i;
                    break;
                }
            }

            return returnVal;
        }
    }
}
