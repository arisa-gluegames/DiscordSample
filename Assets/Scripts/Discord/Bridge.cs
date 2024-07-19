using UnityEngine;
using UnityEngine.Events;

public class UserData
{
    public string userName;
    public string iconUrl;
}
public class Bridge : MonoBehaviour
{
    public UnityEvent<string> OnUserDataChanged { get; } = new();
    public string UserData { get; private set; }

    public void SetUserData(string value)
    {
        UserData = value;
        OnUserDataChanged?.Invoke(UserData);
        LobbyManager.Instance.Log($"Discord: {UserData}");
    }
}
