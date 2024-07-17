using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeaveRoom : MonoBehaviour
{
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
}
