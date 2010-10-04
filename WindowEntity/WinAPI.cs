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

	internal enum SendInputEventType : int
	{
		InputMouse,
		InputKeyboard,
		InputHardware
	}

	internal enum ShowWindowCommand : int
	{
		Hide = 0,
		Normal = 1,
		ShowMinimized = 2,
		Maximize = 3, // is this the right value?
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActive = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public static implicit operator System.Drawing.Point(POINT p)
		{
			return new System.Drawing.Point(p.X, p.Y);
		}

		public static implicit operator POINT(System.Drawing.Point p)
		{
			return new POINT(p.X, p.Y);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		private int _Left;
		private int _Top;
		private int _Right;
		private int _Bottom;

		public RECT(System.Drawing.Rectangle Rectangle)
			: this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
		{
		}

		public RECT(int Left, int Top, int Right, int Bottom)
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

		public static implicit operator System.Drawing.Rectangle(RECT Rectangle)
		{
			return new System.Drawing.Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
		}

		public static implicit operator RECT(System.Drawing.Rectangle Rectangle)
		{
			return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
		}

		public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
		{
			return Rectangle1.Equals(Rectangle2);
		}

		public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
		{
			return !Rectangle1.Equals(Rectangle2);
		}

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public bool Equals(RECT Rectangle)
		{
			return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object Object)
		{
			if(Object is RECT)
			{
				return Equals((RECT)Object);
			}
			else if(Object is System.Drawing.Rectangle)
			{
				return Equals(new RECT((System.Drawing.Rectangle)Object));
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _Left ^ _Top ^ _Right ^ _Bottom;
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal struct WINDOWPLACEMENT
	{
		public int Length;
		public int Flags;
		public ShowWindowCommand ShowCmd;
		public POINT MinPosition;
		public POINT MaxPosition;
		public RECT NormalPosition;

		public static WINDOWPLACEMENT Default
		{
			get
			{
				WINDOWPLACEMENT result = new WINDOWPLACEMENT();
				result.Length = Marshal.SizeOf(result);
				return result;
			}
		}
	}

	internal sealed class WinAPI
	{

		internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		internal const UInt32 SWP_NOSIZE = 0x0001;
		internal const UInt32 SWP_NOMOVE = 0x0002;
		internal const UInt32 SWP_NOZORDER = 0x0004;
		internal const UInt32 SWP_SHOWWINDOW = 0x0040;

		internal const int MOUSEEVENTF_LEFTDOWN = 0x02;
		internal const int MOUSEEVENTF_LEFTUP = 0x04;
		internal const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		internal const int MOUSEEVENTF_RIGHTUP = 0x10;

		internal const UInt32 SW_HIDE = 0;
		internal const UInt32 SW_SHOWNORMAL = 1;
		internal const UInt32 SW_NORMAL = 1;
		internal const UInt32 SW_SHOWMINIMIZED = 2;
		internal const UInt32 SW_SHOWMAXIMIZED = 3;
		internal const UInt32 SW_MAXIMIZE = 3;
		internal const UInt32 SW_SHOWNOACTIVATE = 4;
		internal const UInt32 SW_SHOW = 5;
		internal const UInt32 SW_MINIMIZE = 6;
		internal const UInt32 SW_SHOWMINNOACTIVE = 7;
		internal const UInt32 SW_SHOWNA = 8;
		internal const UInt32 SW_RESTORE = 9;

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

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		internal static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
	}
}
