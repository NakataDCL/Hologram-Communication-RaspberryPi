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
    # base64のbyte配列を受信
    b64_data = tcp_client.receive_byte_arr()

    # base64データをデコード
    b_data = base64.b64decode(b64_data)

    # ndarrayに変換
    img_arr = im.encode_byte_img(b_data)

    # ウィンドウをアップデート
    im.update_window(img_arr)
