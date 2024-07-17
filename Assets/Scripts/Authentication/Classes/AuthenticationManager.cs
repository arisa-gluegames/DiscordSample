using System.Threading.Tasks;
using GlueGames.Utilities;

namespace GlueGames.Authentication
{
    public class AuthenticationManager : SingletonPersistent<AuthenticationManager>
    {
        private IAuthenticationFactory _factory;
        private IAuthenticationFactory _multiplayerFactory;
        private IAuthenticationBackend _authenticationBackend;
        private IAuthenticationBackend _multiplayerAuthenticationBackend;
        public IAuthenticationBackend AuthenticationBackend => _authenticationBackend;
        public IAuthenticationBackend MultiplayerAuthenticationBackend => _multiplayerAuthenticationBackend;
        public bool IsInitialized { get; private set; } = false;
        public bool HasLoginHistory => AuthenticationBackend.HasLoginHistory;

        public void Initialize(bool isOffline, IAuthenticationFactory authenticationFactory, IAuthenticationFactory multiplayerFactory, ISSOAuthenticationManager ssOAuthentication)
        {
            if (IsInitialized) return;
            
            _factory = authenticationFactory;
            _multiplayerFactory = multiplayerFactory;
            // Initialize the Firebase Auth
            _authenticationBackend = _factory.CreateAuthenticationBackend(ssOAuthentication);
            _authenticationBackend.EvtInitialized.AddListener(OnAuthBackendInitialized);

            if (!isOffline)
            {
                // When user signs in with Firebase, authenticate to Nakama
                _multiplayerAuthenticationBackend = _multiplayerFactory.CreateAuthenticationBackend(ssOAuthentication);
                _multiplayerAuthenticationBackend.Initialize();
                _authenticationBackend.EvtSignInSuccess.AddListener(MultiplayerSignInOnAuthenticationBackEndSuccess);
                _authenticationBackend.EvtSignOutSuccess.AddListener(MultiplayerSignOut);
            }

            AuthenticationEventListener[] authenticationEvents = GetComponentsInChildren<AuthenticationEventListener>();
            foreach (var authEvent in authenticationEvents)
            {
                authEvent.AddListeners();
            }

            _authenticationBackend.Initialize();
        }

        private void MultiplayerSignInOnAuthenticationBackEndSuccess()
        {
            MultiplayerSignIn();
        }

        private async Task<bool> MultiplayerSignIn()
        {
            var nakamaAuth = _multiplayerAuthenticationBackend as IMultiPlayerAuthenticationAuthenticationBackend;
            // Pass in the Firebase authentication userId to Nakama
            var success = await nakamaAuth.CustomLoginAsync(_authenticationBackend.UserData.UserId);
            return success;
        }

        public Task<bool> MultiplayerLateSignIn()
        {
            return MultiplayerSignIn();
        }

        private void MultiplayerSignOut()
        {
            _multiplayerAuthenticationBackend.SignOut();
        }

        private void OnAuthBackendInitialized()
        {
            IsInitialized = true;
            _authenticationBackend.EvtInitialized.RemoveListener(OnAuthBackendInitialized);
            LogManager.LogInfo("Authentication Initialized");
        }
    }
}