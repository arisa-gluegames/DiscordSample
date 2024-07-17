using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField]
    protected int playerNumber;
    [SerializeField]
    protected TextMeshProUGUI playerName;
    [SerializeField]
    protected TextMeshProUGUI playerScore;
    [SerializeField]
    protected Image playerIcon;
    [SerializeField]
    protected GameObject waiting, connected;

    private void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        playerScore.text = ScoreManager.Instance.Score.ToString("0000");
    }

    protected virtual void UpdateUI()
    {
        waiting.SetActive(false);
        connected.SetActive(true);
        //Update the info
        playerName.text = "Player";
    }
}
