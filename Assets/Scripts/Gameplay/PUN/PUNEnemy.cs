using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PUNEnemy : Enemy
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PUNBullet>(out PUNBullet bullet))
        {
            TakeDamage(bullet.Damage, bullet.Owner);
        }
    }

    public override void TakeDamage(float damage, Player from)
    {
        if (isDestroyed)
        {
            return;
        }
        currentHealth -= damage;
        UpdateHealthbar();

        if (currentHealth <= 0)
        {
            //Add score
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.ActorNumber == from.ActorNumber)
                    {
                        p.AddScore(scorePoints);
                    }
                }
            }
            DestroyEnemy();
        }
    }

    public override void DestroyEnemy()
    {
        NetworkDestroy();
    }

    public void NetworkDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
            DestroyGlobally();
        else
            DestroyLocally();
    }

    private void DestroyLocally()
    {
        isDestroyed = true;
    }

    private void DestroyGlobally()
    {
        PhotonNetwork.Destroy(this.gameObject);
        isDestroyed = true;
    }
}
