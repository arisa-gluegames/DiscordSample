using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PUNPlayerStatusUI : PlayerStatusUI
{
    private Photon.Realtime.Player player;

    protected void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += UpdateUI;
    }

    protected void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdateUI;
    }

    protected override void UpdateUI()
    {
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
