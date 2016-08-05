using UnityEngine;
using UnityEngine.Networking;

public class PongNetworkManager: NetworkManager
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

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("NetworkManager.OnServerConnect address=" + conn.address + " #players = " + conn.playerControllers.Count);
        foreach (PlayerController pc in conn.playerControllers)
        {
            //PongCoordinator.OnPongDisconnected(pc.gameObject.GetComponent<MPPlayerController>());
        }
        base.OnServerConnect(conn);
    }

}
