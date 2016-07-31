using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MPPongCoordinator : NetworkBehaviour {

    public float radius = 20f;

    public const float PADDLE_DISTANCE = 1.0f;
    public const float PADDLE_LENGTH = 3.0f;
    public const float PADDLE_HEIGHT = 1.0f;

    public ChatWindowController chatWindowController;
    public GameObject PrefabWall;
    public GameObject PrefabGoal;
    public GameObject PrefabBall;

    private LinkedList<MPPlayerController> PlayerList = new LinkedList<MPPlayerController>();
    private List<GameObject> LevelList = new List<GameObject>();
    private GameObject ball;

    public struct MPPlayerInfo
    {
        public string playerid;
        public Vector3 goalLeft;
        public Vector3 goalRight;
        public Vector3 playerLeft;
        public Vector3 playerRight;
        public float length;
        public float height;
    }


    override public void OnStartServer()
    {
        PlayerList = new LinkedList<MPPlayerController>();

        //now let's play ball!
        if (this.ball == null)
        {
            this.ball = CreateBall();
        }

        //previousHandlerAddPlayer = NetworkServer.handlers[MsgType.AddPlayer];
        //NetworkServer.RegisterHandler(MsgType.AddPlayer, OnPlayerAdded);

        //previousHandlerRemovePlayer = NetworkServer.handlers[MsgType.RemovePlayer];
        //NetworkServer.RegisterHandler(MsgType.RemovePlayer, OnPlayerRemoved);

        //previousHandlerDisconnect = NetworkServer.handlers[MsgType.Disconnect];
        //NetworkServer.RegisterHandler(MsgType.RemovePlayer, OnDisconnect);
    }

    NetworkMessageDelegate previousHandlerAddPlayer;
    private void OnPlayerAdded(NetworkMessage netMsg)
    {
        Debug.Log("Player added: " + netMsg.ToString());
        if (previousHandlerAddPlayer != null) previousHandlerAddPlayer(netMsg);
    }

    NetworkMessageDelegate previousHandlerRemovePlayer;
    private void OnPlayerRemoved(NetworkMessage netMsg)
    {
        Debug.Log("Played removed: " + netMsg.ToString());
        if (previousHandlerRemovePlayer != null) previousHandlerRemovePlayer(netMsg);
    }

    NetworkMessageDelegate previousHandlerDisconnect;
    private void OnDisconnect(NetworkMessage netMsg)
    {
        Debug.Log("Disconnect: " + netMsg.ToString());
        if (previousHandlerDisconnect != null) previousHandlerDisconnect(netMsg);
    }

    public void OnPongDisconnected(MPPlayerController gone)
    {
        Debug.Log("Server received player disconnect");
        this.PlayerList.Remove(gone);
        this.ServerStartNewGame();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P)) DebugListAllPlayers();
	}

    private void DebugListAllPlayers()
    {
        foreach(MPPlayerController p in this.PlayerList)
        {
            chatWindowController.addLine("(debug)", "player ctrl id " + p.playerControllerId);
            Debug.Log("player ctrl id " + p.playerControllerId);
        }

    }

    public void RegisterPlayer(MPPlayerController newPlayer)
    {
        chatWindowController.addLine("(local)", "New player joined with player ctrl id " + newPlayer.playerControllerId);
        this.PlayerList.AddLast(newPlayer);
        if (isServer) ServerStartNewGame();
    }

    private void ServerStartNewGame()
    {

        //start by deleting the previous level:
        foreach (GameObject o in this.LevelList)
        {
            NetworkServer.Destroy(o);
        }
        this.LevelList = new List<GameObject>();

        Debug.Log("initializing playfield");
        Vector3[] points = CreateEllipse(radius, radius, new Vector3(0f, 0f, 0f), this.PlayerList.Count * 2);
        GameObject wall;
        GameObject goal;


        var playerEnum = this.PlayerList.GetEnumerator();
        for (int i = 0; i < points.Length - 2; i += 2)
        {
            wall = Instantiate<GameObject>(PrefabWall);
            RotateAndSpawn(wall, points[i + 1], points[i + 2]);
            goal = Instantiate<GameObject>(PrefabGoal);
            RotateAndSpawn(goal, points[i], points[i + 1]);

            if (playerEnum.MoveNext())
            {
                MPGameSetup gs = new MPGameSetup();

                gs.goalLeft = points[i];
                gs.goalRight = points[i + 1];
                gs.height = PADDLE_HEIGHT;
                gs.length = PADDLE_LENGTH;
                gs.playerLeft = gs.goalLeft + (goal.transform.forward * PADDLE_DISTANCE);
                gs.playerRight = gs.goalRight + (goal.transform.forward * PADDLE_DISTANCE);
                goal.name = playerEnum.Current.playerName;

                playerEnum.Current.RpcPrepareForNewGame(gs);
            }
            else
            {
                Debug.LogError("mismatch between playfield size(" + (points.Length / 2) + ") and playercount(" + this.PlayerList.Count + ")");
            }

        }

        if (this.ball != null)
        {
            this.ball.transform.position = new Vector3(0f, 0f, 0f);
            //this.ball.GetComponent<Rigidbody>().velocity =
        }

    }

    private GameObject CreateBall()
    {
        GameObject ballobject = Instantiate<GameObject>(PrefabBall);
        ballobject.transform.position = new Vector3(0f, 0f, 0f);
        ballobject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        ballobject.name = "PongTestBall";
        NetworkServer.Spawn(ballobject);
        return ballobject;
    }


    #region utilities

    private GameObject RotateAndSpawn(GameObject newWall, Vector3 from, Vector3 to)
    {
        //add to the list of objects to be deleted when a new level is created:
        this.LevelList.Add(newWall);

        newWall.transform.position = (from + to) / 2f;
        newWall.transform.rotation = Quaternion.LookRotation(from - to) * Quaternion.AngleAxis(90f, newWall.transform.up);
        newWall.name = from.ToString() + " - " + to.ToString();

        SyncScale scr = newWall.GetComponent<SyncScale>();
        scr.scale = new Vector3(Vector3.Distance(from, to), 1f, 1f);
        NetworkServer.Spawn(newWall);
        //add to the list of objects to be deleted when a new level is created:
        this.LevelList.Add(newWall);

        return newWall;
    }

    private static Vector3[] CreateEllipse(float a, float b, Vector3 center, int resolution)
    {

        Vector3[] positions = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
            positions[i] = new Vector3(a * Mathf.Cos(angle), center.y, b * Mathf.Sin(angle));
            positions[i] = positions[i] + center;
        }

        return positions;
    }

    #endregion
}
