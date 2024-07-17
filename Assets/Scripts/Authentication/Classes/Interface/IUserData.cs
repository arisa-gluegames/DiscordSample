using System;

namespace GlueGames.Authentication
{
    public interface IUserData
    {
        public string UserId { get; }
        public string DisplayName { get; }
        public string Email { get; }
        public string PhotoUrl { get; }
        public DateTime CreationDate { get; }
    }
}
