import base64
from ImageManager import ImageManager
from TCPClient import TcpClient

# 接続先ホストの情報
host_ip = "localhost"
port = 8081

# ウィンドウ操作の初期化処理
im = ImageManager(800, 600, "Screenshot")

# ソケット通信の初期化処理
tcp_client = TcpClient(host_ip, port)
tcp_client.connect()

while True:
    # 画像を受信
    b_img = tcp_client.receive_byte_arr()

    print(len(b_img))

    # 画像データをデコード
    b64decoded_img = base64.b64decode(b_img)

    # 画像データをndarrayに変換
    img = im.encode_byte_img(b64decoded_img)
    print(type(img))

    # ウィンドウをアップデート
    im.update_window(img)
