using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {
	private ConcurrentDictionary<string, int> _clientIP_to_playerID;
	private ConcurrentDictionary<int, byte[]> _playerID_to_b64WebcamImg;
	private ConcurrentDictionary<int, byte[]> _playerID_to_screenshot;

	// Playerの総数
	private int _clientNum = 0;

	// RegisterClientの排他制御用
	static object lockObj = new object ();

	// Use this for initialization
	void Start () {
		_clientIP_to_playerID = new ConcurrentDictionary<string, int> ();
		_playerID_to_b64WebcamImg = new ConcurrentDictionary<int, byte[]> ();
		_playerID_to_screenshot = new ConcurrentDictionary<int, byte[]> ();
	}

	// Clientを登録し、PlayerIDを割り当てる
	public void RegisterClient (string clientIP) {
		lock (lockObj) {
			if (!_clientIP_to_playerID.ContainsKey (clientIP)) {
				int newPlayerID = _clientNum++;
				_clientIP_to_playerID[clientIP] = newPlayerID;
				Debug.Log ("IP: " + clientIP + " を登録しました(playerID: " + newPlayerID + ")");
			} else {
				Debug.Log ("IP: " + clientIP + " のclientは既に存在します(playerID: " + _clientIP_to_playerID[clientIP] + ")");
			}
		}
	}

	/* setter */

	// 指定されたClientのplayerIDを更新
	private void SetPlayerID (string clientIP, int playerID) {

	}

	// 指定されたプレイヤーのBase64画像(ウェブカメラ画像)を更新
	public void SetBase64WebcamImage (int playerID, byte[] b64Img) {
		_playerID_to_b64WebcamImg[playerID] = b64Img;
	}

	// 指定された""Client""のBase64画像(ウェブカメラ画像)を更新
	// Receive Serverから呼び出す用
	public void SetBase64WebcamImage (string clientIP, byte[] b64Img) {
		int playerID = GetPlayerID (clientIP);
		SetBase64WebcamImage (playerID, b64Img);
	}

	// 指定されたプレイヤーのスクリーンショットを更新
	public void SetScreenshot (int playerID, Texture2D screenshot) {
		// PNG(byte配列)に変換してから更新
		SetScreenshot (playerID, screenshot.EncodeToPNG ());
	}

	// 指定されたプレイヤーのスクリーンショットを更新
	public void SetScreenshot (int playerID, byte[] b_screenshot) {
		_playerID_to_screenshot[playerID] = b_screenshot;
	}

	/* getter */

	// ClientのIPアドレスから、Clientに割り当てられているアバターのID(playerID)を取得
	public int GetPlayerID (string clientIP) {
		// ClientにまだplayerIDが割り当てられていない場合
		if (!_clientIP_to_playerID.ContainsKey (clientIP)) {
			// Clientに新たにplayerIDを割り当てる
			int newPlayerID = _clientNum;
			SetPlayerID (clientIP, newPlayerID);
			_clientNum++;
			return newPlayerID;
		} else {
			return _clientIP_to_playerID[clientIP];
		}
	}

	// 指定されたプレイヤーのBase64画像(ウェブカメラ画像)を取得
	public byte[] GetB64WebcamImage (int playerID) {
		if (!_playerID_to_b64WebcamImg.ContainsKey (playerID)) {
			return null;
		} else {
			return _playerID_to_b64WebcamImg[playerID];
		}
	}

	// 指定されたプレイヤーのスクリーンショットを取得
	public byte[] GetScreenshot (int playerID) {
		if (!_playerID_to_screenshot.ContainsKey (playerID)) {
			return null;
		} else {
			return _playerID_to_screenshot[playerID];
		}
	}

	// 指定された""Client""のスクリーンショットを取得
	public byte[] GetScreenshot (string clientIP) {
		int playerID = GetPlayerID (clientIP);
		return GetScreenshot (playerID);
	}
}