# Hologram-Communication-RaspberryPi
Hologram Communication System における、Raspberry Piに関連する以下の機能の実装を行う
・Raspberry PiのWebカメラ画像取得部分
・Unityサーバとのネットワーク通信部分(サーバクライアント両方)
・Raspberry Piの画像表示部分(UnityのGameウィンドウのスクリーンショットのストリーミング再生)

通信部分ではTCP/IPソケット通信を利用し、ServerとClient間で以下のようにデータをやりとりする
・Client -> Server: ClientがWebカメラから取得した画像を、バイナリ形式でUnity上のServerに送信
・Server -> Client: Unity上のServerで生成された画像を、バイナリ形式でClientに送信

## 成果物
・[Client(Python)](https://github.com/NakataDCL/Hologram-Communication-RaspberryPi/tree/master/python/HCS_Python)
Raspberry Pi上で動作する。

・[Server(C#)](https://github.com/NakataDCL/Hologram-Communication-RaspberryPi/tree/master/unity/Assets/Scripts/HCS_Unity)

以上の2つを[本体](https://github.com/DCL-Waseda/Hologram-Communication)と統合した
