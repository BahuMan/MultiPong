using System;
using UnityEngine;
using System.Collections;
using System.Text;

public class PongBall
{
    public string ballid;
    public Vector2 position;
    public Vector2 velocity;
    public float diameter;

    public const string FIELD_BALLID = "BallID";
    public const string FIELD_POSITION = "Position";
    public const string FIELD_VELOCITY = "Velocity";
    public const string FIELD_DIAMETER = "Diameter";

    public PongBall()
    {

    }

    public PongBall(Hashtable BallInfo)
    {
        ballid = (string)BallInfo[FIELD_BALLID];
        Hashtable vect = (Hashtable)BallInfo[FIELD_POSITION];
        position = new Vector2(Convert.ToSingle((double)vect["x"]), Convert.ToSingle((double)vect["y"]));
        vect = (Hashtable)BallInfo[FIELD_VELOCITY];
        velocity = new Vector2(Convert.ToSingle((double)vect["x"]), Convert.ToSingle((double)vect["y"]));
        diameter = Convert.ToSingle((double)BallInfo[FIELD_DIAMETER]);
    }

    public string toJSON()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.Append("\"").Append(FIELD_BALLID).Append("\": \"").Append(ballid).Append("\"");
        sb.Append("\"").Append(FIELD_POSITION).Append("\": {");
        sb.Append("\"x\": ").Append(position.x).Append(",");
        sb.Append("\"y\": ").Append(position.y).Append("}");
        sb.Append("\"").Append(FIELD_VELOCITY).Append("\": {");
        sb.Append("\"x\": ").Append(velocity.x).Append(",");
        sb.Append("\"y\": ").Append(velocity.y).Append("}");
        sb.Append("\"").Append(FIELD_DIAMETER).Append("\": ").Append(diameter).Append(",");
        sb.Append("}");

        return sb.ToString();
    }

}
