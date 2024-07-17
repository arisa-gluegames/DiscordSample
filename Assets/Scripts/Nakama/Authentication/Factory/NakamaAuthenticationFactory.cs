using GlueGames.Authentication;

namespace GlueGames.Nakama
{
    public class NakamaAuthenticationFactory : IAuthenticationFactory
    {
        public IAuthenticationBackend CreateAuthenticationBackend(ISSOAuthenticationManager ssoAuthentication)
        {
            return new NakamaAuthenticationHandler(ssoAuthentication);
        }
    }
}

