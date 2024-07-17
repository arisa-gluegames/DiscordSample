using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PUNHealthManager : HealthManager
{
    public static byte HealthEventCode = 1;

    public override float Health
    {
        get { return health; }
        set
        {
            //Make sure only the MasterClient is changing the health
            if (!PhotonNetwork.IsMasterClient)
                return;

            //Invoke or Raise event over the network
            object[] healthData = new object[] { health, maxHealth };
            RaiseEventOptions options = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };
            PhotonNetwork.RaiseEvent(HealthEventCode, healthData, options, SendOptions.SendUnreliable);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        //Just refresh the call whenever a player enters the room
        Health = Health;
    }
}
