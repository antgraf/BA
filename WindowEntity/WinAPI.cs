using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowEntity
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowRect
	{
		private int _Left;
		private int _Top;
		private int _Right;
		private int _Bottom;

		public WindowRect(System.Drawing.Rectangle Rectangle)
			: this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
		{
		}

		public WindowRect(int Left, int Top, int Right, int Bottom)
		{
			_Left = Left;
			_Top = Top;
			_Right = Right;
			_Bottom = Bottom;
		}

		public int X
		{
			get { return _Left; }
			set { _Left = value; }
		}

		public int Y
		{
			get { return _Top; }
			set { _Top = value; }
		}

		public int Left
		{
			get { return _Left; }
			set { _Left = value; }
		}

		public int Top
		{
			get { return _Top; }
			set { _Top = value; }
		}

		public int Right
		{
			get { return _Right; }
			set { _Right = value; }
		}

		public int Bottom
		{
			get { return _Bottom; }
			set { _Bottom = value; }
		}

		public int Height
		{
			get { return _Bottom - _Top; }
			set { _Bottom = value - _Top; }
		}

		public int Width
		{
			get { return _Right - _Left; }
			set { _Right = value + _Left; }
		}

		public System.Drawing.Point Location
		{
			get { return new System.Drawing.Point(Left, Top); }
			set
			{
				_Left = value.X;
				_Top = value.Y;
			}
		}
		public System.Drawing.Size Size
		{
			get { return new System.Drawing.Size(Width, Height); }
			set
			{
				_Right = value.Width + _Left;
				_Bottom = value.Height + _Top;
			}
		}

		public static implicit operator System.Drawing.Rectangle(WindowRect Rectangle)
		{
			return new System.Drawing.Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
		}

		public static implicit operator WindowRect(System.Drawing.Rectangle Rectangle)
		{
			return new WindowRect(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
		}

		public static bool operator ==(WindowRect Rectangle1, WindowRect Rectangle2)
		{
			return Rectangle1.Equals(Rectangle2);
		}

		public static bool operator !=(WindowRect Rectangle1, WindowRect Rectangle2)
		{
			return !Rectangle1.Equals(Rectangle2);
		}

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public bool Equals(WindowRect Rectangle)
		{
			return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object Object)
		{
			if(Object is WindowRect)
			{
				return Equals((WindowRect)Object);
			}
			else if(Object is System.Drawing.Rectangle)
			{
				return Equals(new WindowRect((System.Drawing.Rectangle)Object));
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ((System.Drawing.Rectangle)this).GetHashCode();
		}
	}

	[Flags]
	internal enum MouseEventFlags
	{
		LEFTDOWN = 0x00000002,
		LEFTUP = 0x00000004,
		MIDDLEDOWN = 0x00000020,
		MIDDLEUP = 0x00000040,
		MOVE = 0x00000001,
		ABSOLUTE = 0x00008000,
		RIGHTDOWN = 0x00000008,
		RIGHTUP = 0x00000010
	}

    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseInputData
    {
		public int dx;
		public int dy;
		public uint mouseData;
		public MouseEventFlags dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
    }

	[StructLayout(LayoutKind.Sequential)]
	internal struct KEYBDINPUT
	{
		 public ushort wVk;
		 public ushort wScan;
		 public uint dwFlags;
		 public uint time;
		 public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct HARDWAREINPUT
	{
		 public int uMsg;
		 public short wParamL;
		 public short wParamH;
	}

    [StructLayout(LayoutKind.Explicit)]
    internal struct MouseKeybdhardwareInputUnion
    {
		[FieldOffset(0)]
		public MouseInputData mi;
		[FieldOffset(0)]
		public KEYBDINPUT ki;
		[FieldOffset(0)]
		public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
		internal SendInputEventType type;
		internal MouseKeybdhardwareInputUnion mkhi;
    }

	enum SendInputEventType : int
	{
		InputMouse,
		InputKeyboard,
		InputHardware
	}

	internal class WinAPI
	{

		internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		internal const UInt32 SWP_NOSIZE = 0x0001;
		internal const UInt32 SWP_NOMOVE = 0x0002;
		internal const UInt32 SWP_SHOWWINDOW = 0x0040;

		internal const int MOUSEEVENTF_LEFTDOWN = 0x02;
		internal const int MOUSEEVENTF_LEFTUP = 0x04;
		internal const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		internal const int MOUSEEVENTF_RIGHTUP = 0x10;

		internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);

		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);
		
		[DllImport("user32.dll")]
		internal static extern IntPtr FindWindow(String sClassName, String sAppName);

		[DllImport("user32.dll")]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref WindowRect rect);

		[DllImport("coredll")]
		internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		// Call this way:
		//SetWindowPos(theWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		internal static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetMessageExtraInfo();
	}
}
