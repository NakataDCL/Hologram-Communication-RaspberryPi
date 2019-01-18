using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer : MonoBehaviour {

  private string _txtLogFilePath;
  private string _byteLogFilePath;
  private string _pngPath;

  private TcpListener _listener;
  private readonly List<TcpClient> _clients = new List<TcpClient> ();

  // Use this for initialization
  void Start () {
    // logファイルのパスを取得
    _txtLogFilePath = Application.dataPath + "/Log/log.txt";
    _byteLogFilePath = Application.dataPath + "/Log/log";

    // 画像ファイルの保存先のパスを取得
    _pngPath = Application.dataPath + "/Img/img.png";

    // Serverの待ち受けを開始
    StartServerListening ("127.0.0.1", 8080);
  }

  // Update is called once per frame
  void Update () {

  }

  // ソケット接続準備、待機
  protected void StartServerListening (string host, int port) {
    Debug.Log ("ipAddress: " + host + ", port: " + port);
    var ip = IPAddress.Parse (host);
    _listener = new TcpListener (ip, port);

    _listener.Start ();
    _listener.BeginAcceptSocket (DoAcceptTcpClientCallback, _listener);
  }

  // クライアントからの接続処理
  private void DoAcceptTcpClientCallback (IAsyncResult ar) {
    TcpListener listener = (TcpListener) ar.AsyncState;
    TcpClient client = listener.EndAcceptTcpClient (ar);
    _clients.Add (client);
    Debug.Log ("Connect: " + client.Client.RemoteEndPoint);

    // 接続が確立したら次の人を受け付ける
    listener.BeginAcceptSocket (DoAcceptTcpClientCallback, listener);

    // bytestreamの読み込み
    NetworkStream stream = client.GetStream ();

    // バイナリデータのサイズをチェック(int: 4[byte])
    byte[] b_data_size = new byte[4];
    stream.Read (b_data_size, 0, 4);
    int data_size = BitConverter.ToInt32 (b_data_size, 0);
    Debug.Log ("data size: " + data_size);

    // バイナリデータを受信(size: 65536[byte])
    // byte[] b_data = new byte[65536];
    byte[] b_data = new byte[data_size]; // data_size + 1 の可能性あり
    stream.Read (b_data, 0, data_size);

    // logファイルに受信したデータを出力する
    ByteSave (b_data);

    // 受信したデータをbase64デコード(画像データ)
    string base64Text = Encoding.UTF8.GetString (b_data);
    Debug.Log (base64Text);
    byte[] decodedByte = Convert.FromBase64String (base64Text);

    // 画像データを保存
    PngSave (decodedByte);

    // 受信したことを通知
    var resMsg = "successfully received";
    var body = Encoding.UTF8.GetBytes (resMsg);
    try {
      stream.Write (body, 0, body.Length);
    } catch {
      _clients.Remove (client);
    }

    // using (MemoryStream ms = new MemoryStream ()) {

    //   int numBytesRead;
    //   while ((numBytesRead = stream.Read (data, 0, data.Length)) > 0) {
    //     ms.Write (data, 0, numBytesRead);

    //   }
    //   str = Encoding.ASCII.GetString (ms.ToArray (), 0, (int) ms.Length);
    // }
    // Debug.Log ("receved: " + str);

    // // 接続した人とのネットワークストリームを取得
    // NetworkStream stream = client.GetStream ();
    // StreamReader reader = new StreamReader (stream, Encoding.UTF8);

    // // 接続が切れるまで送受信を繰り返す
    // while (client.Connected) {
    //   while (!reader.EndOfStream) {
    //     // 文字列を受け取る準備
    //     String message;
    //     StringBuilder messageBuilder = new StringBuilder ();

    //     Debug.Log ("Start reading message");
    //     var str = reader.ReadLine ();
    //     OnMessage (str);

    //     // 受信したことを通知
    //     var resMsg = "successfully received";
    //     var body = Encoding.UTF8.GetBytes (resMsg);
    //     try {
    //       stream.Write (body, 0, body.Length);
    //     } catch {
    //       _clients.Remove (client);
    //     }

    //     // クライアントの接続が切れたら
    //     if (client.Client.Poll (1000, SelectMode.SelectRead) && (client.Client.Available == 0)) {
    //       Debug.Log ("Disconnect: " + client.Client.RemoteEndPoint);
    //       client.Close ();
    //       _clients.Remove (client);
    //       break;
    //     }
    //   }
    // }
  }

  // メッセージ受信
  protected void OnMessage (string msg) {
    Debug.Log ("Client >> " + msg);
  }

  // 終了時処理
  protected void OnApplicationQuit () {
    foreach (var client in _clients) {
      client.Close ();
    }

    _listener.Stop ();
  }

  // 引数でStringを渡してやる
  public void TextSave (string txt) {
    FileInfo fi = new FileInfo (_txtLogFilePath);

    StreamWriter sw;
    sw = fi.AppendText ();
    sw.WriteLine (txt);
    sw.Flush ();
    sw.Close ();
  }

  public void ByteSave (byte[] b) {
    Debug.Log ("save log: " + b.Length + "[byte]");

    FileInfo fi = new FileInfo (_byteLogFilePath);

    FileStream fs = fi.Create ();
    BinaryWriter writer = new BinaryWriter (fs, Encoding.UTF8);

    writer.Write (b);
    writer.Flush ();
    writer.Close ();
  }

  public void PngSave (byte[] b) {
    Debug.Log ("save image");

    FileInfo fi = new FileInfo (_pngPath);

    FileStream fs = fi.Create ();
    BinaryWriter writer = new BinaryWriter (fs, Encoding.UTF8);

    writer.Write (b);
    writer.Flush ();
    writer.Close ();
  }
}