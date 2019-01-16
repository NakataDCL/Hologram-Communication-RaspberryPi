import base64

txt_path = "./doc/test.txt"

# テキストファイルをバイナリモードで開く
file = open(txt_path, "rb")
data = file.read()
file.close()
print(data)

# base64にエンコード
byte_arr = base64.b64encode(data)
print("len: " + str(len(byte_arr)) + ", " + str(byte_arr))

# デコード
txt = base64.b64decode(byte_arr)
print(txt)

# stringに変換
string = txt.decode('utf-8')
print(string)
