using UnityEngine;
using UnityEngine.UI;

public class ChatInputController : MonoBehaviour {

    public string playerName = "Bart Huylebroeck";
    public ChatWindowController chatWindow; //should be coupled in the scene, by filling this variable with the correct component
    private InputField chatInput;
    private PongCoordinator pongCoordinator;

    //public delegate bool ChatMessageHandler(string msg);
    //public event ChatMessageHandler SendChatMessage;


    /**
     * start will locate the inputfield and attach a listener.
     * It will also locate the websockets client and listen for messages of the "chat" format.
     * chat messages should be pretty printed and added to the chat output window
     */
    void Start () {
        //from the input field, listen for submissions (check for the enter)
        chatInput = GetComponent<InputField>();
        chatInput.onEndEdit.AddListener(submitted);

        GameObject GameController = GameObject.FindGameObjectWithTag("GameController");
        pongCoordinator = GameController.GetComponent<PongCoordinator>();

	}



    // Update is called once per frame
    void Update () {
	}

    /**
     * this method should be provided as an argument for InputField.AddListener()
     */
    public void submitted(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            chatWindow.addLine("/me: ", text);
            pongCoordinator.SendChatMessage(text);
        }
        else
        {
            Debug.Log("ChatController.submitted was called, but no enter pressed -- not sending message");
        }
    }
}
