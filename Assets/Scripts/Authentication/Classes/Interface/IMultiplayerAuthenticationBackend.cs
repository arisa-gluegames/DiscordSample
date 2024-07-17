using System.Threading.Tasks;

namespace GlueGames.Authentication
{
    public interface IMultiPlayerAuthenticationAuthenticationBackend: IAuthenticationBackend
    {
        public Task<bool> CustomLoginAsync(string customUserId);
    }
}

