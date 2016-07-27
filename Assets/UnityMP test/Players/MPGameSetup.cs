using UnityEngine;

public struct MPGameSetup
{
    public string playerid;
    public Vector3 goalLeft;
    public Vector3 goalRight;
    public Vector3 playerLeft;
    public Vector3 playerRight;
    public float length;
    public float height;

    public float position; //a number between 0 and 1, with 0 representing left-most position
    public float velocity; //units per second the paddle is moving (negative means move to the left)

}
