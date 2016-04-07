using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour {

    public float speed;
    private Rigidbody ball;

	// Use this for initialization
	void Start () {
        ball = GetComponent<Rigidbody>();
        ball.velocity = new Vector3(speed, 0f, speed);

    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnCollisionExit(Collision coll)
    {
        //Debug.Log("boing - resetting speed");
        ball.velocity = ball.velocity.normalized * speed;
    }
}
