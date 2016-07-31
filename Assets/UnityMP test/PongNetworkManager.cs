using UnityEngine;
using UnityEngine.Networking;

class PongNetworkManager: NetworkManager
{


    public MPPongCoordinator PongCoordinator;

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("NetworkManager.OnServerDisconnect address=" + conn.address + " #players = " + conn.playerControllers.Count);
        foreach (PlayerController pc in conn.playerControllers)
        {
            PongCoordinator.OnPongDisconnected(pc.gameObject.GetComponent<MPPlayerController>());
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Debug.Log("NetworkManager.OnServerRemovePlayer address=" + conn.address + ", playerctrl=" + player.playerControllerId);
        base.OnServerRemovePlayer(conn, player);
    }
}
