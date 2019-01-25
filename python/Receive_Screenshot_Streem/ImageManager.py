import cv2
import numpy as np


class ImageManager:
    def __init__(self, width, height, window_name):
        self._width = width
        self._height = height
        self._window_name = window_name

    def __del__(self):
        cv2.destroyAllWindows()

    def update_window(self, img):
        cv2.imshow(self._window_name, img)

    # byte配列の画像をndarray型に変換する
    def encode_byte_img(self, b_img):
        print(type(b_img))
        return cv2.imdecode(b_img, -1)
        # return cv2.imdecode(np.frombuffer(b_img, np.int64), -1)
