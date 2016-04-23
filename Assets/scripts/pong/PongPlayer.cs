using System;
using UnityEngine;
using System.Collections;
using System.Text;

public class PongPlayer
{

    public string playerid;
    public Vector3 goalLeft;
    public Vector3 goalRight;
    public float length;
    public float height;
    public GameObject paddle;

    public const string ARRAY_PLAYERS = "players";

    public const string FIELD_PLAYERID = "PlayerID";
    public const string FIELD_GOALLEFT = "GoalLeft";
    public const string FIELD_GOALRIGHT = "GoalRight";
    public const string FIELD_PADDLELENGTH = "PaddleLength";
    public const string FIELD_PADDLEHEIGHT = "PaddleHeight";

    public PongPlayer(string id)
    {
        playerid = id;
    }

    public static PongPlayer fromJSON(Hashtable playerinfo)
    {
        return new PongPlayer(playerinfo);
    }

    public PongPlayer(Hashtable playerinfo)
    {
        playerid = (string) playerinfo[FIELD_PLAYERID];
        Hashtable vect = (Hashtable)playerinfo[FIELD_GOALLEFT];
        //goalLeft = new Vector3(Convert.ToSingle((double) vect["x"]), Convert.ToSingle((double) vect["y"]));
        goalLeft = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_GOALRIGHT];
        //goalRight = new Vector3(Convert.ToSingle((double) vect["x"]), Convert.ToSingle((double)vect["y"]));
        goalRight = PongSerializer.toVector(vect);
        height = Convert.ToSingle((double)playerinfo[FIELD_PADDLEHEIGHT]);
        length = Convert.ToSingle((double)playerinfo[FIELD_PADDLELENGTH]);
    }

    public string toJSON()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.Append("\"").Append(FIELD_PLAYERID).Append("\": \"").Append(playerid).Append("\"");
        sb.Append("\"").Append(FIELD_GOALLEFT).Append("\":");
            PongSerializer.forVector(sb, goalLeft);
            //sb.Append("{\"x\": ").Append(goalLeft.x).Append(",");
            //sb.Append("\"y\": ").Append(goalLeft.y).Append("}");
        sb.Append("\"").Append(FIELD_GOALRIGHT).Append("\":");
            PongSerializer.forVector(sb, goalRight);
            //sb.Append("{\"x\": ").Append(goalRight.x).Append(",");
            //sb.Append("\"y\": ").Append(goalRight.y).Append("}");
        sb.Append("\"").Append(FIELD_PADDLELENGTH).Append("\": ").Append(length).Append(",");
        sb.Append("\"").Append(FIELD_PADDLEHEIGHT).Append("\": ").Append(height).Append("");
        sb.Append("}");

        return sb.ToString();
    }
}

