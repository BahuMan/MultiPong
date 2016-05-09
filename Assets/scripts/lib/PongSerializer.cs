using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PongSerializer
{
    public const float SCALE = 100f;
    /**
        * takes name, position and rotation and converts it to a json message. Because in unity, the up-vector is "y", I'm taking (x,z) as 2D-coordinates.
        * So where a 2D system expects the "y" coordinate, I'm filling in the "z" coordinate.
        */
    public static string forGameObject(GameObject go)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ \"type\":\"move\", \"id\":\"").Append(go.name).Append("\", ");
        //sb.Append("\"position\":{\"x\":").Append(go.transform.position.x*scale).Append(", \"y\":").Append(-go.transform.position.z*scale).Append("}, ");
        sb.Append("\"position\":"); forVector(sb, go.transform.position).Append(", ");
        sb.Append("\"rotation\":{\"z\":").Append(go.transform.rotation.eulerAngles.y).Append("}");

        sb.Append("\"position\":{\"x\":").Append(go.transform.position.x * SCALE).Append(", \"y\":").Append(-go.transform.position.z * SCALE).Append("}, ");

        sb.Append("}");

        return sb.ToString();
    }

    public static string forVector(Vector3 v)
    {
        StringBuilder sb = new StringBuilder();
        return forVector(sb, v).ToString();
    }

    public static StringBuilder forVector(StringBuilder sb, Vector3 v)
    {
        return sb.Append("{\"x\":").Append(v.x * SCALE).Append(", \"y\":").Append(v.z * SCALE).Append("}");
    }

    public static Vector3 toVector(Hashtable vect)
    {
        return new Vector3(Convert.ToSingle((double)vect["x"] / SCALE), 0.0f, Convert.ToSingle((double)vect["y"] / SCALE));
    }

    /**
        * generates a JSON string in the form specified by https://github.com/Squarific/Game-Of-Everything/wiki/protocol-chat
        *
        * @TODO: URLEncode
        */
    public static string forChatMessage(String from, String msg)
    {
        return "{\"type\":\"" + PongCoordinator.TYPE_CHAT + "\", \"from\":\"" + from + "\", \"msg\":\"" + msg + "\"}";
    }

    public static string forPlayerJoin(string name)
    {
        return "{\"type\":\"" + PongCoordinator.TYPE_JOIN + "\", \"" + PongPlayer.FIELD_PLAYERID + "\":\"" + name + "\"}";
    }

    public static string forGameSetup(Dictionary<string, PongPlayer> playerInfo)
    {
        StringBuilder sb = new StringBuilder("{\"type\":\"").Append(PongCoordinator.TYPE_SETUP_GAME).Append("\", ");
        sb.Append("\"").Append(PongPlayer.ARRAY_PLAYERS).Append("\":[");
        foreach (PongPlayer p in playerInfo.Values)
        {
            sb.AppendLine();
            sb = p.toJSON(sb);
            sb.Append(", ");
        }
        sb.Length -= 2; //remove the last comma
        sb.Append("]}");
        return sb.ToString();
    }

    public static string forBallMove(PongBall ball)
    {
        StringBuilder sb = new StringBuilder("{\"type\":\"").Append(PongCoordinator.TYPE_BALL_MOVE).Append("\", ");
        sb.Append("\"").Append(PongBall.FIELD_BALLID).Append("\": \"").Append(ball.ballid).Append("\"");
        sb.Append("\"").Append(PongBall.FIELD_POSITION).Append("\":");
        PongSerializer.forVector(sb, ball.position);
        sb.Append("\"").Append(PongBall.FIELD_VELOCITY).Append("\":");
        PongSerializer.forVector(sb, ball.velocity);
        sb.Append("\"").Append(PongBall.FIELD_DIAMETER).Append("\": ").Append(ball.diameter).Append(",");
        sb.Append("}");

        return sb.ToString();
    }

    public static PongBall fromBallMove(PongBall pongBall, Hashtable BallInfo)
    {
        pongBall.ballid = (string)BallInfo[PongBall.FIELD_BALLID];

        Hashtable vect = (Hashtable)BallInfo[PongBall.FIELD_POSITION];
        pongBall.position = PongSerializer.toVector(vect);

        vect = (Hashtable)BallInfo[PongBall.FIELD_VELOCITY];
        pongBall.velocity = PongSerializer.toVector(vect);

        pongBall.diameter = Convert.ToSingle((double)BallInfo[PongBall.FIELD_DIAMETER]);

        return pongBall;
    }

    public static string forPlayerMove(PongPlayer player)
    {
        StringBuilder sb = new StringBuilder("{\"type\":\"").Append(PongCoordinator.TYPE_PLAYER_MOVE).Append("\", ");
        sb.Append("\"").Append(PongPlayer.FIELD_PLAYERID).Append("\": \"").Append(player.playerid).Append("\", ");
        sb.Append("\"").Append(PongPlayer.FIELD_POSITION).Append("\":").Append(player.position).Append(", ");
        sb.Append("\"").Append(PongPlayer.FIELD_VELOCITY).Append("\":").Append(player.velocity);
        sb.Append("}");

        return sb.ToString();
    }

    public static PongPlayer fromPlayerMove(PongPlayer player, Hashtable playerMove)
    {
        player.position = Convert.ToSingle((double)playerMove[PongPlayer.FIELD_POSITION]);
        player.velocity = Convert.ToSingle((double)playerMove[PongPlayer.FIELD_VELOCITY]);
        return player;
    }
}
