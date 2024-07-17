using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class CreateRoom : MonoBehaviourPunCallbacks
{
    private Button button;

    // Start is called before the first frame update
    private void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;
    }

    public void Create()
    {
        //Check if connected to master before creating a room
        if (!PhotonNetwork.IsConnected)
            return;

        //Create a Room
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = LobbyManager.Instance.MaxPlayersPerRoom;
        options.IsVisible = true;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "'s Room", options);
    }

    public override void OnCreatedRoom()
    {
        LobbyManager.Instance.Log("Successfully created a room");
        LobbyManager.Instance.ShowPanelGroup("roomPanel");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LobbyManager.Instance.Log("Failed to create a room");
    }


}
