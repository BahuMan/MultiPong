using UnityEngine;

[RequireComponent(typeof(Collider))]
class GoalController : MonoBehaviour
{
    private PongCoordinator theGame;
    private GameObject ball;
    private string playerName;
    private PaddleController playerPaddle;

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("ball");
        theGame = GameObject.FindGameObjectWithTag("GameController").GetComponent<PongCoordinator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject != ball) { Debug.Log("collision irrelevant for " + col.gameObject.name); return; }
        Debug.Log("player lost!");
    }

    public void setPlayer(string name, PaddleController paddle)
    {
        playerName = name;
        playerPaddle = paddle;
    }
}
