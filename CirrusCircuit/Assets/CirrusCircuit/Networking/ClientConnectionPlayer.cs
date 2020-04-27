using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{
    // Serves to sync the connection
    public class ClientConnectionPlayer : NetworkBehaviour
    {
        public static ClientConnectionPlayer Instance;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Instance = this;
        }        

        private AutoResetEvent _serverResponseEvent = new AutoResetEvent(false);
        private ServerMessage _serverResponse = null;
        private Mutex _mutex = new Mutex(false);

        // TODO lock
        public ServerMessage WaitResponse(int millisecondsTimeout)
        {
            _mutex.WaitOne();
            if(!_serverResponseEvent.WaitOne(millisecondsTimeout)) return new ServerMessage() { Id = ServerMessageId.Timeout };
            if (_serverResponse == null) return new ServerMessage() { Id = ServerMessageId.NullResponse };
            var response = _serverResponse;
            _serverResponse = null;
            _mutex.ReleaseMutex();
            return response;
        }

        [TargetRpc]
        public void TargetReceive(ServerMessage msg)
        {            
            _serverResponseEvent.Set();
            _serverResponse = msg;
        }

        [Command]
        public void CmdTryChangeState_CharacterSelectSlot(GameObject obj, CharacterSelectSlot.State target)
        {            
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcTryChangeState(target);          
        }

        public void TryChangeState_Game(Game.State transition, params object[] args)
        {
            CmdTryChangeState_Game(transition, args);
        }

        [Command]
        public void CmdTryChangeState_Game(Game.State transition, object[] args)
        {
            RpcTryChangeState_Game(transition, args);
        }

        [ClientRpc]
        public void RpcTryChangeState_Game(Game.State transition, object[] args)
        {            
            Game.Instance.TryChangeState(transition, args);
        }

        [Command]
        public void CmdScroll_CharacterSelectSlot(GameObject obj, bool scroll)
        {
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcScroll(scroll);
        }
    }
}
