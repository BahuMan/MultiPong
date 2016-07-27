using UnityEngine;
using UnityEngine.UI;

public class MPMenuController : MonoBehaviour {

    public GameObject totalMenu; //use this to make everything disappear
    public InputField playerNameInput;
    public Text joinOrHostDropDown;
    public Text addressInput; //for joining games, this input will contain the address. For local hosting, this should display the local IP address
    public float menuspeed = 0.5f;
    public bool hosting = false;

    void Start()
    {
        menuVisible = true;
        if (totalMenu != null) totalMenu.SetActive(true);

        if (playerNameInput != null) playerNameInput.onEndEdit.AddListener(PlayerNameChanged);
        if (addressInput != null) addressInput.text = "ws://" + Network.player.ipAddress + ":8080";


    }

    public bool menuVisible { get; private set; }
    /*
     * called when the hamburger menu-icon in the top left is clicked
     */
    public void ToggleMenu()
    {
        menuVisible = !menuVisible;
        if (totalMenu != null) totalMenu.SetActive(menuVisible);
        Debug.Log("toggle: activated currently " + menuVisible);
    }

    private string previousAddress;
    /*
     * called when the dropdown has been used to choose between hosting your own server or joining an existing one
     */
    public void DropDownValueChanged()
    {
        if (joinOrHostDropDown.text.Equals("Join Remote Server"))
        {
            Debug.Log("join game selected");
            hosting = false;
            //if (previousAddress != null)
            //{
            //    addressInput.text = previousAddress;
            //}
        }
        else
        {
            Debug.Log("Local hosting selected");
            hosting = true;
            //previousAddress = addressInput.text;
            //addressInput.text = "local IP = " + Network.player.ipAddress;
        }
    }

    public void PlayerNameChanged(string newPlayerName)
    {
        Debug.Log("Local player is now known as " + newPlayerName);
    }

}
