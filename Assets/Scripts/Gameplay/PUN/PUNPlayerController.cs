using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PUNPlayerController : PlayerController
{
    public override void OnEnable()
    {
        base.OnEnable();
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayerSprite;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayerSprite;
    }

    protected override void Update()
    {
        //Control only the player which we own
        if (!photonView.IsMine)
            return;

        base.Update();
    }

    protected override void Shoot()
    {
        //Ensure that the RPC call will be handled only by the local player
        if (!photonView.IsMine)
        {
            return;
        }
        photonView.RPC("RPCShootBullet", RpcTarget.AllViaServer);
    }

    protected override void ShootBullet()
    {
        GameObject pooledBullet = ObjectPoolManager.Instance.GetPooledObject(bulletId);
        if (pooledBullet != null)
        {
            //Modify the bullet's position and rotation
            pooledBullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            pooledBullet.GetComponent<PUNBullet>().InitializeValues(baseDamage, photonView.Owner);
            //Enable the gameObject
            pooledBullet.SetActive(true);
        }
    }

    [PunRPC]
    private void RPCShootBullet()
    {
        ShootBullet();
    }

    [PunRPC] // mark this function as an RPC or Remote Procedure Call
    // As a good practice to differentiate RPC methods to normal methods, add RPC at the beginning
    private void RPCAssignPlayerSprite()
    {
        //Change the sprite to match the index of the sprite array
        int playerNumber = photonView.Owner.GetPlayerNumber();
        spriteRenderer.sprite = NetworkManager.Instance.GetPlayerSprite(playerNumber);
    }

    private void UpdatePlayerSprite()
    {
        photonView.RPC("RPCAssignPlayerSprite", RpcTarget.AllBufferedViaServer);
    }
}
