using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ReceiveServer : MonoBehaviour {
	[SerializeField]
	public int port = 8080;
	private TcpListener _listener;
	private readonly List<TcpClient> _clients = new List<TcpClient> ();

	private string _pngPath;
	private int _count = 0;

	// Use this for initialization
	void Start () {
		// 画像ファイルの保存先のパスを取得
		_pngPath = Application.dataPath + "/Img/img";

		// IPアドレスを取得
		string ipv4 = IPManager.GetIP (ADDRESSFAM.IPv4);

		// Serverの待ち受けを開始
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
		Debug.Log ("Connect: " + client.Client.RemoteEndPoint);

		// 接続が確立したら次の人を受け付ける
		listener.BeginAcceptSocket (DoAcceptTcpClientCallback, listener);

		// bytestreamの読み込み
		NetworkStream stream = client.GetStream ();

		// クライアントとの接続が切れるまで繰り返す
		while (client.Connected) {
			// 次に送られてくるバイト配列のサイズを受信(int: 4[byte])
			byte[] b_data_size = new byte[4];
			stream.Read (b_data_size, 0, 4);
			int data_size = BitConverter.ToInt32 (b_data_size, 0);

			// バイナリデータを受信
			int rest = data_size;
			byte[] b_data_sum = new byte[data_size];
			int read_size_sum = 0;
			while (rest > 0) {
				byte[] b_data = new byte[1024];
				int read_size = 0;
				if (rest >= 1024) {
					read_size = stream.Read (b_data, 0, 1024);
				} else {
					read_size = stream.Read (b_data, 0, rest);
				}
				Array.Copy (b_data, 0, b_data_sum, read_size_sum, read_size);
				rest -= read_size;
				read_size_sum += read_size;
			}

			// この段階のデータをAPIに投げる

			// rawデータをbase64エンコードするように修正
			// string str = Convert.ToBase64String (b_data_sum);

			// 最初の10枚を保存
			if (_count < 10) {
				PngSave (b_data_sum);
				_count++;
			}

			// クライアントの接続が切れた場合
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

	public void PngSave (byte[] b) {
		Debug.Log ("save image");

		FileInfo fi = new FileInfo (_pngPath + _count + ".png");

		FileStream fs = fi.Create ();
		BinaryWriter writer = new BinaryWriter (fs, Encoding.UTF8);

		writer.Write (b);
		writer.Flush ();
		writer.Close ();
	}
}