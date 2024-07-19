using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Networking;
using static Dissonity.Api;
using GlueGames.Utilities;
using Dissonity;
using Discord;

public class LobbyManager : SingletonPun<LobbyManager>
{
    [SerializeField]
    private Text uiLogger;
    [SerializeField]
    private string gameVersion = "1";
    [SerializeField]
    private string sceneToLoad = "Game";
    [SerializeField]
    private TMP_InputField _userName;
    [SerializeField]
    private Image _userAvatar;

    [SerializeField]
    private PlayerStatusUI _lobbyPlayerStatus;

    public byte MaxPlayersPerRoom { get; private set; }

    [SerializeField]
    private GameObject lobbyPanel, roomPanel;

    private int _lastParticipantCount;

    private async void Start()
    {
        _lastParticipantCount = (await GetInstanceParticipants()).participants.Length;
        Log($"[Discord] There are: {_lastParticipantCount} users");
        // Discord may send an event multiple times, so if you're
        // just trying to detect when a user joins or leaves, don't
        // do anything when (lastParticipantCount == data.participants.Length)
        SubActivityInstanceParticipantsUpdate((data) => {
            //? Someone left
            if (data.participants.Length < _lastParticipantCount)
            {
                _lastParticipantCount = data.participants.Length;
                DissonityLog("Received a user leave!");
            }
            //? Some joined
            else if (data.participants.Length > _lastParticipantCount)
            {
                _lastParticipantCount = data.participants.Length;
                DissonityLog("Received a new user!");
            }
            Log($"[Discord] Participants Update: {_lastParticipantCount}");
        });

        SubCurrentUserUpdate((user) =>
        {
            if (user == null) return;
            Log($"[Discord] Recieved user: {user.display_name} {user.avatar}");
            _userName.text = user.display_name;
            if(user.avatar != null)
            {
                string iconUrl = $"https://cdn.discordapp.com/avatars/{user.id}/{user.avatar}.png?size=256";
                StartCoroutine(LoadImage(iconUrl));
            }
        });
        Log("Discord started");
        MaxPlayersPerRoom = 4;
        Connect();
        ShowPanelGroup("lobbyPanel");
    }

    private void UpdateLobbyPlayers(Participant[] participants)
    {
        Log($"[Discord] Participants:");
        for (int i = 0; i < participants.Length; i++)
        {
            Log($"[{i}]{participants[i].display_name} {participants[i].avatar}:<br>");
        }
        Log($"----");
    }

    private IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();  
        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Log(request.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(myTexture, new UnityEngine.Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
            _userAvatar.sprite = sprite;
        }
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
        Log("Trying to connect to Photon");
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
