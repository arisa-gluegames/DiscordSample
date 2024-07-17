
using UnityEngine;

namespace GlueGames.Authentication
{
    public abstract class AuthenticationEventListener : MonoBehaviour
    {
        protected IAuthenticationBackend AuthenticationBackend => AuthenticationManager.Instance.AuthenticationBackend;
        public abstract void AddListeners();
    }
}
