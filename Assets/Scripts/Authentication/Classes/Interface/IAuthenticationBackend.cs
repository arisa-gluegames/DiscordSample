using System.Threading.Tasks;
using UnityEngine.Events;

namespace GlueGames.Authentication
{
    public interface IAuthenticationBackend
    {
        public UnityEvent EvtInitialized { get; }
        public UnityEvent EvtSignInSuccess { get; }
        public UnityEvent EvtSignInFailed { get; }
        public UnityEvent EvtSignOutInitiated { get; }
        public UnityEvent EvtSignOutSuccess { get; }
        public UnityEvent EvtSignOutFailed { get; }
        public UnityEvent EvtUserDeletedSuccess { get; }
        public UnityEvent<string> EvtUserDeletedFailed { get; }
        public UnityEvent EvtLinkUserSuccess { get; }
        public UnityEvent<string> EvtLinkUserFailed { get; }
        public UnityEvent EvtSwitchAccountSuccess { get; }
        public UnityEvent EvtSwitchAccountFailed { get; }
        public void Initialize();
        public IUserData UserData { get; }
        public bool IsUserFetched { get; }
        public bool HasLoginHistory { get; }
        public AuthProviderType CurrentAuthProviderType { get; }
        public void Login(AuthProviderType type);
        public Task LoginAsync(AuthProviderType type, string token = "", string email = "", string password = "");
        public Task<bool> RefreshLogin();
        public void LinkAccount(AuthProviderType type);
        public void UnlinkAccount(AuthProviderType type);
        public void SwitchAccount(AuthProviderType type);
        public void SignOut();
        public void DeleteUser();
    }
}

