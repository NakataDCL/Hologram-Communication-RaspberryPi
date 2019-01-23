import cv2

img_width = 800
img_height = 600
window_name = str(img_width) + ' x ' + str(img_height)

# VideoCapture オブジェクトを取得
capture = cv2.VideoCapture(0)

while True:
    ret, frame = capture.read()
    scaled_image = cv2.resize(frame, dsize=(img_width, img_height))
    cv2.imshow(window_name, scaled_image)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

capture.release()
cv2.destroyAllWindows()
