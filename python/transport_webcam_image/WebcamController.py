import cv2
from PIL import Image


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
            # print(type(frame))
            success, encoded_frame = cv2.imencode('.png', frame)
            # print(type(encoded_frame.tobytes()))
            return encoded_frame.tobytes()
