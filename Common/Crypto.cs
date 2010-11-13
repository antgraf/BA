using System;
using System.Globalization;

// ReSharper disable CheckNamespace
namespace BACommon
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Password strings hiding with XOR algorithm using static limited key.
	/// </summary>
	public class Crypto
	{
		private const string key = "DR12ll kgt94$u^$H75#45v.f 28Vk2XX";

		/// <summary>
		/// Encodes string.
		/// </summary>
		/// <param name="data">String to encode.</param>
		/// <returns>Result string.</returns>
		public static string Encode(string data)
		{
			if(data != null)
			{
				char[] buf = new char[data.Length * 2];
				for(int i = 0; i < data.Length; i++)
				{
					byte b = (byte)(data[i] ^ key[i % key.Length]);
					string symbol = b.ToString("X2");
					buf[2 * i] = symbol[0];
					buf[2 * i + 1] = symbol[1];
				}
				return new string(buf);
			}
			return null;
		}

		/// <summary>
		/// Decodes string.
		/// </summary>
		/// <param name="data">String to decode.</param>
		/// <returns>Result string.</returns>
		public static string Decode(string data)
		{
			if(data != null)
			{
				char[] buf = new char[data.Length / 2];
				for(int i = 0; i < buf.Length; i++)
				{
					byte b = Byte.Parse(data.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
					buf[i] = (char)(b ^ (byte)key[i % key.Length]);
				}
				return new string(buf);
			}
			return null;
		}
	}
}