using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueGames.Authentication
{
    public interface IAuthenticationFactory
    {
        public IAuthenticationBackend CreateAuthenticationBackend(ISSOAuthenticationManager ssoAuthentication);
    }
}
