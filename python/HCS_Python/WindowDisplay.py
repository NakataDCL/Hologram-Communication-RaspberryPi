import cv2
import numpy as np
import subprocess


class WindowDisplay:
    def __init__(self, window_name):
        self._window_name = window_name
        self._dspW, self._dspH = self.get_display_resolution()

    def __del__(self):
        cv2.destroyAllWindows()

    def update_window(self, img):
        cv2.imshow(self._window_name, img)

    def update_fullscreen_window(self, img):
        cv2.namedWindow(self._window_name, cv2.WND_PROP_FULLSCREEN)
        cv2.setWindowProperty(self._window_name, cv2.WND_PROP_FULLSCREEN,
                              cv2.WINDOW_FULLSCREEN)
        cv2.imshow(self._window_name, img)

    # byte配列の画像をndarray型に変換する
    def encode_byte_img(self, b_img):
        nparr = np.fromstring(b_img, np.uint8)
        img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        return img_np

    # フルスクリーン画像を生成する
    def create_fullscreen_img(self, img):
        # 画像のサイズを取得
        img_h, img_w = img.shape[:2]
        # 拡大倍率とオフセットを計算
        ratio = 1.0
        x_offset = y_offset = 0
        if img_h > img_w:
            ratio = self._dspW / img_w
            y_offset = int((self._dspH - img_h)/2)
        else:
            ratio = self._dspH / img_h
            x_offset = int((self._dspW - img_w)/2)
        resized_img = cv2.resize(img, None, fx=ratio, fy=ratio)
        # フルスクリーン画像を生成
        fullscreen_img = np.zeros((self._dspH, self._dspW, 3), np.uint8)
        fullscreen_img[y_offset:y_offset+resized_img.shape[0],
                       x_offset:x_offset+resized_img.shape[1]] = resized_img
        return fullscreen_img

    def command2pipe(self, cmd1, cmd2):
        p = subprocess.Popen(cmd1, stdout=subprocess.PIPE)
        p2 = subprocess.Popen(cmd2, stdin=p.stdout, stdout=subprocess.PIPE)
        p.stdout.close()
        first_line, rest_lines = p2.communicate()
        return(first_line, rest_lines)

    def get_display_resolution(self):
        cmd1 = ['xrandr']
        cmd2 = ['grep', '*']
        resolution_string, junk = self.command2pipe(cmd1, cmd2)
        b_resolution = resolution_string.split()[0]
        resolution = b_resolution.decode('utf-8')
        width, height = resolution.split('x')
        return [int(width), int(height)]
