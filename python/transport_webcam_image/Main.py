import base64
from WebcamController import WebcamController
from TCPClient import TcpClient
from time import sleep

# 接続先ホストの情報
host_ip = "localhost"
port = 8080

# ウェブカメラ操作の初期化処理
wc = WebcamController(100, 100)

# ソケット通信の初期化処理
tcp_client = TcpClient(host_ip, port)
tcp_client.connect()

for i in range(5):
    # カメラ画像を取得
    img_byte = wc.get_camera_image()

    # 画像データをbase64にエンコード
    b64encoded_img = base64.b64encode(img_byte)

    # 画像データをサーバに送信
    tcp_client.transport_byte_arr(b64encoded_img)

    sleep(1)
