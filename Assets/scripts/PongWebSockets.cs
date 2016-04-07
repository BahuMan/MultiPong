using WebSocketSharp;
using UnityEngine;
using System;

public class PongWebSockets : IDisposable {

    public PongWebSockets(): this("ws://localhost:8080")
    {
    }

    public PongWebSockets(string WebSocketServerAddress)
    {
        //nf = new Notifier();
        ws = new WebSocket(WebSocketServerAddress);

        ws.OnMessage += Ws_OnMessage;
        ws.OnOpen += Ws_OnOpen;
        ws.OnClose += Ws_OnClose;
        ws.OnError += Ws_OnError;
        ws.Connect();
    }

    public void Reconnect(string WebSocketServerAddress)
    {
        ws.Close();
        ws.OnMessage -= Ws_OnMessage;
        ws.OnOpen -= Ws_OnOpen;
        ws.OnClose -= Ws_OnClose;
        ws.OnError -= Ws_OnError;

        ws = new WebSocket(WebSocketServerAddress);
        ws.OnMessage += Ws_OnMessage;
        ws.OnOpen += Ws_OnOpen;
        ws.OnClose += Ws_OnClose;
        ws.OnError += Ws_OnError;
        ws.Connect();
    }

    public bool wsMessage(string msg)
    {
        if (!open) return false;
        ws.Send(msg);
        return true;
    }

    public delegate bool wsMessageArrivedHandler(string msg, object JSONParsed);
    public event wsMessageArrivedHandler wsMessageArrived;

    //private Notifier nf;
    private WebSocket ws;
    private bool open;
    private float lastTime = 0.0f;

    private void Ws_OnOpen(object sender, EventArgs e)
    {
        open = true;
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        open = false;
    }

    private void Ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError(e.Message);
        Debug.LogError(e.Exception);
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        if (wsMessageArrived != null)
        {
            object parsed = Procurios.Public.JSON.JsonDecode(e.Data);
            bool consumed = false;
            try
            {
                if (parsed == null) Debug.LogError("the object returned by JSON.JsonDecode is null -- parsing error?");
                consumed |= wsMessageArrived(e.Data, parsed);
                if (!consumed)
                {
                    Debug.Log("unconsumed message at WebSockets: " + e.Data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        else
        {
            Debug.Log("WebSocketTest received message but no one is listening: " + e.Data);
        }
    }

    public void Dispose()
    {
        if (ws == null) return;

        ws.OnMessage -= Ws_OnMessage;
        Debug.Log("WebSockets were destroyed");
        ((IDisposable)ws).Dispose();
    }


}
