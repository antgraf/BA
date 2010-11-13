using System;

// ReSharper disable CheckNamespace
namespace BACommon
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Stores global constants and values used in other BA components and libraries.
	/// </summary>
	public static class Globals
	{
		// VERSION INFORMATION
		/// <summary>
		/// Version Control System version number (XXYYZZ format).
		/// </summary>
		public const int Version = 100;
		/// <summary>
		/// Version Control System major version.
		/// </summary>
		public const string MajorVersion = "0";
		/// <summary>
		/// Version Control System minor version.
		/// </summary>
		public const string MinorVersion = "4";
		/// <summary>
		/// Version Control System patch version.
		/// </summary>
		public const string PatchVersion = "0";
		/// <summary>
		/// Version Control System brief version information.
		/// </summary>
		public const string VersionText = "BA Alpha 0.4.0";
		/// <summary>
		/// x64 platform marker.
		/// </summary>
// ReSharper disable InconsistentNaming
		public static bool x64
// ReSharper restore InconsistentNaming
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}
	}
}
