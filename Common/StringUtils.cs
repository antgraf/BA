using System;
using System.Text;

// ReSharper disable CheckNamespace
namespace BACommon
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Utility class for string manipulations.
	/// </summary>
	public static class StringUtils
	{
		private static readonly char[] hexDigitsUpper = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
		private static readonly char[] hexDigitsLower = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

		/// <summary>
		/// Checks if string is null or has zero-length.
		/// </summary>
		/// <param name="str">String to check.</param>
		/// <returns>True if string is null or has zero-length. False otherwise.</returns>
		public static bool IsEmpty(string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Converts string to its UTF-8 representation.
		/// </summary>
		/// <param name="str">String to convert.</param>
		/// <returns>Converted string.</returns>
		public static string ConvertToUtf8(string str)
		{
			byte[] buf = Encoding.UTF8.GetBytes(str);
			return Encoding.Default.GetString(buf);
		}

		/// <summary>
		/// Converts string from its UTF-8 representation.
		/// </summary>
		/// <param name="str">String to convert.</param>
		/// <returns>Converted string.</returns>
		public static string ConvertFromUtf8(string str)
		{
			byte[] buf = Encoding.Default.GetBytes(str);
			return Encoding.UTF8.GetString(buf);
		}

		/// <summary>
		/// Converts string to its UTF-8 representation as Hex-string.
		/// </summary>
		/// <param name="str">String to convert.</param>
		/// <returns>Converted string.</returns>
		public static string ConvertToHexUtf8(string str)
		{
			byte[] buf = Encoding.UTF8.GetBytes(str);
			return ConvertToHexString(buf);
		}

		/// <summary>
		/// Converts string from its UTF-8 representation as Hex-string.
		/// </summary>
		/// <param name="str">String to convert.</param>
		/// <returns>Converted string.</returns>
		public static string ConvertFromHexUtf8(string str)
		{
			byte[] buf = ConvertFromHexString(str);
			return Encoding.UTF8.GetString(buf);
		}

		/// <summary>
		/// Encodes Hex-string.
		/// </summary>
		/// <param name="bytes">Bytes array to encode.</param>
		/// <param name="upperCase">Generate uppercase hex-codes flag.</param>
		/// <returns>Hex-string.</returns>
		public static string ConvertToHexString(byte[] bytes, bool upperCase = false)
		{
			char[] hexDigits = upperCase ? hexDigitsUpper : hexDigitsLower;
			char[] chars = new char[bytes.Length * 2];
			for(int i = 0; i < bytes.Length; i++)
			{
				int b = bytes[i];
				chars[i * 2] = hexDigits[b >> 4];
				chars[i * 2 + 1] = hexDigits[b & 0xF];
			}
			return new string(chars);
		}

		/// <summary>
		/// Decodes Hex-string.
		/// </summary>
		/// <param name="str">Hex-string to decode.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ConvertFromHexString(string str)
		{
			throw new NotImplementedException("ConvertFromHexString() is not implemented yet.");
			/*
			byte[] bytes = new char[str.Length / 2];
			for (int i = 0; i < bytes.Length; i++) 
			{
			}
			return null;
			*/
		}
	}
}
