using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PongCoordinator : MonoBehaviour {

    #region variables

    public string localPlayerName = "Bart Huylebroeck";
    public string ServerURL = "ws://localhost:8080";
    public float radius = 20f;

    public ChatWindowController chatWindowController;
    public GameObject PrefabWall;
    public GameObject PrefabGoal;
    public GameObject PrefabPaddle;
    public Rigidbody ball;

    public const float PADDLE_DISTANCE = 1.0f;
    public const float PADDLE_LENGTH = 3.0f;
    public const float PADDLE_HEIGHT = 1.0f;

    public const string TYPE_TYPE = "type";
    public const string TYPE_JOIN = "PlayerJoined";
    public const string TYPE_CHAT = "chat";
    public const string TYPE_WALL = "createWall";
    public const string TYPE_SETUP_GAME = "setupGame";

    public enum CoordinatorStatus { INIT, HOSTING_WAITING_FOR_PLAYERS, JOINING, HOSTING_STARTING, HOSTING_PLAYING, JOINED_WAITING, JOINED_STARTING, JOINED_PLAYING };

    public CoordinatorStatus status = CoordinatorStatus.INIT;
    private PongWebSockets pongWebSockets;

    private Dictionary<string, PongPlayer> playerInfo;
    private PongPlayer localPlayer = null;
    private Hashtable parsedGameSetup;
    #endregion

    /**
     * called by any GUI component to start hosting a game.
     * We will start listening for "playerjoined" requests and start a game.
     **/
    public void HostGame(string url)
    {
        Debug.Log("Host starting game");
        status = CoordinatorStatus.HOSTING_WAITING_FOR_PLAYERS;
        playerInfo = new Dictionary<string, PongPlayer>();
        pongWebSockets.Reconnect(url);

        //join my own game:
        SendPlayerJoin(localPlayerName);
        chatWindowController.addLine("server", "You're the host! Press the 'fire' button to start the game");
    }

    /**
     * called by any GUI component to join a remote game.
     * The remote host should tell us what game we're in and where our paddle is in world coordinates
     **/
    public void JoinGame(string url)
    {
        Debug.Log("joining game");
        status = CoordinatorStatus.JOINED_WAITING;
        playerInfo = new Dictionary<string, PongPlayer>();
        pongWebSockets.Reconnect(url);
        SendPlayerJoin(localPlayerName);
    }

    #region websockets

    private bool PongWebSockets_wsMessageArrived(string msg, object JSONParsed)
    {
        checkJSON(msg, JSONParsed);

        Hashtable jsonhash = (Hashtable)JSONParsed;
        string msgType = (string) jsonhash[TYPE_TYPE];
        Debug.Log("PongCoordinator received msg='" + msgType + "'");
        if (msgType.Equals(TYPE_JOIN))
        {
            ReceivePlayerJoined(jsonhash);
            return true;
        }
        else if (msgType.Equals(TYPE_CHAT))
        {
            ReceiveChatMessage(jsonhash);
            return true;
        }
        else if (msgType.Equals(TYPE_SETUP_GAME))
        {
            return ReceiveGameSetup(jsonhash);
        }
        return false;
    }

    private void ReceivePlayerJoined(Hashtable parsedPlayer)
    {

        //construct a place-holder player info. The coordinates and other details will be communicated by the host (later)
        PongPlayer playa = new PongPlayer((string)parsedPlayer[PongPlayer.FIELD_PLAYERID]);

        Debug.Log("player joined: " + playa.playerid);
        if (status == CoordinatorStatus.HOSTING_PLAYING)
        {
            status = CoordinatorStatus.HOSTING_WAITING_FOR_PLAYERS;
        }
        if (this.localPlayerName.Equals(playa.playerid))
        {
            Debug.Log("hey, that's me joining!");
            if (status == CoordinatorStatus.JOINING)
            {
                status = CoordinatorStatus.JOINED_STARTING;
            }
            //@TODO: bind this player's paddle to local keys
            //CORRECTION: binding should only happen during setup of the game (when host is building playfield)
            localPlayer = playa;
        }
        playerInfo.Add(playa.playerid, playa);
        Debug.Log("Total #players joined: " + playerInfo.Count);

    }

    private void SendPlayerJoin(string playerid)
    {
        //this is used by host and players to make themselves known.
        pongWebSockets.wsMessage(PongSerializer.forPlayerJoin(playerid));
    }

    private void SendGameSetup(Dictionary<string, PongPlayer> players)
    {
        //this is used by host and players to make themselves known.
        pongWebSockets.wsMessage(PongSerializer.forGameSetup(players));
    }

    private bool ReceiveGameSetup(Hashtable parsed)
    {
        if (status == CoordinatorStatus.HOSTING_PLAYING)
        {
            Debug.Log("PongCoordinator received echo of my own GameSetup -- ignoring. (it's all good)");
            return true;
        }

        if (status == CoordinatorStatus.JOINED_PLAYING || status == CoordinatorStatus.JOINED_WAITING)
        {
            //since I can't create unity objects outside the FixedUpdate() method,
            //I'm simply setting the status and storing the parsedGameSetup;
            //it will be used in UpdateJoinedStarting()
            this.parsedGameSetup = parsed;
            status = CoordinatorStatus.JOINED_STARTING;
            return true;
        }

        Debug.LogError("PongCoordinator should not be in status '" + status + "' when receiving gamesetup message");
        return false;
    }

    /**
     * Called by the chat GUI component whenever user entered a line in the chatbox.
     * it will be serialized into JSON and broadcast to other players.
     **/
    public void SendChatMessage(string msg)
    {
        pongWebSockets.wsMessage(PongSerializer.forChatMessage(localPlayerName, msg));
    }

    private void ReceiveChatMessage(Hashtable chat)
    {
        chatWindowController.addLine((string)chat["from"], (string)chat["msg"]);
    }

    private void checkJSON(string msg, object JSONParsed)
    {
        if (JSONParsed == null)
        {
            Debug.LogError("unparsed msg; aborting! " + msg);
            throw new System.Exception("unparsed msg: " + msg);
        }

        if (!(JSONParsed is Hashtable))
        {
            Debug.LogError("json did not result in hashtable ?!" + msg);
            throw new System.Exception("json did not result in hashtable ?!" + msg);
        }
    }

    #endregion

    // Use this for initialization
    void Start () {


        pongWebSockets = new PongWebSockets();
        pongWebSockets.wsMessageArrived += PongWebSockets_wsMessageArrived;

    }

    void FixedUpdate()
    {
        if (status == CoordinatorStatus.HOSTING_PLAYING)
        {
            //disable temporarily to allow debugging of the JSON messages during playfield setup
            //UpdateHostPlaying();
        }
        else if (status == CoordinatorStatus.HOSTING_WAITING_FOR_PLAYERS)
        {
            UpdateHostWaiting();
        }
        else if (status == CoordinatorStatus.JOINED_STARTING)
        {
            UpdateJoinedStarting();
        }
        else if (status == CoordinatorStatus.JOINED_PLAYING)
        {
            UpdateJoinedPlaying();
        }

    }

    private void UpdateJoinedStarting()
    {
        //we're starting a new game
        this.playerInfo.Clear(); //@TODO: destroy gameobject paddles from previous game
        ArrayList players = (ArrayList)parsedGameSetup[PongPlayer.ARRAY_PLAYERS];
        PongPlayer previousPlayer = null;
        for (int p = 0; p < players.Count; ++p)
        {
            PongPlayer newPlayer = new PongPlayer((Hashtable)players[p]);
            
            //now create the walls & goals:
            GameObject goal = Instantiate<GameObject>(PrefabGoal);
            createWall(goal, newPlayer.goalLeft, newPlayer.goalRight);
            goal.name = "Goal for " + newPlayer.playerid;

            newPlayer.paddle = CreatePaddle("Paddle for " + newPlayer.playerid, goal.transform.rotation, newPlayer.playerLeft, newPlayer.playerRight);
            //@TODO: for local paddle, bind keys to the newly instantiated paddle
            this.playerInfo.Add(newPlayer.playerid, newPlayer);
            //@TODO: link goal to correct user, for points and awards

            if (previousPlayer != null)
            {
                GameObject wall = Instantiate<GameObject>(PrefabWall);
                createWall(wall, previousPlayer.goalRight, newPlayer.goalLeft);
            }
            previousPlayer = newPlayer;
        }

        //some ugly shenanigans to create the last wall (between first and last player)
        GameObject lastwall = Instantiate<GameObject>(PrefabWall);
        createWall(lastwall, previousPlayer.goalRight, PongSerializer.toVector((Hashtable) ((Hashtable)players[0])[PongPlayer.FIELD_GOALLEFT]));

        //next frame, we can start playing!
        status = CoordinatorStatus.JOINED_PLAYING;
    }


    private void UpdateJoinedPlaying()
    {

    }

    private void UpdateHostPlaying()
    {
        //@TODO: process all player updates from websockets

        //@TODO: loop balls and broadcast all ball positions
        this.pongWebSockets.wsMessage(PongSerializer.forGameObject(this.ball.gameObject));

        //broadcast all player positions
        foreach (PongPlayer p in this.playerInfo.Values)
        {
            this.pongWebSockets.wsMessage(PongSerializer.forGameObject(p.paddle));
        }
    }

    private void UpdateHostWaiting()
    {
        //while waiting, do nothing until fire is pressed
        if (!Input.GetButton("Fire1")) return;

        //if host presses fire, init the play field:
        StartNewGame();
    }

    private void StartNewGame()
    {
        Debug.Log("initializing playfield");
        Vector3[] points = CreateEllipse(radius, radius, new Vector3(0f, 0f, 0f), this.playerInfo.Count * 2);
        GameObject wall;
        GameObject goal;

        //@TODO: initialize & position balls & players
        this.ball.name = "PongTestBall";
        //this.localPlayer.paddle = Instantiate<GameObject>(PrefabPadle);

        var playerEnum = this.playerInfo.GetEnumerator();
        for (int i = 0; i < points.Length - 2; i += 2)
        {
            //@TODO: should this stay here, or move to the corresponding receiveGameSetup? The server may not see its own gamesetup message
            wall = Instantiate<GameObject>(PrefabWall);
            createWall(wall, points[i + 1], points[i + 2]);
            goal = Instantiate<GameObject>(PrefabGoal);
            createWall(goal, points[i], points[i + 1]);

            if (playerEnum.MoveNext())
            {
                var p = playerEnum.Current.Value;
                p.goalLeft = points[i];
                p.goalRight = points[i + 1];
                p.height = PADDLE_HEIGHT;
                p.length = PADDLE_LENGTH;
                p.playerLeft = p.goalLeft + (goal.transform.forward * PADDLE_DISTANCE);
                p.playerRight = p.goalRight + (goal.transform.forward * PADDLE_DISTANCE);
                p.paddle = CreatePaddle("Paddle for " + p.playerid, goal.transform.rotation, p.playerLeft, p.playerRight);
                goal.name = p.playerid;
            }
            else
            {
                Debug.LogError("mismatch between playfield size(" + (points.Length/2) + ") and playercount(" + this.playerInfo.Count + ")");
            }

        }

        this.ball.transform.position = new Vector3(0.0f, 0.0f, 1.0f);

        SendGameSetup(this.playerInfo);
        status = CoordinatorStatus.HOSTING_PLAYING;
    }


    #region utilities

    private GameObject CreatePaddle(string name, Quaternion rotation, Vector3 left, Vector3 right)
    {
        GameObject pad = Instantiate<GameObject>(PrefabPaddle);
        pad.name = name;
        pad.transform.rotation = rotation;
        PaddleController ctrl = pad.GetComponent<PaddleController>();
        ctrl.left = left;
        ctrl.right = right;

        return pad;
    }

    private GameObject createWall(GameObject newWall, Vector3 from, Vector3 to)
    {
        newWall.transform.position = (from + to) / 2f;
        newWall.transform.localScale = new Vector3(Vector3.Distance(from, to), 1f, 1f);
        newWall.transform.rotation = Quaternion.LookRotation(from - to) * Quaternion.AngleAxis(90f, newWall.transform.up);
        newWall.name = from.ToString() + " - " + to.ToString();
        //Quaternion.FromToRotation(from, to);
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

    public void OnDestroy()
    {
        Debug.Log("WebSockets discarded");
        if (pongWebSockets != null)
        {
            pongWebSockets.Dispose();
            pongWebSockets = null;
        }
    }

    #endregion
}
