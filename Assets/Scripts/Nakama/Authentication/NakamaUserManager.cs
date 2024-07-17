using GlueGames.Authentication;
using GlueGames.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace GlueGames.Nakama
{
    public class NakamaUserManager : SingletonPersistent<NakamaUserManager>
    {
        private NakamaUserData _nakamaUserData;
        public NakamaUserData NakamaUserData => _nakamaUserData;
        public IUserData UserData => _nakamaUserData;
        public UnityEvent OnAccountLoaded { get; } = new();
        public void Initialize()
        {
            _nakamaUserData = new();
        }

        public async void UpdateDisplayName(string displayName)
        {
            await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session, _nakamaUserData.NakamaUser.Username, displayName, canceller: destroyCancellationToken);
        }

        public async void UpdateAvatarUrl(string url)
        {
            await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session,
                _nakamaUserData.NakamaUser.Username, _nakamaUserData.NakamaUser.DisplayName, url, canceller: destroyCancellationToken);
        }

        public T GetWallet<T>()
        {
            if (_nakamaUserData.Account == null || _nakamaUserData.Account.Wallet == null)
                return default;

            return _nakamaUserData.Account.Wallet.Deserialize<T>();
        }

        public async Task LoadUserAccount()
        {
            LogManager.Log($"Session: {NakamaManager.Instance.Session}");
            var account = await NakamaManager.Instance.Client.GetAccountAsync(NakamaManager.Instance.Session);
            _nakamaUserData.SetNakamaAccount(account);
            LogManager.Log($"User authenticated with nakama. DisplayName: {_nakamaUserData.DisplayName} UserName: {_nakamaUserData.NakamaUser.Username} ID: {_nakamaUserData.UserId}");
            OnAccountLoaded?.Invoke();
        }
    }
}
