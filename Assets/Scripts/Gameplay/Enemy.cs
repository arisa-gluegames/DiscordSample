using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected float maxHealth = 100;
    [SerializeField]
    protected float moveSpeed = 2.0f;
    [SerializeField]
    protected int scorePoints = 100;
    [SerializeField]
    protected float damage = 100;

    [SerializeField]
    protected Image healthbar;

    protected float currentHealth;
    protected Transform target;
    protected bool isDestroyed;

    public float Damage => damage;

    public override void OnEnable()
    {
        base.OnEnable();
        ResetValues();
    }

    protected void ResetValues()
    {
        isDestroyed = false;
        LookAtTarget();
        currentHealth = maxHealth;
        healthbar.fillAmount = 1.0f;
    }

    protected void UpdateHealthbar()
    {
        healthbar.fillAmount = currentHealth / maxHealth;
    }

    protected void LookAtTarget()
    {
        Quaternion newRotation;
        //Make the enemy face the target
        //If we do not have a specific target, make the object look at the center
        if (target == null)
        {
            newRotation = Quaternion.LookRotation(transform.position, Vector3.forward);
        }
        else
        {
            newRotation = Quaternion.LookRotation(transform.position - target.transform.position, Vector3.forward);
        }

        //Since rotation is only based on the z-axis
        newRotation.x = 0;
        newRotation.y = 0;
        transform.rotation = newRotation;
    }

    protected void Move()
    {
        //Since the object has been rotated, just make it move 
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    protected void Update()
    {
        if (isDestroyed)
        {
            return;
        }
        Move();
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.DestroyBullet();
            TakeDamage(bullet.Damage, null);
        }
    }

    public virtual void TakeDamage(float damage, Player from)
    {
        if (isDestroyed)
        {
            return;
        }
        currentHealth -= damage;
        UpdateHealthbar();

        if (currentHealth <= 0)
        {
            ScoreManager.Instance.AddScore(scorePoints);
            DestroyEnemy();
        }
    }

    public virtual void DestroyEnemy()
    {
        ObjectPoolManager.Instance.ReturnObjectToPool(this.gameObject);
        isDestroyed = true;
    }
}
