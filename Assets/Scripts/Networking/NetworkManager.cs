using Photon.Pun;
using UnityEngine;

public class NetworkManager : SingletonPun<NetworkManager>
{
    public bool IsMultiplayer = true;

    private const string playerPefabName = "Player";

    [SerializeField]
    private Sprite[] playerSprites;

    private void Start()
    {
        if (!IsMultiplayer) return;
        //Check if we are connected to the Photon Network
        if (!PhotonNetwork.IsConnected)
        {
            //Load the Title screen instead
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        //Instantiate a player gameObject to represent our local client
        PhotonNetwork.Instantiate(playerPefabName, Vector3.zero, Quaternion.identity);
    }

    public Sprite GetPlayerSprite(int playerNumber)
    {
        return playerSprites[playerNumber];
    }
}
