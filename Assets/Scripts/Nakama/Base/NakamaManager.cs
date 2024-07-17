using GlueGames.Utilities;
using Nakama;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GlueGames.Nakama
{
    public class NakamaManager : SingletonPersistent<NakamaManager>
    {
        /// <summary>
        /// Used to establish connection between the client and the server.
        /// Contains a list of usefull methods required to communicate with Nakama server.
        /// </summary>
        public IClient Client => _client;
        /// <summary>
        /// Used to communicate with Nakama server.
        ///  /// </summary>
        public ISession Session => _session;
        /// <summary>
        /// Socket responsible for maintaining connection with Nakama server
        /// and exchange realtime messages.
        /// </summary>
        public ISocket Socket => _socket;

        [SerializeField] private NakamaConnectionData _connection;

        private IClient _client = null;
        private ISession _session = null;
        private ISocket _socket = null;
        private bool _connected = false;
        private bool _shouldReconnect = false;

        public UnityEvent EvtSocketConnecting { get; } = new();
        public UnityEvent EvtSocketConnected { get; } = new();
        public UnityEvent EvtSocketDisconnected { get; } = new();
        public UnityEvent EvtSocketReconnectionSuccess { get; } = new();
        public UnityEvent EvtSocketReconnectionFailed { get; } = new();
        public bool IsSocketDisconnected => !Socket.IsConnected;
        public bool Connected => _connected;
        public bool ShouldReconnect => _shouldReconnect;

        public bool IsInitialized { get; private set; }
        public void Initialize()
        {
            _client = new Client(_connection.Scheme, _connection.Host, _connection.Port, _connection.ServerKey, UnityWebRequestAdapter.Instance)
            {
                Timeout = _connection.SocketTimeOut
            };

            _socket = Client.NewSocket(useMainThread: true, new WebSocketAdapter(_connection.KeepAliveInterval, _connection.SendTimeOut));
            _socket.Connected += Connect;
            _socket.Closed += Disconnect;
            _socket.ReceivedError += ReceiveError;
            IsInitialized = true;
            LogManager.LogInfo("Nakama Initialized");
        }

        private void ReceiveError(Exception exception)
        {
            LogManager.LogError(exception);
        }

        private void Disconnect()
        {
            LogManager.LogInfo("!!!Nakama socket has been disconnected!!!");
            _connected = false;
            EvtSocketDisconnected?.Invoke();
        }

        private void Connect()
        {
            LogManager.LogInfo("!!!Nakama socket is connected!!!");
            _connected = true;
            _shouldReconnect = true;
            EvtSocketConnected?.Invoke();
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        public async void AttemptRefreshSocket(UnityAction OnConnectionRefreshed, UnityAction OnConnectionFailed)
        {
            if (Session == null || Socket == null)
            {
                LogManager.LogWarningInfo("Cannot attempt to refresh socket when session or socket is null");
                EvtSocketReconnectionFailed?.Invoke();
                OnConnectionFailed?.Invoke();
                return;
            }
            try
            {
                LogManager.LogInfo("Attempting socket reconnection...");
                await Socket.ConnectAsync(Session);
                LogManager.LogInfo("!!!Socket has been reconnected!!!");
                OnConnectionRefreshed?.Invoke();
            }
            catch (Exception e)
            {
                if (e.Message == "401 Unauthorized")
                {
                    LogManager.LogError("Session seems to be invalid: " + e.ToString());
                    SetSession(null);
                    AttemptRefreshSocket(OnConnectionRefreshed, OnConnectionFailed);
                    return;
                }
                LogManager.LogError(e.ToString());
                EvtSocketReconnectionFailed?.Invoke();
                OnConnectionFailed?.Invoke();
            }
        }

        public async Task<IApiRpc> SendRPC(string rpc, string payload = "{}")
        {
            if (Client == null || _session == null)
                return null;
            LogManager.Log($"Sending {rpc}");
            return await Client.RpcAsync(_session, rpc, payload);
        }

        public async Task CloseSocket(bool shouldReconnect = true)
        {
            _shouldReconnect = shouldReconnect;
            await _socket.CloseAsync();
            LogManager.LogInfo("Nakama Socket Closed.");
        }
    }
}
