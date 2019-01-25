import socket
import base64


class TcpClient:
    def __init__(self, host_ip, port):
        self._ip = host_ip
        self._port = port
        self._client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print("succesfully connected")

    # Serverへ接続
    def connect(self):
        self._client.connect((self._ip, self._port))

    # Serverから切断
    def disconnect(self):
        self._client.close()

    # byte配列をサーバから受信する
    def receive_byte_arr(self):
        try:
            # 受信するbyte配列のサイズを取得(int: 4[byte])
            b_arr_size = self._client.recv(4)
            arr_size = int.from_bytes(b_arr_size, "little")
            print(arr_size)

            # byte配列を受信
            b_data = self._client.recv(arr_size)

            # Serverに受信したことを通知

            return b_data

        except OSError:
            print("error")
            return '\x00'
