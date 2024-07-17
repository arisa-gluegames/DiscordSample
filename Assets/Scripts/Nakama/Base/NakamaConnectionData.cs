using UnityEngine;

namespace GlueGames.Nakama
{
    [CreateAssetMenu(menuName = "Nakama/Connection data")]
    public class NakamaConnectionData : ScriptableObject
    {
        // Temporarily adding this to test login even without server
        [Header("Nakama Connection")]
        [SerializeField] private string _scheme = null;
        [SerializeField] private string _host = null;
        [SerializeField] private int _port = default(int);
        [SerializeField] private string _serverKey = null;
        [Header("Socket Settings")]
        [SerializeField] private int _socketTimeOut = 60;
        [SerializeField] private int _keepAliveIntervalSec = 5;
        [SerializeField] private int _sendTimeOut = 15;

       
        public string Scheme => _scheme;
        public string Host => _host;
        public int Port => _port;
        public string ServerKey => _serverKey;
        public int SocketTimeOut => _socketTimeOut;
        public int KeepAliveInterval => _keepAliveIntervalSec;
        public int SendTimeOut => _sendTimeOut;
    }

}
