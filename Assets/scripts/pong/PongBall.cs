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

    public GameObject actualBall;

    public const string FIELD_BALLID = "BallID";
    public const string FIELD_POSITION = "Position";
    public const string FIELD_VELOCITY = "Velocity";
    public const string FIELD_DIAMETER = "Diameter";

    public PongBall(GameObject UnityBall)
    {
        this.actualBall = UnityBall;
        this.ballid = actualBall.name;
        this.position = actualBall.transform.position;
        this.velocity = actualBall.GetComponent<Rigidbody>().velocity;
    }

    public PongBall(GameObject UnityBall,  Hashtable BallInfo)
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

    public PongBall UpdateFromUnity()
    {
        this.position = actualBall.transform.position;
        this.velocity = actualBall.GetComponent<Rigidbody>().velocity;
        return this;
    }

}
