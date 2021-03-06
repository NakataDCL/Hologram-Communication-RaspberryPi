﻿using System;
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
	private string _byteLogFilePath;
	private TcpListener _listener;
	private readonly List<TcpClient> _clients = new List<TcpClient> ();
	private Texture2D _texture = null;

	private byte[] _b_screenshot;

	[SerializeField]
	public RawImage raw;

	// Use this for initialization
	void Start () {
		// logファイルのパスを取得
		_byteLogFilePath = Application.dataPath + "/Log/log";

		// Serverの待ち受けを開始
		StartServerListening ("127.0.0.1", 8081);
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
			if (_b_screenshot == null || _b_screenshot.Length <= 0) {
				Debug.Log ("_b_screenshot: " + _b_screenshot.Length);
				continue;
			}

			// base64に変換
			// string str = Convert.ToBase64String (_b_screenshot);
			// byte[] bytes = System.Text.Encoding.UTF8.GetBytes (str);
			byte[] bytes = _b_screenshot;

			// バイナリのサイズを通知(int: 4[byte])
			int len = bytes.Length;
			byte[] b_len = BitConverter.GetBytes (len);
			stream.Write (b_len, 0, 4);

			// バイナリデータを送信
			stream.Write (bytes, 0, len);
			Debug.Log ("send: " + len + "[bytes]");

			// クライアントからのレスポンスを受信

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

		_texture = new Texture2D (Screen.width, Screen.height);
		// Debug.Log (Screen.width / 2 + ", " + Screen.height / 2);

		_texture.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		_texture.Apply ();
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
}