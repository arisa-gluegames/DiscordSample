using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PUNPlayerStatusUI : PlayerStatusUI
{
    private Photon.Realtime.Player player;

    private void Start()
    {
        if (PhotonNetwork.PlayerList.Length - 1 >= playerNumber)
        {
            player = PhotonNetwork.PlayerList[playerNumber];
        }  
    }

    protected void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += UpdateUI;
    }

    protected void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdateUI;
    }

    protected override void Update()
    {
        if (player == null) return;
        playerScore.text = player.GetScore().ToString();
    }

    protected override void UpdateUI()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);
        //Check if there is an available player
        if (PhotonNetwork.PlayerList.Length - 1 >= playerNumber)
        {
            waiting.SetActive(false);
            connected.SetActive(true);
            //Update the info
            player = PhotonNetwork.PlayerList[playerNumber];
            playerName.text = player.NickName;
            playerIcon.sprite = NetworkManager.Instance.GetPlayerSprite(playerNumber);
            playerScore.text = player.GetScore().ToString();
        }
        else
        {
            waiting.SetActive(true);
            connected.SetActive(false);
        }
    }
}
