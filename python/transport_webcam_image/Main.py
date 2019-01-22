from WebcamController import WebcamController

wc = WebcamController(100, 100)

# カメラ画像を取得
img_data = wc.get_camera_image()
