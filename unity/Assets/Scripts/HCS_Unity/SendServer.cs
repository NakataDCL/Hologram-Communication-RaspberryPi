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

public class SendServer : MonoBehaviour {
	[SerializeField]
	public ClientManager cm;

	[SerializeField]
	public int port = 8081;
	private TcpListener _listener;
	private readonly List<TcpClient> _clients = new List<TcpClient> ();
	private Texture2D _texture = null;
	private byte[] _b_screenshot;

	// key: playerId, Value: Screenshot
	private Dictionary<int, Texture2D> _screenshot_dict = new Dictionary<int, Texture2D> ();

	// Use this for initialization
	void Start () {
		// IPアドレスを取得
		string ipv4 = IPManager.GetIP (ADDRESSFAM.IPv4);

		// Serverの待ち受けを開始
		StartServerListening ("127.0.0.1", port);
		//StartServerListening (ipv4, port);
		//StartServerListening ("192.168.10.33", port);

	}

	// Update is called once per frame
	void Update () {
		// スクリーンショットの更新を開始
		StartCoroutine ("LoadScreenshot");

		if (_texture != null) {
			// 画面のスクリーンショットをPNGにエンコード
			_b_screenshot = _texture.EncodeToPNG ();
		}
	}

	// ソケット接続準備、待機
	protected void StartServerListening (string host, int port) {
		Debug.Log ("ipAddress: " + host + ", port: " + port);
		var ip = IPAddress.Parse (host);
		_listener = new TcpListener (ip, port);

		_listener.Start ();
		_listener.BeginAcceptSocket (DoAcceptTcpClientCallback, _listener);
	}

	// クライアントからの接続時処理
	private void DoAcceptTcpClientCallback (IAsyncResult ar) {
		TcpListener listener = (TcpListener) ar.AsyncState;
		TcpClient client = listener.EndAcceptTcpClient (ar);
		_clients.Add (client);

		// ClientをClientManagerに登録する
		string endPoint = client.Client.RemoteEndPoint.ToString ();
		cm.RegisterClient (endPoint.Split (':') [0]);

		// 接続が確立したら次の人を受け付ける
		listener.BeginAcceptSocket (DoAcceptTcpClientCallback, listener);

		// bytestreamの読み込み
		NetworkStream stream = client.GetStream ();

		// クライアントとの接続が切れるまで繰り返す
		while (client.Connected) {
			if (_b_screenshot == null || _b_screenshot.Length <= 0) {
				continue;
			}

			// ここでカメラからスクショを取得する
			// Texture2D _b_screenshot = GetScreenshot(0);

			// バイト配列(画像データ)のサイズを通知(int: 4[byte])
			int data_size = _b_screenshot.Length;
			byte[] b_data_size = BitConverter.GetBytes (data_size);
			stream.Write (b_data_size, 0, 4);

			// バイト配列を送信
			stream.Write (_b_screenshot, 0, data_size);

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

		// スクリーンショットを取得
		_texture = new Texture2D (Screen.width, Screen.height);
		// textureに適用
		_texture.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		_texture.Apply ();
	}

	public void SendScreenshot () {
		return;
	}
}