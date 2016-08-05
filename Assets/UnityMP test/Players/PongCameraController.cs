using UnityEngine;
using UnityEngine.Networking;

class PongCameraController: MonoBehaviour
{

    public Vector3 offset = new Vector3(0f, 5f, -5f);
    private MPPlayerController player;
    private bool followPlayer = false;

    //making this a property so it won't show up in editor to override.
    public MPPlayerController target
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
            followPlayer = true;
            RepositionCamera();
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R)) RepositionCamera();
        if (Input.GetKeyDown(KeyCode.F)) followPlayer = !followPlayer;
        if (followPlayer)
        {
            transform.LookAt(player.transform);
        }
    }

    public void RepositionCamera()
    {
        transform.position = (player.left + player.right / 2);
        transform.LookAt(player.right);
        transform.rotation *= Quaternion.Euler(0f, -90f, 0f);
        transform.position += transform.rotation * this.offset;
    }

}

