using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected float maxSpeed = 2.0f;
    [SerializeField]
    protected float radius = 1.0f;
    [SerializeField]
    protected float fireRate = 0.1f;
    [SerializeField]
    protected float baseDamage = 20f;

    [SerializeField]
    //What is the ID of the pooled object that we want as a bullet
    protected string bulletId;
    //private GameObject bullet;

    protected float currentSpeed;
    protected float timeCounter;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        //Repeat calling the function "ShootBullet" every fireRate seconds
        //after the initial delay of 0.001f seconds
        InvokeRepeating("Shoot", 0.001f, fireRate);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleMovement();
    }

    protected void HandleMovement()
    {
        //Get the player input to determine the direction of the movement
        float movementInput = Input.GetAxis("Horizontal");
        currentSpeed = movementInput * Time.deltaTime * maxSpeed;
        timeCounter += currentSpeed;

        //circular motion based on the movement radius
        float x = Mathf.Cos(timeCounter) * radius;
        float y = Mathf.Sin(timeCounter) * radius;

        transform.position = new Vector2(x, y);

        HandleRotation();
    }

    protected void HandleRotation()
    {
        //Define a quaternion that will make the player face outwards of the circle
        Quaternion newRotation = Quaternion.LookRotation(-transform.position, Vector3.forward);
        //Disregard the x and y rotation since we are working on 2D
        newRotation.x = 0;
        newRotation.y = 0;
        transform.rotation = newRotation;
    }

    protected virtual void Shoot()
    {
        ShootBullet();
    }

    protected virtual void ShootBullet()
    {
        GameObject pooledBullet = ObjectPoolManager.Instance.GetPooledObject(bulletId);
        if (pooledBullet != null)
        {
            //Modify the bullet's position and rotation
            pooledBullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            pooledBullet.GetComponent<Bullet>().InitializeValues(baseDamage);
            //Enable the gameObject
            pooledBullet.SetActive(true);
        }
    }
}
