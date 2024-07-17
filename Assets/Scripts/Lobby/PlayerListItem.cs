using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class PlayerListItem : MonoBehaviour
{
    [SerializeField]
    private Text information;

    public Player player;

    public void SetPlayerInfo(Player p)
    {
        information.text = p.NickName;
        player = p;
    }
}
