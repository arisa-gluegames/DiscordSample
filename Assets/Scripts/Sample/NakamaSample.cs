using GlueGames.Authentication;
using GlueGames.Nakama;
using GlueGames.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NakamaSample : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private TextMeshProUGUI _userText;
    private IAuthenticationBackend _nakamaAuth;
    private IAuthenticationFactory _nakamaFactory;

    private async void Start()
    {
        LogManager.CurrentLogLevel = LogLevel.All;
        _nakamaFactory = new NakamaAuthenticationFactory();
        _nakamaAuth = _nakamaFactory.CreateAuthenticationBackend(null);
        _debugText.text = "Nakama Starting...";
        NakamaManager.Instance.EvtSocketConnected.AddListener(OnSocketConnected);
        NakamaManager.Instance.Initialize();
        NakamaUserManager.Instance.OnAccountLoaded.AddListener(OnAccountLoaded);
        NakamaUserManager.Instance.Initialize();
        await _nakamaAuth.LoginAsync(AuthProviderType.Email, "", "arisa@glue.games", "123password");
    }

    private void OnSocketConnected()
    {
        _debugText.text = "Nakama Connected!";
    }

    private void OnAccountLoaded()
    {
        NakamaUserManager userManager = NakamaUserManager.Instance;
        _userText.text = $"DisplayName: {userManager.NakamaUserData.DisplayName} UserName: {userManager.NakamaUserData.Username} ID: {userManager.NakamaUserData.UserId}";
        _debugText.text = "Joining Match...";
        MultiplayerManager.Instance.EvtMatchJoined.AddListener(OnMatchJoined);
        MultiplayerManager.Instance.JoinMatchAsync();
    }

    private void OnMatchJoined()
    {
        _debugText.text = "Match joined!";
        MultiplayerManager.Instance.EvtMatchJoined.RemoveListener(OnMatchJoined);
        SceneManager.LoadScene(3);
    }
}
