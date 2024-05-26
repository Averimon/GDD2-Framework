using UnityEngine;
using Mirror;

namespace Framework.Networking
{
    public class NetworkManagerExtension : NetworkManager
    {
        public PlayerJoin OnPlayerJoin;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            OnPlayerJoin.Invoke(conn.connectionId);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            Debug.Log($"Client connected: {conn.connectionId}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Client disconnected: {conn.connectionId}");
            base.OnServerDisconnect(conn);
        }

    }
}
