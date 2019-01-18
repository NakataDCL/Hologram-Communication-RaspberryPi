# 画像ファイルをbase64エンコードしてTCP/IPで送信する #

import socket
import base64

# 接続先ホストの情報
host = "localhost"
port = 8080

# 送信する画像のファイルパス
img_path = "../img/chunchun_evening.png"

# 画像ファイルをバイナリモードで開き、base64にエンコード
file = open(img_path, "rb")
data = file.read()
file.close()

byte_arr = base64.b64encode(data)
print(str(len(byte_arr)) + "[byte]")
# print(byte_arr)

# clientオブジェクトの作成
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
try:
    # Serverへ接続&データ転送
    client.connect((host, port))
    print("successfully connected")

    # バイナリのサイズを通知(int: 4[byte])
    arr_size = len(byte_arr)
    client.send(arr_size.to_bytes(4, 'little'))

    # バイナリデータを送信
    client.sendall(byte_arr)
    print("successfully sent data")

    # Serverから返送されたデータを受信
    data = client.recv(1024)
    print(data)
finally:
    client.close()
