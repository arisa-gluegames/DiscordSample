using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public float Score { get; private set; }

    public void AddScore(float value)
    {
        Score += value;
    }
}
