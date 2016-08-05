using UnityEngine;
using UnityEngine.Networking;

class MPGoalController: NetworkBehaviour
{

    private MPPlayerController associatedPlayer;

    [Server]
    public void setPlayer(MPPlayerController p)
    {
        associatedPlayer = p;
    }

    [Server]
    public MPPlayerController getPlayer()
    {
        return associatedPlayer;
    }
}
