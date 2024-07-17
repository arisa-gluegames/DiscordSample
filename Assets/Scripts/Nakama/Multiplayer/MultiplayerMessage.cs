using GlueGames.Utilities;
using Nakama;

namespace GlueGames.Nakama
{
    public class MultiplayerMessage
    {
        private readonly string _json = null;
        private readonly byte[] _bytes = null;

        public MultiplayerManager.Code DataCode { get; private set; }
        public string SessionId { get; private set; }
        public string UserId { get; private set; }
        public string Username { get; private set; }

        public MultiplayerMessage(IMatchState matchState)
        {
            DataCode = (MultiplayerManager.Code)matchState.OpCode;
            if (matchState.UserPresence != null)
            {
                UserId = matchState.UserPresence.UserId;
                SessionId = matchState.UserPresence.SessionId;
                Username = matchState.UserPresence.Username;
            }

            var encoding = System.Text.Encoding.UTF8;
            _json = encoding.GetString(matchState.State);
            _bytes = matchState.State;
        }

        public T GetData<T>()
        {
            return _json.Deserialize<T>();
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }
    }
}
