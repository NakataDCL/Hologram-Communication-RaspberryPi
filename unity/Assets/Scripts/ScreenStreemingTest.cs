using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScreenStreemingTest : MonoBehaviour {

	private TcpListener _listener;
	private readonly List<TcpClient> _clients = new List<TcpClient> ();

	private Texture2D _texture;

	[SerializeField]
	public RawImage raw;

	// Use this for initialization
	void Start () {
		// Serverの待ち受けを開始
		StartServerListening ("127.0.0.1", 8081);

		// rawimageの設定
		raw.color = Color.white;
		raw.texture = _texture;
	}

	// Update is called once per frame
	void Update () {
		// スクリーンショットの更新を開始
		StartCoroutine ("LoadScreenshot");
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

		// クライアントとの接続が切れるまで繰り返す
		while (client.Connected) {
			// 画面のスクリーンショットを取得

			// base64に変換

			// バイナリのサイズを通知(int: 4[byte])

			// バイナリデータを送信

			// クライアントの接続が切れたら
			if (client.Client.Poll (1000, SelectMode.SelectRead) && (client.Client.Available == 0)) {
				Debug.Log ("Disconnect: " + client.Client.RemoteEndPoint);
				client.Close ();
				_clients.Remove (client);
				break;
			}
		}
	}

	// 終了時処理
	protected void OnApplicationQuit () {
		foreach (var client in _clients) {
			client.Close ();
		}

		_listener.Stop ();
	}

	IEnumerator LoadScreenshot () {
		yield return new WaitForEndOfFrame ();

		_texture = new Texture2D (Screen.width / 2, Screen.height / 2);
		Debug.Log (Screen.width / 2 + ", " + Screen.height / 2);

		_texture.ReadPixels (new Rect (0, 0, Screen.width / 2, Screen.height / 2), 0, 0);
		_texture.Apply ();

		raw.texture = _texture;
	}
}