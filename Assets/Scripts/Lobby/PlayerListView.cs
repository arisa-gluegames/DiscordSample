using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
public class PlayerListView : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerListItem playerItemPrefab;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private Text roomTitle;

    private List<PlayerListItem> listings = new List<PlayerListItem>();

    //Update UI Information
    private void HandleUI()
    {
        roomTitle.text = PhotonNetwork.CurrentRoom.Name;
        startButton.SetActive(PhotonNetwork.CurrentRoom.MasterClientId == PhotonNetwork.LocalPlayer.ActorNumber);
    }

    //Create a playerItem in the list
    private void AddPlayerListing(Player p)
    {
        PlayerListItem item = Instantiate(playerItemPrefab, content);
        item.SetPlayerInfo(p);
        listings.Add(item);
    }

    //Update the listing based on the existing players in the room
    private void GetCurrentRoomPlayers()
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerListing(p);
        }
    }

    public override void OnJoinedRoom()
    {
        GetCurrentRoomPlayers();
        HandleUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnLeftRoom()
    {
        //Destroy all playerItems
        foreach (PlayerListItem item in listings)
            Destroy(item.gameObject);
        //Clear the list
        listings.Clear();
        //Go back to the lobby view
        LobbyManager.Instance.ShowPanelGroup("lobbyPanel");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        HandleUI();
        //Get the playerlistitem that has the same player reference as that of the player that has left the room
        int playerIndex = listings.FindIndex(p => p.player == otherPlayer);
        //if we found the player
        if(playerIndex != -1)
        {
            //Destroy the gameobject and remove it from the list
            Destroy(listings[playerIndex].gameObject);
            listings.RemoveAt(playerIndex);
        }
    }
}
