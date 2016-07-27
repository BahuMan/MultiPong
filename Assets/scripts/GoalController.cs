using UnityEngine;

[RequireComponent(typeof(Collider))]
class GoalController : MonoBehaviour
{
    private string playerName;
    private PaddleController playerPaddle;

    void Start()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "ball") { Debug.Log("collision irrelevant for " + col.gameObject.name); return; }
        Debug.Log("player lost!");
    }

    public void setPlayer(string name, PaddleController paddle)
    {
        playerName = name;
        playerPaddle = paddle;
    }
}
