using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PongCoordinator : MonoBehaviour {

    #region variables

    public string localPlayerName = "Bart Huylebroeck";
    public string ServerURL = "ws://localhost:8080";
    public int numPlayers = 5;
    public float radius = 20f;

    public ChatWindowController chatWindowController;
    public GameObject PrefabWall;
    public GameObject PrefabGoal;
    public GameObject PrefabPadle;
    public Rigidbody ball;

    public const string TYPE_TYPE = "type";
    public const string TYPE_JOIN = "PlayerJoined";
    public const string TYPE_CHAT = "chat";

    public enum CoordinatorStatus { INIT, WAITING_FOR_PLAYERS, JOINING, JOINED_STARTING, JOINED_PLAYING };

    private CoordinatorStatus status = CoordinatorStatus.INIT;
    private PongWebSockets pongWebSockets;

    private Dictionary<string, PongPlayer> playerInfo;
    #endregion

    /**
     * called by any GUI component to start hosting a game.
     * We will start listening for "playerjoined" requests and start a game.
     **/
    public void HostGame(string url)
    {
        Debug.Log("Coordinator starting game");
        status = CoordinatorStatus.WAITING_FOR_PLAYERS;
        pongWebSockets.Reconnect(url);
    }

    /**
     * called by any GUI component to join a remote game.
     * The remote host should tell us what game we're in and where our paddle is in world coordinates
     **/
    public void JoinGame(string url)
    {
        Debug.Log("Coordinator joining game");
        status = CoordinatorStatus.JOINED_STARTING;
        pongWebSockets.Reconnect(url);
        SendPlayerJoin(localPlayerName);
    }

    #region websockets

    private bool PongWebSockets_wsMessageArrived(string msg, object JSONParsed)
    {
        checkJSON(msg, JSONParsed);

        Hashtable jsonhash = (Hashtable)JSONParsed;
        string msgType = (string) jsonhash[TYPE_TYPE];
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
        return false;
    }

    private void ReceivePlayerJoined(Hashtable playa)
    {
        Debug.Log("player joined: " + playa[PongPlayer.FIELD_PLAYERID]);
    }

    private void SendPlayerJoin(string playerid)
    {
        pongWebSockets.wsMessage(PongSerializer.forPlayerJoin(playerid));
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

        Vector3[] points = CreateEllipse(radius, radius, new Vector3(0f, 0f, 0f), numPlayers*2);
        GameObject newObj;

        for (int i=0; i<points.Length-2; i+=2)
        {
            newObj = Instantiate<GameObject>(PrefabGoal);
            createWall(newObj, points[i], points[i+1]);
            newObj = Instantiate<GameObject>(PrefabWall);
            createWall(newObj, points[i+1], points[i + 2]);
        }

        pongWebSockets = new PongWebSockets();

    }



    #region utilities

    private GameObject createWall(GameObject newWall, Vector3 from, Vector3 to)
    {

        newWall.transform.position = (from + to) / 2f;
        newWall.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(from, to));
        newWall.transform.rotation = Quaternion.LookRotation(from - to);
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
