using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{
  private TcpListener _listener;
  private readonly List<TcpClient> _clients = new List<TcpClient>();

  // Use this for initialization
  void Start()
  {
    StartServerListening("127.0.0.1", 8080);
  }

  // Update is called once per frame
  void Update()
  {

  }

  // ソケット接続準備、待機
  protected void StartServerListening(string host, int port)
  {
    Debug.Log("ipAddress: " + host + ", port: " + port);
    var ip = IPAddress.Parse(host);
    _listener = new TcpListener(ip, port);

    _listener.Start();
    _listener.BeginAcceptSocket(DoAcceptTcpClientCallback, _listener);
  }

  // クライアントからの接続処理
  private void DoAcceptTcpClientCallback(IAsyncResult ar)
  {
    var listener = (TcpListener)ar.AsyncState;
    var client = listener.EndAcceptTcpClient(ar);
    _clients.Add(client);
    Debug.Log("Connect: " + client.Client.RemoteEndPoint);

    // 接続が確立したら次の人を受け付ける
    listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);

    // 接続した人とのネットワークストリームを取得
    var stream = client.GetStream();
    var reader = new StreamReader(stream, Encoding.UTF8);

    // 接続が切れるまで送受信を繰り返す
    while (client.Connected)
    {
      while (!reader.EndOfStream)
      {
        // 一行分の文字列を受け取る
        var str = reader.ReadLine();
        OnMessage(str);

        // 文字列を送り返す
        var resMsg = "re:" + str;
        var body = Encoding.UTF8.GetBytes(resMsg);
        try
        {
          stream.Write(body, 0, body.Length);
        }
        catch
        {
          _clients.Remove(client);
        }

        // クライアントの接続が切れたら
        if (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0))
        {
          Debug.Log("Disconnect: " + client.Client.RemoteEndPoint);
          client.Close();
          _clients.Remove(client);
          break;
        }
      }
    }
  }

  // メッセージ受信
  protected void OnMessage(string msg)
  {
    Debug.Log("Client >> " + msg);
  }

  // 終了時処理
  protected void OnApplicationQuit()
  {
    foreach (var client in _clients)
    {
      client.Close();
    }

    _listener.Stop();
  }
}
