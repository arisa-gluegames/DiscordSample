using GlueGames.Authentication;
using Nakama;
using System;

namespace GlueGames.Nakama
{
    public class NakamaUserData : IUserData
    {
        private IApiAccount _account;
        public IApiAccount Account => _account;
        public IApiUser NakamaUser => _account.User;

        public string Wallet => _account.Wallet;
        public string DisplayName => _account.User.DisplayName;
        public string Email => _account.Email;
        public string PhotoUrl => _account.User.AvatarUrl;
        public string UserId => _account.User.Id;
        public string Username => _account.User.Username;
        public DateTime CreationDate
        {
            get
            {
                if (_account == null) return DateTime.UtcNow;
                return DateTime.Parse(_account.User.CreateTime);
            }
        }

        public NakamaUserData()
        {
            _account = null;
        }

        public void SetNakamaAccount(IApiAccount account)
        {
            _account = account;
        }
        
    }
}

