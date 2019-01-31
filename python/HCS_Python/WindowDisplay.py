import cv2
import numpy as np


class WindowDisplay:
    def __init__(self, width, height, window_name):
        self._width = width
        self._height = height
        self._window_name = window_name

    def __del__(self):
        cv2.destroyAllWindows()

    def update_window(self, img):
        cv2.imshow(self._window_name, img)
        cv2.namedWindow(self._window_name, cv2.WND_PROP_FULLSCREEN)
        cv2.setWindowProperty(
            self._window_name, cv2.WND_PROP_FULLSCREEN, cv2.WINDOW_FULLSCREEN)

        # byte配列の画像をndarray型に変換する
    def encode_byte_img(self, b_img):
        nparr = np.fromstring(b_img, np.uint8)
        img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        return img_np
