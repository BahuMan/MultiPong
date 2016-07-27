using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MPPlayerController : NetworkBehaviour {

    public string playerName;
    public Vector3 left;
    public Vector3 right;
    public float movementSpeed;
    public bool listenToKeyboard = false;

    private MPPongCoordinator PongCoordinator;
    private ChatWindowController ChatWindow;

    //position is kept track of as a float between 0.0f(=left) to 1.0f(=right)
    private float curPos;
    
    // Use this for initialization
    void Start () {
        curPos = 0.5f;
        transform.position = left + ((right - left) * curPos);
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer) return;

        if (listenToKeyboard)
        {
            curPos = Mathf.Clamp01(curPos + movementSpeed * Input.GetAxis("Horizontal"));
            transform.position = Vector3.Lerp(left, right, curPos);
        }
    }

    override public void OnStartLocalPlayer()
    {
        this.listenToKeyboard = true;
        CmdAnnounceMyself();
    }

    [Command]
    private void CmdAnnounceMyself()
    {
        //first tell all clients to register themselves
        RpcHelloEveryone();
    }
    [ClientRpc]
    private void RpcHelloEveryone()
    {
        this.PongCoordinator.RegisterPlayer(this);
    }

    override public void OnStartClient()
    {
        //find the coordinator and register ourselves (this should happen on server & clients)
        this.ChatWindow = GameObject.FindGameObjectWithTag("ChatWindow").GetComponent<ChatWindowController>();
        this.PongCoordinator = GameObject.FindGameObjectWithTag("GameController").GetComponent<MPPongCoordinator>();

        //this code is first executed server-side, and causes the new game to start even before there is a localplayer object
        //this.PongCoordinator.RegisterPlayer(this);
    }

    [ClientRpc]
    public void RpcPrepareForNewGame(MPGameSetup gamesetup)
    {
        this.left = gamesetup.goalLeft;
        this.right = gamesetup.goalRight;
        curPos = 0.5f;
        transform.position = left + ((right - left) * curPos);
        transform.LookAt(this.right);
        transform.rotation *= Quaternion.Euler(0f, 90f, 0f);

        //goals and walls will be spawned server-side
    }

}
