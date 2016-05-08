using System;
using UnityEngine;
using System.Collections;
using System.Text;

public class PongBall
{
    public string ballid;
    public Vector3 position;
    public Vector3 velocity;
    public float diameter;

    public Rigidbody actualBall;

    public const string FIELD_BALLID = "BallID";
    public const string FIELD_POSITION = "Position";
    public const string FIELD_VELOCITY = "Velocity";
    public const string FIELD_DIAMETER = "Diameter";

    /**
     * this constructor should only be used as a dummy for the unittests
     */
    public PongBall()
    {
    }

    public PongBall(Rigidbody UnityBall)
    {
        this.actualBall = UnityBall;
        this.ballid = actualBall.name;
        this.position = actualBall.position;
        this.velocity = actualBall.velocity;
    }

    public PongBall(Rigidbody UnityBall,  Hashtable BallInfo)
    {
        this.actualBall = UnityBall;
        ballid = (string)BallInfo[FIELD_BALLID];
        this.actualBall.name = ballid;

        Hashtable vect = (Hashtable)BallInfo[FIELD_POSITION];
        position = PongSerializer.toVector(vect);
        actualBall.transform.position = position;

        vect = (Hashtable)BallInfo[FIELD_VELOCITY];
        velocity = PongSerializer.toVector(vect);
        actualBall.GetComponent<Rigidbody>().velocity = velocity;

        diameter = Convert.ToSingle((double)BallInfo[FIELD_DIAMETER]);

    }

    /*
     * This method should be called inside the Unity Update() or FixedUpdate()
     */
    public PongBall UpdateFromUnity()
    {
        this.position = actualBall.transform.position;
        this.velocity = actualBall.GetComponent<Rigidbody>().velocity;
        return this;
    }

    /*
     * This method should be called inside the Unity Update() or FixedUpdate()
     */
    public PongBall UpdateToUnity()
    {
        actualBall.MovePosition(position);
        actualBall.velocity = velocity;
        return this;
    }
}
