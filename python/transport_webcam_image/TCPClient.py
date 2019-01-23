import socket
import base64


class TcpClient:
    def __init__(self, host_ip, port):
        self._ip = host_ip
        self._port = port
        self._client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    def __del__(self):
        self.disconnect()

    # Serverへ接続
    def connect(self):
        self._client.connect((self._ip, self._port))

    # Serverから切断
    def disconnect(self):
        self._client.close()

    # byte配列をサーバに送信する
    def transport_byte_arr(self, byte_arr):
        try:
            # 送信するbyte配列のサイズを通知(int: 4[byte])
            arr_size = len(byte_arr)
            self._client.send(arr_size.to_bytes(4, 'little'))
            print("send: " + str(arr_size) + "[byte]")

            # byte配列を送信
            self._client.sendall(byte_arr)

            # Serverから返送されたデータを受信
            res = self._client.recv(1024)
            print(res)
            if not res:
                print("err")
        except OSError:
            print("error")
