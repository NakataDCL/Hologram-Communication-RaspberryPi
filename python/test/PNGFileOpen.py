import cv2
import numpy as np

# 展開する画像のファイルパス
img_path = "./log.png"

# 画像ファイルをバイナリモードで開く
file = open(img_path, "rb")
data = file.read()
file.close()

print(len(data))

file = open("./data", "wb")
file.write(data)
file.close()

# 変換
nparr = np.fromstring(data, np.uint8)
img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

while True:
    cv2.imshow("test", img_np)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
