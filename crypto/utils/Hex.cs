using System.Globalization;
using System.Text;

namespace JP.CryptoClass
{
	/// <summary>Static methods to convert numbers between byte arrays and string representations in hexadecimal.</summary>
	public static class Hex
	{
		/// <summary>For example, from "426f62" it returns { 0x42, 0x6f, 0x62 }</summary>
		public static byte[] GetBytes(string text)
		{
			var bytes = new byte[text.Length / 2];
			for(int i = 0; i < bytes.Length; ++i)
				bytes[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);

			return bytes;
		}

		/// <summary>For example, from { 0x42, 0x6f, 0x62 } it returns "426f62"</summary>
		public static string GetFromBytes(byte[] arr)
		{
			var text = new StringBuilder(arr.Length * 2);
			foreach(var b in arr)
				text.Append(b.ToString("x2"));

			return text.ToString();
		}
	}
}
