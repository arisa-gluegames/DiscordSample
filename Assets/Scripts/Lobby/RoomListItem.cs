using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListItem : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text information;
    public RoomInfo roomInfo;


    //Initialize the room information
    public void SetRoomInfo(RoomInfo info)
    {
        information.text = string.Format("{0} {1}/{2}", info.Name, info.PlayerCount, info.MaxPlayers);
        roomInfo = info;
    }

    //Update the room information when the player joins the room
    public override void OnJoinedRoom()
    {
        information.text = string.Format("{0} {1}/{2}", roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers);
        LobbyManager.Instance.Log("Joined " + roomInfo.Name);
        LobbyManager.Instance.ShowPanelGroup("roomPanel");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LobbyManager.Instance.Log("Failed to join because " + message);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }
}
