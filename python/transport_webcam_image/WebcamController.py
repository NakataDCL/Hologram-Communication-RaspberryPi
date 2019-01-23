import cv2


class WebcamController:
    def __init__(self, width, height):
        self._width = width
        self._height = height
        self._capture = cv2.VideoCapture(0)

    def __del__(self):
        self._capture.release()

    # webカメラから取得した画像をbyte列で返す
    def get_camera_image(self):
        if self._capture.isOpened():
            ret, frame = self._capture.read()
            # 画像サイズが大きすぎるとハングするので注意
            frame = frame[0:300, 0:150]
            success, encoded_frame = cv2.imencode('.png', frame)
            return encoded_frame.tobytes()
        else:
            print("can not get webcam image.")
            return '\x00'

    # webカメラの画像のエンコードテスト
    def test_webcam_img_encode(self):
        if self._capture.isOpened():
            ret, frame = self._capture.read()
            print(type(frame))

            # webcameraから取得した画像をpngにエンコード
            success, encoded_frame = cv2.imencode('.png', frame)
            print(type(encoded_frame))

            # webcameraから取得した画像を一度保存
            cv2.imwrite('res.png', frame)

            # 画像をopencvで開き、pngにエンコード
            img = cv2.imread("./res.png")
            print(type(img))
            success, encoded_img = cv2.imencode('.png', img)

            # 画像ファイルをバイナリモードで開く
            file = open("./res.png", "rb")
            data = file.read()
            file.close()
            print(type(data))  # これと同じものを取得したい

            print(data == frame.tobytes())  # false
            print(data == encoded_frame.tobytes())  # true
            print(data == encoded_img.tobytes())  # true
