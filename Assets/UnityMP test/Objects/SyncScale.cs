using UnityEngine;
using UnityEngine.Networking;

class SyncScale: NetworkBehaviour
{
    [SyncVar(hook = "OnScaleChanged")]
    public Vector3 scale;

    void Start()
    {
        transform.localScale = this.scale;
    }

    void OnScaleChanged(Vector3 newScale)
    {
        transform.localScale = newScale;
    }

}
