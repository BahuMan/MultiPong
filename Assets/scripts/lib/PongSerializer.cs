using System;
using System.Text;
using UnityEngine;

class PongSerializer
{
    /**
        * takes name, position and rotation and converts it to a json message. Because in unity, the up-vector is "y", I'm taking (x,z) as 2D-coordinates.
        * So where a 2D system expects the "y" coordinate, I'm filling in the "z" coordinate.
        */
    public static string forGameObject(GameObject go)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ \"type\":\"move\", \"id\":\"").Append(go.name).Append("\", ");
        sb.Append("\"position\":{\"x\":").Append(go.transform.position.x*100f).Append(", \"y\":").Append(-go.transform.position.z*100f).Append("}, ");
        sb.Append("\"rotation\":{\"z\":").Append(go.transform.rotation.eulerAngles.y).Append("}");

        sb.Append("}");

        return sb.ToString();
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
}
