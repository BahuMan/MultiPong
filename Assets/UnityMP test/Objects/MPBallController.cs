using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
class MPBallController : NetworkBehaviour
{
    public float speed;
    private Rigidbody ball;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
        ball.velocity = new Vector3(speed, 0f, speed);
    }

    void OnCollisionExit(Collision coll)
    {
        //Debug.Log("boing - resetting speed");
        ball.velocity = ball.velocity.normalized * speed;
    }


}
