using System;
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

		public WindowRect(System.Drawing.Rectangle rectangle)
			: this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
		{}

		public WindowRect(int left, int top, int right, int bottom)
		{
			_Left = left;
			_Top = top;
			_Right = right;
			_Bottom = bottom;
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

		public static implicit operator System.Drawing.Rectangle(WindowRect rectangle)
		{
			return new System.Drawing.Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}

		public static implicit operator WindowRect(System.Drawing.Rectangle rectangle)
		{
			return new WindowRect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}

		public static bool operator ==(WindowRect rectangle1, WindowRect rectangle2)
		{
			return rectangle1.Equals(rectangle2);
		}

		public static bool operator !=(WindowRect rectangle1, WindowRect rectangle2)
		{
			return !rectangle1.Equals(rectangle2);
		}

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public bool Equals(WindowRect rectangle)
		{
			return rectangle.Left == _Left && rectangle.Top == _Top && rectangle.Right == _Right && rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object @object)
		{
			if(@object is WindowRect)
			{
				return Equals((WindowRect)@object);
			}
			if(@object is System.Drawing.Rectangle)
			{
				return Equals(new WindowRect((System.Drawing.Rectangle)@object));
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
		LeftDown = 0x00000002,
		LeftUp = 0x00000004,
		MiddleDown = 0x00000020,
		MiddleUp = 0x00000040,
		Move = 0x00000001,
		Absolute = 0x00008000,
		RightDown = 0x00000008,
		RightUp = 0x00000010
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
	internal struct KeyboardInput
	{
		 public ushort wVk;
		 public ushort wScan;
		 public uint dwFlags;
		 public uint time;
		 public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct HardwareInput
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
		public KeyboardInput ki;
		[FieldOffset(0)]
		public HardwareInput hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Input
    {
		internal SendInputEventType type;
		internal MouseKeybdhardwareInputUnion mkhi;
    }

	internal enum SendInputEventType
	{
		InputMouse,
		InputKeyboard,
		InputHardware
	}

	internal enum ShowWindowCommand
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
	internal struct WindowPoint
	{
		public int X;
		public int Y;

		public WindowPoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static implicit operator System.Drawing.Point(WindowPoint p)
		{
			return new System.Drawing.Point(p.X, p.Y);
		}

		public static implicit operator WindowPoint(System.Drawing.Point p)
		{
			return new WindowPoint(p.X, p.Y);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowRectangle
	{
		private int _Left;
		private int _Top;
		private int _Right;
		private int _Bottom;

		public WindowRectangle(System.Drawing.Rectangle rectangle)
			: this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
		{
		}

		public WindowRectangle(int left, int top, int right, int bottom)
		{
			_Left = left;
			_Top = top;
			_Right = right;
			_Bottom = bottom;
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

		public static implicit operator System.Drawing.Rectangle(WindowRectangle rectangle)
		{
			return new System.Drawing.Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}

		public static implicit operator WindowRectangle(System.Drawing.Rectangle rectangle)
		{
			return new WindowRectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}

		public static bool operator ==(WindowRectangle rectangle1, WindowRectangle rectangle2)
		{
			return rectangle1.Equals(rectangle2);
		}

		public static bool operator !=(WindowRectangle rectangle1, WindowRectangle rectangle2)
		{
			return !rectangle1.Equals(rectangle2);
		}

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public bool Equals(WindowRectangle rectangle)
		{
			return rectangle.Left == _Left && rectangle.Top == _Top && rectangle.Right == _Right && rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object @object)
		{
			if(@object is WindowRectangle)
			{
				return Equals((WindowRectangle)@object);
			}
			if(@object is System.Drawing.Rectangle)
			{
				return Equals(new WindowRectangle((System.Drawing.Rectangle)@object));
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
	internal struct WindowPlacement
	{
		public int Length;
		public int Flags;
		public ShowWindowCommand ShowCmd;
		public WindowPoint MinPosition;
		public WindowPoint MaxPosition;
		public WindowRectangle NormalPosition;

		public static WindowPlacement Default
		{
			get
			{
				WindowPlacement result = new WindowPlacement();
				result.Length = Marshal.SizeOf(result);
				return result;
			}
		}
	}

	[Flags]
	internal enum WindowFromPointFlags
	{
		CwpAll = 0x0000,
		CwpSkipInvisible = 0x0001,
		CwpSkipDisabled = 0x0002,
		CwpSkipTransparent = 0x0003
	}

	internal enum GetAncestorFlags
	{
		GetParent = 1,
		GetRoot = 2,
		GetRootOwner = 3
	}

	internal sealed class WinApi
	{

		internal static readonly IntPtr HwndTopmost = new IntPtr(-1);
		internal const UInt32 SwpNoSize = 0x0001;
		internal const UInt32 SwpNoMove = 0x0002;
		internal const UInt32 SwpNoZOrder = 0x0004;
		internal const UInt32 SwpShowWindow = 0x0040;

		internal const int MouseEventfLeftDown = 0x02;
		internal const int MouseEventfLeftUp = 0x04;
		internal const int MouseEventfRightDown = 0x08;
		internal const int MouseEventfRightUp = 0x10;

		internal const UInt32 SwHide = 0;
		internal const UInt32 SwShowNormal = 1;
		internal const UInt32 SwNormal = 1;
		internal const UInt32 SwShowMinimized = 2;
		internal const UInt32 SwShowMaximized = 3;
		internal const UInt32 SwMaximize = 3;
		internal const UInt32 SwShowNoActivate = 4;
		internal const UInt32 SwShow = 5;
		internal const UInt32 SwMinimize = 6;
		internal const UInt32 SwShowMinNoActive = 7;
		internal const UInt32 SwShowNA = 8;
		internal const UInt32 SwRestore = 9;

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
		internal static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
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
		internal static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetMessageExtraInfo();

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		internal static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

		[DllImport("user32.dll")]
		internal static extern IntPtr WindowFromPoint(WindowPoint windowPoint);

		[DllImport("user32.dll")]
		internal static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, WindowPoint pt, uint uFlags);

		[DllImport("user32.dll", ExactSpelling = true)]
		internal static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags gaFlags);
	}
}
