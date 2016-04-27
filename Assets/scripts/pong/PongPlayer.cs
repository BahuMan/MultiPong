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
        goalLeft = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_GOALRIGHT];
        goalRight = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_PLAYERLEFT];
        playerLeft = PongSerializer.toVector(vect);
        vect = (Hashtable)playerinfo[FIELD_PLAYERRIGHT];
        playerRight = PongSerializer.toVector(vect);
        height = Convert.ToSingle((double)playerinfo[FIELD_PADDLEHEIGHT]);
        length = Convert.ToSingle((double)playerinfo[FIELD_PADDLELENGTH]);
    }


    public string toJSON()
    {
        StringBuilder sb = new StringBuilder();
        return toJSON().ToString();
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
}

