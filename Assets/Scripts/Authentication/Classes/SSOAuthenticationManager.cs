using UnityEngine;
using GlueGames.Utilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GlueGames.Authentication
{
    public class SSOAuthenticationManager :  SingletonPersistent<SSOAuthenticationManager>, ISSOAuthenticationManager
    {
        [SerializeField]
        private List<AuthenticationMethod> _authenticationMethods;

        public List<AuthenticationMethod> AuthenticationMethods => _authenticationMethods;

        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        public UnityEvent EvtSSOSignInFailed { get; } = new();
        public UnityEvent EvtSSOSignInSuccess { get; } = new();

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            foreach (var method in _authenticationMethods)
            {
                method.Provider.Initialize();
                method.Provider.EvtSignInSuccess.AddListener(() => EvtSSOSignInSuccess?.Invoke());
                method.Provider.EvtSignInFailed.AddListener(() => EvtSSOSignInFailed?.Invoke());
            }
            _isInitialized = true;
        }

        public void SignIn(AuthProviderType type, bool isQuickSignIn = false)
        {
            foreach (var method in _authenticationMethods)
            {
                if(method.Type == type)
                {
                    method.Provider.SignIn(isQuickSignIn);
                    return;
                }
            }
        }

        public void SignOut(AuthProviderType type)
        {
            foreach (var method in _authenticationMethods)
            {
                if (method.Type == type)
                {
                    method.Provider.SignOut();
                    return;
                }
            }
        }

        public async Task<bool> SignInAsync(AuthProviderType type, bool isQuickSignIn = false)
        {
            foreach (var method in _authenticationMethods)
            {
                if (method.Type == type)
                {
                    await method.Provider.SignInAsync(isQuickSignIn);
                    return true;
                }
            }
            return false;
        }

        public string GetToken(AuthProviderType type)
        {
            foreach (var method in _authenticationMethods)
            {
                if (method.Type == type)
                {
                    return method.Provider.GetToken();
                }
            }
            return string.Empty;
        }

        public AuthenticationMethod GetAuthenticationMethod(AuthProviderType type)
        {
            foreach (var method in _authenticationMethods)
            {
                if (method.Type == type)
                {
                    return method;
                }
            }
            return null;
        }
    }
}