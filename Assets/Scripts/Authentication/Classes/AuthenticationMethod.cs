namespace GlueGames.Authentication
{
    public enum AuthProviderType
    {
        None,
        Facebook,
        Google,
        Apple,
        Device,
        Email,
        Custom
    }

    [System.Serializable]
    public class AuthenticationMethod
    {
        public AuthProviderType Type;
        public AuthenticationProvider Provider;
    }
}