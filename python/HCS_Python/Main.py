import base64
import sys
import threading
from time import sleep

from WindowDisplay import WindowDisplay
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
window_w = 800
# 表示するウィンドウの高さ
window_h = 600

# 1秒の間に画像を送信する回数
rate = 1


### 送受信処理定義 ###

# frame_rate: 1秒間に送信する回数
def send_image(client, webcam_cnt, send_rate):
    t = 1.0 / send_rate
    while True:
        # Webカメラ画像を取得
        img = webcam_cnt.get_camera_image()
        # 画像をbase64エンコード
        b64_img = base64.b64encode(img)
        # データをサーバに送信
        client.send_byte_arr(b64_img)
        # 一定秒数待機
        sleep(t)


def receive_image(client, window_dsp):
    cnt = 0
    while True:
        # byte配列の画像データをサーバから受信
        b_img = client.receive_byte_arr()
        # ndarrayに変換
        img = window_dsp.encode_byte_img(b_img)
        # ウィンドウをアップデート
        window_dsp.update_window(img)
        print(cnt)
        cnt += 1


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
thread_snd = threading.Thread(target=send_image, args=(client_snd, wc, rate))
thread_rcv = threading.Thread(target=receive_image, args=(client_rcv, wd))

# マルチスレッド処理開始
# thread_snd.start()
thread_rcv.start()

# Ctrl + C で終了
try:
    while True:
        pass
except KeyboardInterrupt:
    print('Exit')
    sys.exit(0)
