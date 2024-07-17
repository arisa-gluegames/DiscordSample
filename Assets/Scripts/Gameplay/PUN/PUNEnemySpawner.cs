using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PUNEnemySpawner : EnemySpawner
{
    protected override void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CalculateScreenRestrictions();
    }

    protected override void Update()
    {
        //Don't do anything if you are not the master client
        if (!PhotonNetwork.IsMasterClient) return;

        base.Update();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            CalculateScreenRestrictions();
        }
    }

    protected override void SpawnEnemy()
    {
        SpawnOverNetwork();
    }

    private void SpawnOverNetwork()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(enemyId, GetSpawnPosition(), Quaternion.identity);
        }
    }
}
