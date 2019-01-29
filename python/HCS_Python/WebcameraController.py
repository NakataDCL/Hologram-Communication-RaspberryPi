import cv2


class WebcameraController:
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
            frame = frame[0:self._width, 0:self._width]
            success, encoded_frame = cv2.imencode('.png', frame)
            return encoded_frame
        else:
            print("can not get webcam image.")
            return '\x00'
