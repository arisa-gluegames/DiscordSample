using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomListView : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomListItem roomListPrefab;
    [SerializeField]
    private Transform content;

    private List<RoomListItem> listings = new List<RoomListItem>();

    //When the client joins the room, clear all the room listing
    public override void OnJoinedRoom()
    {
        foreach (RoomListItem item in listings)
            Destroy(item.gameObject);
        listings.Clear();
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            //Check if the room is removed from the list
            if (info.RemovedFromList)
            {
                //Find the room info that has the same name in the list
                int index = listings.FindIndex(r => r.roomInfo.Name == info.Name);
                //found a room
                if(index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            }
            //Otherwise, the room info is added/updated
            else
            {
                //Check if the room is already existing in the roomList
                //Get the index of the room that has the same name
                int roomIndex = listings.FindIndex(r => r.roomInfo.Name == info.Name);

                //We did not find a room with the same room name
                if (roomIndex == -1)
                {
                    //Add a roomlist prefab
                    RoomListItem item = Instantiate(roomListPrefab, content);
                    item.SetRoomInfo(info);
                    listings.Add(item);
                }
                //Otherwise, we found a room
                else
                {
                    //Update the room information
                    listings[roomIndex].SetRoomInfo(info);
                }

            }
        }
    }
}
