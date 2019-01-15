import socket

# 接続先ホストの情報
host = "localhost"
port = 8080

# 送信する文字列
data = "Hello World!"

# clientオブジェクトの作成
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

try:
    # Serverへ接続&データ転送
    client.connect((host, port))
    client.sendall(bytes(data + "\n", 'UTF-8'))
    # Serverから返送されたデータを受信
    data = client.recv(1024)
    print(data)
finally:
    client.close()
