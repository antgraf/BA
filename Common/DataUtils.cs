using System;

namespace BACommon
{
	/// <summary>
	/// Data manipulations utility class.
	/// </summary>
	public sealed class DataUtils
	{
		private const int Int16Length = 2;
		private const int Int32Length = 4;
		private const int Int64Length = 8;
		private const byte ByteMask = 0xFF;
		private const byte ByteLength = 8;

		/// <summary>
		/// Check array is null or empty.
		/// </summary>
		/// <param name="ar">Array to check.</param>
		/// <returns>True if array is null or empty; false otherwise.</returns>
		public static bool ArrayIsEmpty(Array ar)
		{
			return ar == null || ar.Length == 0;
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(Int16 i)
		{
			return AllToBytes(unchecked((UInt64)i), Int16Length);
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(UInt16 i)
		{
			return AllToBytes(i, Int16Length);
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(Int32 i)
		{
			return AllToBytes(unchecked((UInt64)i), Int32Length);
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(UInt32 i)
		{
			return AllToBytes(unchecked((UInt64)i), Int32Length);
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(Int64 i)
		{
			return AllToBytes(unchecked((UInt64)i), Int64Length);
		}

		/// <summary>
		/// Converts (serializes) integer to bytes array.
		/// </summary>
		/// <param name="i">Integer to serialize.</param>
		/// <returns>Bytes array.</returns>
		public static byte[] ToBytes(UInt64 i)
		{
			return AllToBytes(i, Int64Length);
		}

		private static byte[] AllToBytes(UInt64 i, int length)
		{
			byte[] buf = new byte[length];
			for(int p = 0; p < length; p++)
			{
				buf[p] = (byte)(ByteMask & (i >> ((length - p - 1) * ByteLength)));
			}
			return buf;
		}

		/// <summary>
		/// Converts (deserializes) bytes array to unsigned integer.
		/// <seealso cref="ToUnsigned">ToSigned</seealso>
		/// </summary>
		/// <param name="bytes">Bytes array to deserialize.</param>
		/// <returns>Unsigned integer.</returns>
		public static UInt64 ToUnsigned(byte[] bytes)
		{
			if(ArrayIsEmpty(bytes))
			{
				return 0;
			}
			if(bytes.Length > Int64Length)
			{
				throw new OverflowException("Cannot convert bytes array to integer. Array length is too long.");
			}
			UInt64 i = 0;
			for(int p = 0; p < bytes.Length; p++)
			{
				i |= (UInt64)bytes[p] << ((bytes.Length - p - 1) * ByteLength);
			}
			return i;
		}

		/// <summary>
		/// Converts (deserializes) bytes array to long signed integer.
		/// <seealso cref="ToUnsigned"></seealso>
		/// </summary>
		/// <param name="bytes">Bytes array to deserialize.</param>
		/// <returns>Signed integer.</returns>
		public static Int64 ToSigned(byte[] bytes)
		{
			if(bytes.Length > Int32Length)
			{
				return unchecked((Int64)ToUnsigned(bytes));
			}
			else if(bytes.Length > Int16Length)
			{
				return unchecked((Int32)ToUnsigned(bytes));
			}
			else
			{
				return unchecked((Int16)ToUnsigned(bytes));
			}
		}
	}
}
