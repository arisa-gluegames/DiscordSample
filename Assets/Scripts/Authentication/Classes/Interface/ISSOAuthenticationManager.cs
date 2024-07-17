using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GlueGames.Authentication
{
    public interface ISSOAuthenticationManager 
    {
        public List<AuthenticationMethod> AuthenticationMethods { get; }
        public void SignIn(AuthProviderType type, bool isQuickSignIn = false);
        public Task<bool> SignInAsync(AuthProviderType type, bool isQuickSignIn = false);
        public void SignOut(AuthProviderType type);
        public string GetToken(AuthProviderType type);
        public AuthenticationMethod GetAuthenticationMethod(AuthProviderType type);
    }
}
