using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Boundary boundary;

    private Rigidbody2D rb;

    public float Damage { get; protected set; }
  
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //Do not destroy the gameobject because Destroy() is also heavy, rather use the object pool to disable the bullet
        //Destroy(this.gameObject, 3.0f);
        boundary = new Boundary();
        boundary.CalculateScreenRestrictions();
    }

    protected void OnEnable()
    {
        //We use the up vector since the object has been instantiated based on the player's positon and rotation
        rb.velocity = transform.up * speed;
    }

    protected void Update()
    {
        CheckIfOutOfBounds();
    }

    protected void CheckIfOutOfBounds()
    {
        //Check if our position is greater than the max boundaries or less than the min boundaries
        if ((transform.position.x > boundary.Bounds.x || transform.position.x < -boundary.Bounds.x) ||
                (transform.position.y > boundary.Bounds.y || transform.position.y < -boundary.Bounds.y))
        {
            //Deactivate the gameobject
            this.gameObject.SetActive(false);
        }
    }
    public void InitializeValues(float damage)
    {
        Damage = damage;
    }

    public virtual void DestroyBullet()
    {
        ObjectPoolManager.Instance.ReturnObjectToPool(this.gameObject);
    }
}
