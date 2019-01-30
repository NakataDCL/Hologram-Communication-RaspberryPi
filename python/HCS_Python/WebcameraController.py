import cv2


class WebcameraController:
    def __init__(self, width, height):
        cap = cv2.VideoCapture(0)
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, width)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, height)
        self._capture = cap

    def __del__(self):
        self._capture.release()

    # webカメラから取得した画像をbyte列で返す
    def get_camera_image(self):
        if self._capture.isOpened():
            ret, frame = self._capture.read()
            success, encoded_frame = cv2.imencode('.png', frame)
            return encoded_frame
        else:
            print("can not get webcam image.")
            return '\x00'
