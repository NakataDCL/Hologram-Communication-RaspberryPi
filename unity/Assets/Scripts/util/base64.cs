// using System.Text;

// // Base64形式の汎用操作を提供するクラス
// public static class Base64 {
// 	// 指定した通常の文字列をUTF-8としてBase64文字列に変換する
// 	public static string Encode (string str) {
// 		return Encode (str, Encoding.UTF8);
// 	}
// 	// 上記のエンコードが指定できるバージョン
// 	public static string Encode (string str, Encoding encode) {
// 		return Convert.ToBase64String (encode.GetBytes (str));
// 	}

// 	// 指定したBase64文字列をUTF-8として通常の文字列に変換する
// 	public static string Decode (string base64Str) {
// 		return Decode (base64Str, Encoding.UTF8);
// 	}
// 	// 上記のエンコードが指定できるバージョン
// 	public static string Decode (string base64Str, Encoding encode) {
// 		return encode.GetString (Convert.FromBase64String (base64Str));
// 	}
// }