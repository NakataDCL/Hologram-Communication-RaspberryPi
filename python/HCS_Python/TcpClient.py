import socket
import base64


class TcpClient:
    def __init__(self, host_ip, port):
        self._ip = host_ip
        self._port = port
        self._client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print("succesfully connected... " +
              str(self._ip) + ":" + str(self._port))

    # Serverへ接続
    def connect(self):
        self._client.connect((self._ip, self._port))

    # Serverから切断
    def disconnect(self):
        self._client.close()

    # byte配列をサーバに送信する
    def send_byte_arr(self, byte_arr):
        try:
            # 送信するbyte配列のサイズを通知(int: 4[byte])
            arr_size = len(byte_arr)
            b_arr_size = arr_size.to_bytes(4, 'little')
            self._client.send(b_arr_size)

            # byte配列を送信
            self._client.sendall(byte_arr)
        except OSError:
            print("send error")

    # byte配列をサーバから受信する
    def receive_byte_arr(self):
        try:
            # 受信するbyte配列のサイズを取得(int: 4[byte])
            b_arr_size = self._client.recv(4)
            arr_size = int.from_bytes(b_arr_size, "little")

            # byte配列を受信
            b_data = self._client.recv(arr_size)
            return b_data
        except OSError:
            print("receive error")
            return '\x00'
