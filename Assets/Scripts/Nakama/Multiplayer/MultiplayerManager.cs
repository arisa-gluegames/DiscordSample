using GlueGames.Utilities;
using Nakama;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GlueGames.Nakama
{
    public partial class MultiplayerManager : SingletonPersistent<MultiplayerManager>
    {
        private const string LogFormat = "{0} with code {1}:\n{2}";
        private const string SendingDataLog = "Sending data";
        private const string SentDataLog = "Sent data";
        private const string ReceivedDataLog = "Received data";
        private const string JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";

        public IUserPresence Self { get => _match?.Self; }

        public UnityEvent EvtMatchJoined;
        public UnityEvent EvtMatchLeft;
        public UnityEvent EvtSwitchToOffline { get; } = new();

        [SerializeField]
        private bool _enableLog = true;

        private readonly Dictionary<Code, UnityAction<MultiplayerMessage>> _onReceivedData = new();
        private IMatch _match = null;

        public IMatch Match => _match;

        private readonly Queue<(Code, string)> _pendingMessages = new();

        public void Subscribe(Code code, UnityAction<MultiplayerMessage> action)
        {
            if (!_onReceivedData.ContainsKey(code))
                _onReceivedData.Add(code, null);

            _onReceivedData[code] += action;
        }

        public void Unsubscribe(Code code, UnityAction<MultiplayerMessage> action)
        {
            if (_onReceivedData.ContainsKey(code))
                _onReceivedData[code] -= action;
        }

        public async void JoinMatchAsync()
        {
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
            NakamaManager.Instance.EvtSocketReconnectionSuccess.AddListener(ResendQueuedMessages);
            IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinOrCreateMatchRpc);
            string matchId = rpcResult.Payload;
            _match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId);
            LogManager.Log("Match joined successfully");
            EvtMatchJoined?.Invoke();
        }

        public async Task LeaveMatchAsync()
        {
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            NakamaManager.Instance.EvtSocketReconnectionSuccess.RemoveListener(ResendQueuedMessages);
            _pendingMessages.Clear();

            if (_match == null)
            {
                LogManager.Log("Cannot leave when match is null");
                return;
            }

            await NakamaManager.Instance.Socket.LeaveMatchAsync(_match);
            LogManager.LogWarningInfo("Match left successfully");
            _match = null;
            EvtMatchLeft?.Invoke();
        }

        public void Send(Code code, object data = null)
        {
            if (_match == null)
            {
                LogManager.LogWarning("Cannot send code when match is null");
                return;
            }


            string json = data != null ? data.Serialize() : string.Empty;

            if (NakamaManager.Instance.Socket.IsConnected)
            {
                SendMessage(code, json);
            }
            else
            {
                // Queue unsent messages here?
                LogManager.LogWarningInfo($"Failed to send {code} {json}. Try queueing this message");
                _pendingMessages.Enqueue((code, json));
            }
        }

        private void ResendQueuedMessages()
        {
            if (_pendingMessages == null || _pendingMessages.Count <= 0) return;
            (Code code, string json) message = _pendingMessages.Peek();
            SendMessage(message.code, message.json, () => { _pendingMessages.Dequeue(); });
        }

        private async void SendMessage(Code code, string json, UnityAction OnMessageSent = null)
        {
            try
            {
                await NakamaManager.Instance.Socket.SendMatchStateAsync(_match.Id, (long)code, json);
                if (_enableLog) LogData(SendingDataLog, (long)code, json);
                OnMessageSent?.Invoke();
            }
            catch (Exception e)
            {
                LogManager.LogError($"Error in sending match state async: {e}");
            }
        }

        private void Receive(IMatchState newState)
        {
            if (_enableLog)
            {
                var encoding = System.Text.Encoding.UTF8;
                var json = encoding.GetString(newState.State);
                LogData(ReceivedDataLog, newState.OpCode, json);
            }

            MultiplayerMessage multiplayerMessage = new MultiplayerMessage(newState);
            if (_onReceivedData.ContainsKey(multiplayerMessage.DataCode))
                _onReceivedData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
        }

        private void Disconnected()
        {
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            _match = null;
            EvtMatchLeft?.Invoke();
        }

        private void LogData(string description, long dataCode, string json)
        {
            LogManager.LogInfo(string.Format(LogFormat, description, (Code)dataCode, json));
        }
    }
}

