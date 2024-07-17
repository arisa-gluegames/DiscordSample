using GlueGames.Utilities;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GlueGames.Authentication
{
    public abstract class AuthenticationProvider : MonoBehaviour
    {
        [SerializeField]
        protected bool _enableLog;

        public abstract void Initialize();
        public abstract void SignIn(bool isQuickSignIn = false);
        public abstract Task<bool> SignInAsync(bool isQuickSignIn = false);
        public abstract void SignOut();
        public abstract string GetToken();

        public UnityEvent EvtSignInSuccess { get; } = new();
        public UnityEvent EvtSignInFailed { get; } = new();

        protected void LogMessage(string message)
        {
            if (!_enableLog)
            {
                return;
            }
            LogManager.LogInfo(message);
        }
    }
}