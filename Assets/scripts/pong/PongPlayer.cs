using System;
using UnityEngine;
using System.Collections;
using System.Text;

public class PongPlayer
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

    //this gameobject is not serialized into the JSON and should be recreated from above parameters:
    public GameObject paddle;

    public const string ARRAY_PLAYERS = "players";

    public const string FIELD_PLAYERID = "PlayerID";
    public const string FIELD_GOALLEFT = "GoalLeft";
    public const string FIELD_GOALRIGHT = "GoalRight";
    public const string FIELD_PLAYERLEFT = "PlayerLeft";
    public const string FIELD_PLAYERRIGHT = "PlayerRight";
    public const string FIELD_PADDLELENGTH = "PaddleLength";
    public const string FIELD_PADDLEHEIGHT = "PaddleHeight";
    public const string FIELD_POSITION = "PaddlePosition";
    public const string FIELD_VELOCITY = "PaddleVelocity";

    public PongPlayer(string id)
    {
        playerid = id;
    }

    public PongPlayer(Hashtable playerinfo)
    {
        fromJSON(playerinfo);
    }

    public PongPlayer fromJSON(Hashtable playerinfo)
    {
        playerid = (string)playerinfo[FIELD_PLAYERID];
        Hashtable vect = (Hashtable)playerinfo[FIELD_GOALLEFT];
        goalLeft = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_GOALRIGHT];
        goalRight = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_PLAYERLEFT];
        playerLeft = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_PLAYERRIGHT];
        playerRight = PongSerializer.toVector(vect);
        height = Convert.ToSingle((double)playerinfo[FIELD_PADDLEHEIGHT]);
        length = Convert.ToSingle((double)playerinfo[FIELD_PADDLELENGTH]);

        return this;
    }

    public string toJSON()
    {
        StringBuilder sb = new StringBuilder();
        return toJSON(sb).ToString();
    }

    public StringBuilder toJSON(StringBuilder sb)
    {
        sb.Append("{");
        sb.Append("\"").Append(FIELD_PLAYERID).Append("\": \"").Append(playerid).Append("\"");
        sb.Append("\"").Append(FIELD_GOALLEFT).Append("\":");
            PongSerializer.forVector(sb, goalLeft);
        sb.Append("\"").Append(FIELD_GOALRIGHT).Append("\":");
            PongSerializer.forVector(sb, goalRight);
        sb.Append("\"").Append(FIELD_PLAYERLEFT).Append("\":");
            PongSerializer.forVector(sb, playerLeft);
        sb.Append("\"").Append(FIELD_PLAYERRIGHT).Append("\":");
            PongSerializer.forVector(sb, playerRight);
        sb.Append("\"").Append(FIELD_PADDLELENGTH).Append("\": ").Append(length).Append(",");
        sb.Append("\"").Append(FIELD_PADDLEHEIGHT).Append("\": ").Append(height).Append("");
        sb.Append("}");

        return sb;
    }

    public PongPlayer UpdateFromUnity()
    {
        //@TODO - ugly hack; I should keep proper velocity and interpolate in frames where I don't have a network update
        this.velocity = paddle.GetComponent<PaddleController>().getCurPos() - this.position;
        this.position = paddle.GetComponent<PaddleController>().getCurPos();
        return this;
    }

    public PongPlayer UpdateToUnity()
    {
        paddle.GetComponent<PaddleController>().setCurPos(this.position);
        return this;
    }
}

