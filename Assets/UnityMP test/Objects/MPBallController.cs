using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
class MPBallController : NetworkBehaviour
{
    public float speed;
    private Rigidbody ball;
    private MPPongCoordinator pongCoordinator;

    override public void OnStartClient()
    {
        ball = GetComponent<Rigidbody>();
        ball.velocity = new Vector3(speed, 0f, speed);
        pongCoordinator = GameObject.FindGameObjectWithTag("GameController").GetComponent<MPPongCoordinator>();
    }

    [Server]
    public void Reset(Vector3 pos, Quaternion rotation)
    {
        transform.position = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        ball.velocity = transform.forward * speed;
    }

    void OnCollisionExit(Collision coll)
    {
        //Debug.Log("boing - resetting speed");
        ball.velocity = ball.velocity.normalized * speed;
    }

    void OnTriggerEnter(Collider coll)
    {
        MPGoalController goal = coll.gameObject.GetComponent<MPGoalController>();
        if (goal != null)
        {
            Debug.Log("ball - hit goal with " + coll.gameObject.name);
            pongCoordinator.PlayerGoalHit(goal.getPlayer());
        }
    }

}
