using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : SingletonPun<LobbyManager>
{
    [SerializeField]
    private Text uiLogger;
    [SerializeField]
    private string gameVersion = "1";
    [SerializeField]
    private string sceneToLoad = "Game";
    public byte MaxPlayersPerRoom { get; private set; }

    [SerializeField]
    private GameObject lobbyPanel, roomPanel;

    private void Start()
    {
        MaxPlayersPerRoom = 4;
        Connect();
        ShowPanelGroup("lobbyPanel");
    }

    public void ShowPanelGroup(string panelName)
    {
        lobbyPanel.SetActive(panelName.Equals(lobbyPanel.name));
        roomPanel.SetActive(panelName.Equals(roomPanel.name));
    }

    #region MonobehaviourPunCallbacks

    public override void OnConnectedToMaster()
    {
        Log("<color=Green>Connected to Master!</color>");
        Debug.Log("Connected to Master");

        //Join the lobby
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected because " + cause);
    }


    #endregion

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void StartGame()
    {
        Debug.Log(photonView);
        photonView.RPC("RPCLoadLevel", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void RPCLoadLevel()
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public void Log(string message)
    {
        //Dont do anything if we dont any reference to the ui logger
        if (uiLogger == null)
            return;

        uiLogger.text += System.Environment.NewLine + message;
    }

}
