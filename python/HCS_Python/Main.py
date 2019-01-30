import cv2
import sys


from WindowDisplay import WindowDisplay
from LoopThread import LoopThread
from TcpClient import TcpClient
from WebcameraController import WebcameraController


### 定数宣言 ###

# 接続先ホストのIPアドレス
host_ip = "localhost"
# Python -> Unity 用のポート番号
port_snd = 8080
# Unity -> Python用のポート番号
port_rcv = 8081

# 表示するウィンドウの幅
window_w = 640
# 表示するウィンドウの高さ
window_h = 480

# 画像を送信した後の待機秒数[sec]
interval = 1


### 送受信処理定義 ###

def send_image(client, webcam_cnt):
    # Webカメラ画像を取得
    img = webcam_cnt.get_camera_image()
    # データをサーバに送信
    client.send_byte_arr(img)


def receive_image(client, window_dsp):
    # byte配列の画像データをサーバから受信
    b_img = client.receive_byte_arr()
    # ndarrayに変換
    img = window_dsp.encode_byte_img(b_img)
    # ウィンドウをアップデート
    window_dsp.update_window(img)


### 初期化処理 ###

# ウィンドウ表示
wd = WindowDisplay(window_w, window_h, "window")
# Webカメラ操作
wc = WebcameraController(window_w, window_h)
# TCPクライアント(Python -> Unity)
client_snd = TcpClient(host_ip, port_snd)
# TCPクライアント(Unity -> Python)
client_rcv = TcpClient(host_ip, port_rcv)


### メイン処理 ###

# サーバ接続
client_snd.connect()
client_rcv.connect()

# マルチスレッド処理宣言
lt_snd = LoopThread(interval=interval, func=send_image, args=(client_snd, wc))

# マルチスレッド処理開始
lt_snd.start()

# Ctrl + C で終了
try:
    while True:
        # cv2.imshow()とcv2.waitKeyはMainスレッドで実行しないといけない
        receive_image(client_rcv, wd)
except KeyboardInterrupt:
    lt_snd.stop()
    print('Exit')
    sys.exit(0)
