using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HealthManager : SingletonPun<HealthManager>
{
    [SerializeField]
    protected float maxHealth = 1000;
    protected float health;


    public delegate void HealthListener(float health, float maxHealth);
    public static event HealthListener OnHealthUpdate;

    public virtual float Health
    {
        get { return health; }
        set
        {
            health = value;
            OnHealthUpdate?.Invoke(health, maxHealth);
        }
    }

    protected void Start()
    {
        Health = maxHealth;
    }
}
