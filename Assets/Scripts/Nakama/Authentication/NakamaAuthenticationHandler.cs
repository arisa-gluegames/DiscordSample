using GlueGames.Authentication;
using GlueGames.Utilities;
using Nakama;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GlueGames.Nakama
{
    public class NakamaAuthenticationHandler : IMultiPlayerAuthenticationAuthenticationBackend
    {
        public AuthProviderType LogInMethod => _authenticationType;
        private AuthProviderType _authenticationType;
        private Task<ISession> _authenticationTask;
        private NakamaManager Manager => NakamaManager.Instance;
        private IClient Client => Manager.Client;
        private readonly ISSOAuthenticationManager _authenticationManager;
        private AuthProviderType _currentAuthProvider;

        public UnityEvent EvtInitialized { get; } = new();
        public UnityEvent EvtSignInSuccess { get; } = new();
        public UnityEvent EvtSignInFailed { get; } = new();
        public UnityEvent EvtSignOutInitiated { get; } = new();
        public UnityEvent EvtSignOutSuccess { get; } = new();
        public UnityEvent EvtSignOutFailed { get; } = new();
        public UnityEvent EvtLinkUserSuccess { get; } = new();
        public UnityEvent<string> EvtLinkUserFailed { get; } = new();
        public UnityEvent EvtSwitchAccountSuccess { get; } = new();
        public UnityEvent EvtSwitchAccountFailed { get; } = new();
        public UnityEvent EvtUserDeletedSuccess { get; } = new();
        public UnityEvent<string> EvtUserDeletedFailed { get; } = new();
        public IUserData UserData => NakamaUserManager.Instance.NakamaUserData;
        public AuthProviderType CurrentAuthProviderType => _currentAuthProvider;

        public bool IsUserFetched { get; private set; } = false;
        public bool HasLoginHistory { get; private set; } = false;
        public void Initialize()
        {
            EvtInitialized.Invoke();
        }

        public NakamaAuthenticationHandler(ISSOAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public void Login(AuthProviderType type)
        {
            var loginTask = LoginAsync(type);
            loginTask.Start();
            loginTask.Wait();
        }

        public async Task<bool> CustomLoginAsync(string customUserId)
        {
            Task<ISession> loginTask = Login(AuthProviderType.Custom, customUserId, "", "");
            await loginTask;
            var session = loginTask.Result;
            if (session != null)
            {
                LoginAsync(session);
            }

            return session != null;
        }

        public async Task LoginAsync(AuthProviderType type, string token = "", string email = "", string password = "")
        {
            Task<ISession> loginTask = Login(type, token, email, password);
            await loginTask;
            var session = loginTask.Result;
            if (session != null)
            {
                LoginAsync(session);
            }
        }

        private async void LoginAsync(ISession session)
        {
            try
            {
                Manager.SetSession(session);
                await Manager.Socket.ConnectAsync(session);
                await NakamaUserManager.Instance.LoadUserAccount();
                EvtSignInSuccess?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                EvtSignInFailed?.Invoke();
            }
        }

        public void LinkAccount(AuthProviderType type)
        {
            _currentAuthProvider = type;
            PlayerPrefs.SetString(Commons.LastAuthMethodKey, _currentAuthProvider.ToString());
            switch (type)
            {
                case AuthProviderType.Facebook:
                    LinkWithFacebook();
                    break;
                case AuthProviderType.Google:
                    LinkWithGoogle();
                    break;
                case AuthProviderType.Apple:
                    LinkWithApple();
                    break;
            }
        }


        public void UnlinkAccount(AuthProviderType type)
        {
            _currentAuthProvider = type;
            PlayerPrefs.SetString(Commons.LastAuthMethodKey, _currentAuthProvider.ToString());
            switch (type)
            {
                case AuthProviderType.Facebook:
                    UnlinkWithFacebook();
                    break;
                case AuthProviderType.Google:
                    UnlinkWithGoogle();
                    break;
                case AuthProviderType.Apple:
                    UnlinkWithApple();
                    break;
            }
        }

        public void SignOut()
        {
            EvtSignOutInitiated?.Invoke();
            SignOutSession();
        }

        private async void SignOutSession()
        {
            await Manager.Client.SessionLogoutAsync(Manager.Session);
            await Manager.CloseSocket();
            EvtSignOutSuccess?.Invoke();
        }

        public void DeleteUser()
        {
            DeleteSession();
        }

        private async void DeleteSession()
        {
            await Manager.Client.DeleteAccountAsync(Manager.Session);
            EvtUserDeletedSuccess?.Invoke();
        }

        public void SwitchAccount(AuthProviderType type)
        {

        }

        public void Disconnect()
        {
            PlayerPrefs.SetString(Commons.AuthenticationTokenKey, "");
            PlayerPrefs.SetString(Commons.RefreshTokenKey, "");
        }

        public async Task<bool> RefreshLogin()
        {
            Task<ISession> refreshLoginTask = RefreshLoginSession();
            await refreshLoginTask;
            Manager.SetSession(refreshLoginTask.Result);
            if (Manager.Session != null)
            {
                await Manager.Socket.ConnectAsync(Manager.Session);
                await NakamaUserManager.Instance.LoadUserAccount();
                EvtSignInSuccess?.Invoke();
                //Debug.Log("Login restored");
                return true;
            }
            //Debug.Log("No previous login info");
            EvtSignInFailed?.Invoke();
            return false;
        }

        private async Task<ISession> RefreshLoginSession()
        {
            bool authenticated = !string.IsNullOrEmpty(PlayerPrefs.GetString(Commons.NakamaAuthMethodKey, null));
            bool isAuthToken = !string.IsNullOrEmpty(PlayerPrefs.GetString(Commons.AuthenticationTokenKey, null));
            if (!authenticated || !isAuthToken)
                return null;

            var authMethod = (AuthProviderType)Enum.Parse(typeof(AuthProviderType), PlayerPrefs.GetString(Commons.NakamaAuthMethodKey));
            //Debug.Log("Refreshing authentication from last method: " + authMethod);
            return await ValidateLogin(authMethod);
        }

        private async Task<ISession> ValidateLogin(AuthProviderType authMethod)
        {
            string authToken = PlayerPrefs.GetString(Commons.AuthenticationTokenKey, null);
            string refreshToken = PlayerPrefs.GetString(Commons.RefreshTokenKey, null);
            bool isAuthToken = !string.IsNullOrEmpty(authToken);
            ISession session = null;
            // If there is an authToken, switch to refresh token
            if (isAuthToken)
            {
                Task<ISession> restoreTask = RestoreSession(authToken, refreshToken);
                session = await restoreTask;
            }
            return session;
        }

        public async Task<ISession> Login(AuthProviderType authType, string token, string email, string password, bool isQuickSignIn = false)
        {
            // Firebase is the default authentication method so while the rest are implemented, it is not actually used besides the Custom
            // where the Firebase id is being passed
            _authenticationType = authType;
            PlayerPrefs.SetString(Commons.NakamaAuthMethodKey, _authenticationType.ToString());
            switch (authType)
            {
                case AuthProviderType.Facebook:
                    return await ThirdPartyLoginAsync(AuthProviderType.Facebook, isQuickSignIn);
                case AuthProviderType.Apple:
                    return await ThirdPartyLoginAsync(AuthProviderType.Apple, isQuickSignIn);
                case AuthProviderType.Google:
                    return await ThirdPartyLoginAsync(AuthProviderType.Google, isQuickSignIn);
                case AuthProviderType.Email:
                    return await LoginWithEmail(email, password);
                case AuthProviderType.Device:
                    return await LoginWithDevice();
                // Only this case is relevant
                case AuthProviderType.Custom:
                    return await LoginCustom(token);
            }
            return await Authenticate();
        }

        private async Task<ISession> LoginCustom(string customUserId)
        {
            // Set username with custom user id as well
            _authenticationTask = Client.AuthenticateCustomAsync(customUserId, customUserId);
            _currentAuthProvider = AuthProviderType.Custom;
            return await Authenticate();
        }

        private async Task<ISession> Authenticate()
        {
            ISession session;
            try
            {
                session = await _authenticationTask;
                PlayerPrefs.SetString(Commons.AuthenticationTokenKey, session.AuthToken);
                PlayerPrefs.SetString(Commons.RefreshTokenKey, session.RefreshToken);
                PlayerPrefs.Save();
                return session;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Error authenticating: {exception}-{exception.Message}");
                return null;
            }
        }

        private async Task<ISession> RestoreSession(string authToken, string refreshToken)
        {
            ISession session = Session.Restore(authToken, refreshToken);
            // Check whether a session is close to expiry.
            if (session.HasExpired(DateTime.UtcNow.AddDays(1)))
            {
                try
                {
                    // get a new access token
                    session = await Client.SessionRefreshAsync(session);
                    PlayerPrefs.SetString(Commons.AuthenticationTokenKey, session.AuthToken);
                    PlayerPrefs.Save();
                }
                catch (ApiResponseException)
                {
                    Debug.LogWarning("Session expired.");
                    return null;
                }
            }
            return session;
        }

        private async Task<ISession> LoginWithDevice()
        {
            LogManager.LogWarningInfo("Authenticate With Device");
            _authenticationTask = Client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
            _currentAuthProvider = AuthProviderType.Device;
            return await Authenticate();
        }

        private async Task<ISession> LoginWithEmail(string email, string password)
        {
            _authenticationTask = Client.AuthenticateEmailAsync(email, password);
            _currentAuthProvider = AuthProviderType.Email;
            return await Authenticate();
        }

        private async Task<ISession> ThirdPartyLoginAsync(AuthProviderType method, bool isQuickSignIn)
        {
            Task<bool> loginTask = _authenticationManager.SignInAsync(method, isQuickSignIn);
            await loginTask;
            bool isLoggedIn = loginTask.Result;
            if (isLoggedIn)
            {
                string token = _authenticationManager.GetToken(method);
                switch (method)
                {
                    case AuthProviderType.Facebook:
                        _authenticationTask = Client.AuthenticateFacebookAsync(token);
                        _currentAuthProvider = AuthProviderType.Facebook;
                        break;
                    case AuthProviderType.Google:
                        _authenticationTask = Client.AuthenticateGoogleAsync(token);
                        _currentAuthProvider = AuthProviderType.Google;
                        break;
                    case AuthProviderType.Apple:
                        _authenticationTask = Client.AuthenticateAppleAsync(token);
                        _currentAuthProvider = AuthProviderType.Google;
                        break;
                }
                return await Authenticate();
            }
            else
            {
                return null;
            }
        }

        private void LinkWithFacebook()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInSuccess.AddListener(OnLinkedWithFacebook);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Facebook);
        }

        private void UnlinkWithFacebook()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInSuccess.AddListener(OnUnlinkWithFacebook);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Facebook);
        }

        private async void OnLinkedWithFacebook()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInSuccess.RemoveListener(OnLinkedWithFacebook);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string accessToken = _authenticationManager.GetToken(AuthProviderType.Facebook);
            await Manager.Client.LinkFacebookAsync(Manager.Session, accessToken);
        }

        private async void OnUnlinkWithFacebook()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInSuccess.RemoveListener(OnUnlinkWithFacebook);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string accessToken = _authenticationManager.GetToken(AuthProviderType.Facebook);
            await Manager.Client.UnlinkFacebookAsync(Manager.Session, accessToken);
        }

        private void LinkWithApple()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInSuccess.AddListener(OnLinkedWithApple);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Apple);
        }

        private void UnlinkWithApple()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInSuccess.AddListener(OnUnlinkedWithApple);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Apple);
        }

        private async void OnLinkedWithApple()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInSuccess.RemoveListener(OnLinkedWithApple);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string appleIdToken = _authenticationManager.GetToken(AuthProviderType.Apple);
            //var appleProvider = _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider as AppleAuthenticationProvider;
            //string rawNonce = appleProvider.RawNonce;
            await Manager.Client.LinkAppleAsync(Manager.Session, appleIdToken);
        }

        private async void OnUnlinkedWithApple()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInSuccess.RemoveListener(OnUnlinkedWithApple);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string appleIdToken = _authenticationManager.GetToken(AuthProviderType.Apple);
            //var appleProvider = _authenticationManager.GetAuthenticationMethod(AuthProviderType.Apple).Provider as AppleAuthenticationProvider;
            //string rawNonce = appleProvider.RawNonce;
            await Manager.Client.UnlinkAppleAsync(Manager.Session, appleIdToken);
        }

        private void LinkWithGoogle()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInSuccess.AddListener(OnLinkedWithGoogle);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Google);
        }

        private void UnlinkWithGoogle()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInSuccess.AddListener(OnUnlinkedWithGoogle);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInFailed.AddListener(OnSignInFailed);
            _authenticationManager.SignIn(AuthProviderType.Google);
        }

        private async void OnLinkedWithGoogle()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInSuccess.RemoveListener(OnLinkedWithGoogle);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string authCode = _authenticationManager.GetToken(AuthProviderType.Google);
            await Manager.Client.LinkGoogleAsync(Manager.Session, authCode);
        }

        private async void OnUnlinkedWithGoogle()
        {
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInSuccess.RemoveListener(OnUnlinkedWithGoogle);
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Google).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            string authCode = _authenticationManager.GetToken(AuthProviderType.Google);
            await Manager.Client.UnlinkGoogleAsync(Manager.Session, authCode);
        }

        private void OnSignInFailed()
        {
            _currentAuthProvider = AuthProviderType.None;
            PlayerPrefs.SetString(Commons.NakamaAuthMethodKey, _currentAuthProvider.ToString());
            _authenticationManager.GetAuthenticationMethod(AuthProviderType.Facebook).Provider.EvtSignInFailed.RemoveListener(OnSignInFailed);
            EvtSignInFailed?.Invoke();
        }
    }
}
