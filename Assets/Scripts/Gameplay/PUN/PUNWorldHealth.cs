using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PUNWorldHealth : WorldHealth, IOnEventCallback
{
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //We are only interested in the health event
        if (eventCode == PUNHealthManager.HealthEventCode)
        {
            //get the data from the parameters passed by the RaiseEvent
            object[] data = (object[])photonEvent.CustomData;
            float health = (float)data[0];
            float maxHealth = (float)data[1];
            UpdateHealthBar(health, maxHealth);
        }
    }

    protected override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    protected override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
