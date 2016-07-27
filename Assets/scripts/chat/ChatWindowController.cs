using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class ChatWindowController : MonoBehaviour {

    public int maxChatLines = 5;
    private System.Collections.Generic.Queue<string> chatLines;
    private bool dirty = false;
    private Text chatWindow;
    private PongWebSockets webSockets;

    // Use this for initialization
    void Awake()
    {
        chatLines = new Queue<string>(maxChatLines);
        chatWindow = GetComponent<Text>();
    }

    /*
     * This method was used by the following deprecated piece of code in the start() method:
     *         PongCoordinator gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<PongCoordinator>();
     *         webSockets = gameController.GetWebSockets();
     *         webSockets.wsMessageArrived += WebSockets_wsMessageArrived;
     */
    private bool WebSockets_wsMessageArrived(string msg, object JSONparsed)
    {
        if (!(JSONparsed is Hashtable)) return false;
        Hashtable args = JSONparsed as Hashtable;
        if (!"chat".Equals(args["type"])) return false;

        Debug.Log("I'm getting the following message: " + args["msg"]);
        addLine((string)args["from"], (string)args["msg"]);
        //chatWindow.text = "<B><color = \"#0000FF\">" + args["from"] + "</B></color> - " + args["msg"];
        return true;
    }

    // Update is called once per frame
    void Update () {
        if (!dirty) return;

        StringBuilder sb = new StringBuilder();
        lock (chatLines)
        {
            foreach (string l in chatLines)
            {
                sb.AppendLine(l);
            }
        }

        chatWindow.text = sb.ToString();
        dirty = false;
	}

    /**
     * This method can be called from the websockets listener (the old way)
     * or by any other component. In this new way, the PongController will parse and identify chat messages
     *
     * If I try to manipulate the text immediately upon receiving an event, I get errors that this can only be done in the main thread.
     * So this method only updates the internal state and sets a dirty flag.
     * The actual work (putting the text to screen) is done in the update() method
     */
    public void addLine(string from, string msg)
    {
        if ((from == null) || (msg == null)) return;
        string extraLine = "<color=\"blue\">" + from + "</color>: " + msg;

        lock (chatLines)
        {
            while (chatLines.Count >= maxChatLines)
            {
                chatLines.Dequeue();
            }
            chatLines.Enqueue(extraLine);

        }
        dirty = true; //force update to the UI.Text component
    }
}
