using System;
using UnityEngine;

[Serializable]
public class MinMaxPair<T>
{
    [SerializeField]
    private T _min;
    [SerializeField]
    private T _max;

    public T Min => _min;
    public T Max => _max;

    public MinMaxPair(T min, T max)
    {
        _min = min;
        _max = max;
    }
}