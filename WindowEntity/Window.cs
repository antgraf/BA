using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WindowsInput;
using System.Windows.Forms;
using System.Threading;

namespace WindowEntity
{
	public struct WindowHandle
	{
		public IntPtr Handle;

		public static bool operator ==(WindowHandle a, WindowHandle b)
		{
			// If both are null, or both are same instance, return true.
			if(System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if(((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.Handle == b.Handle;
		}

		public static bool operator !=(WindowHandle a, WindowHandle b)
		{
			return !(a == b);
		}

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}

			// Return true if the fields match:
			return Handle == ((WindowHandle)obj).Handle;
		}

		public bool Equals(WindowHandle p)
		{
			// Return true if the fields match:
			return Handle == p.Handle;
		}

		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}
	}

	public enum MouseActions
	{
		LeftClick,
		RightClick,
		MiddleClick,
		X1Click,
		X2Click,
		LeftDown,
		RightDown,
		MiddleDown,
		X1Down,
		X2Down,
		LeftUp,
		RightUp,
		MiddleUp,
		X1Up,
		X2Up
	}

	public class Window
	{
		private const int MaxTitleLength = 512;
		private const int DefaultTimerDeviationPercent = 15;
		private const int DefaultTimerKeyWait = 100;

		private static Random pRandom = new Random();

		private double pTimeMultiplier = 1.0;
		private int pWidth = -1;
		private int pHeight = -1;
		private int pX = -1;
		private int pY = -1;
		private string pTitle = null;
		private WindowHandle pHandle = new WindowHandle() { Handle = IntPtr.Zero };

		#region CommonMethods

		protected virtual bool IsActivationNeeded()
		{
			if(this.GetType().IsSubclassOf(typeof(Screen)) || this.GetType() == typeof(Screen))
			{
				return false;
			}
			return WinAPI.GetForegroundWindow() != pHandle.Handle;
		}

		protected virtual bool ActivateIfNeeded()
		{
			bool needed = IsActivationNeeded();
			if(needed)
			{
				WinAPI.SetForegroundWindow(pHandle.Handle);
				Wait(100);
			}
			return needed;
		}

		public static Window[] FindWindowsByProcessName(string process)
		{
			List<Window> windows = new List<Window>();
			Process[] procs = Process.GetProcessesByName(process);
			IntPtr hWnd;
			foreach(Process proc in procs)
			{
				if((hWnd = proc.MainWindowHandle) != IntPtr.Zero)
				{
					Window window = FromHandle(hWnd);
					if(window != null)
					{
						windows.Add(window);
					}
				}
			}
			return windows.ToArray();
		}

		public static Window FromHandle(IntPtr hwnd)
		{
			WindowRect rect = new WindowRect();
			if(!WinAPI.GetWindowRect(hwnd, ref rect))
			{
				return null;
			}
			Window window = new Window();
			window.Handle = new WindowHandle() { Handle = hwnd };
			window.X = rect.X;
			window.Y = rect.Y;
			window.Width = rect.Width;
			window.Height = rect.Height;
			window.Title = GetTitle(hwnd);
			return window;
		}

		public static string GetTitle(IntPtr hwnd)
		{
			StringBuilder title = new StringBuilder(MaxTitleLength);
			if(WinAPI.GetWindowText(hwnd, title, MaxTitleLength) == 0)
			{
				return null;
			}
			return title.ToString();
		}

		public static void WaitGlobal(int msec)
		{
			Thread.Sleep(msec);
		}

		public static void WaitRandomGlobal(int msec)
		{
			WaitRandomGlobal(msec, DefaultTimerDeviationPercent);
		}

		public static void WaitRandomGlobal(int msec, int percentDelta)
		{
			//if(percentDelta >= 100) throw new ArgumentException("Percentage deviation should be < 100%", "percentDelta");
			int minmsec = (int)(msec - msec * (percentDelta / 100.0));
			if(minmsec <= 0) minmsec = 1;
			int maxmsec = (int)(msec + msec * (percentDelta / 100.0));
			WaitRandomInRangeGlobal(minmsec, maxmsec);
		}

		public static void WaitRandomInRangeGlobal(int msecMin, int msecMax)
		{
			int msec = pRandom.Next(msecMin, msecMax);
			WaitGlobal(msec);
		}

		public virtual void Wait(int msec)
		{
			Thread.Sleep((int)(msec * pTimeMultiplier));
		}

		public virtual void WaitRandom(int msec)
		{
			WaitRandom(msec, DefaultTimerDeviationPercent);
		}

		public virtual void WaitRandom(int msec, int percentDelta)
		{
			//if(percentDelta >= 100) throw new ArgumentException("Percentage deviation should be < 100%", "percentDelta");
			int minmsec = (int)(msec - msec * (percentDelta / 100.0));
			if(minmsec <= 0) minmsec = 1;
			int maxmsec = (int)(msec + msec * (percentDelta / 100.0));
			WaitRandomInRange(minmsec, maxmsec);
		}

		public virtual void WaitRandomInRange(int msecMin, int msecMax)
		{
			int msec = pRandom.Next(msecMin, msecMax);
			Wait(msec);
		}

		public virtual void Close()
		{
			KeySend("%{F4}");
		}

		#endregion

		#region MouseMethods

		public virtual void SetCursorPosition(Coordinate point)
		{
			Point abs = point.ToAbsolute(this);
			System.Drawing.Point pt = new System.Drawing.Point(abs.X, abs.Y);
			Cursor.Position = pt;
		}

		/*
		public virtual void InstantClick(MouseActions action, Coordinate point)
		{
			Click(action, point, false);
		}
		*/

		public virtual void Click(MouseActions action, Coordinate point)
		{
			Click(action, point, true);
		}

		private void Click(MouseActions action, Coordinate point, bool moveCursor)
		{
			ActivateIfNeeded();
			int x = 0;
			int y = 0;
			MouseFlag absflag = MouseFlag.EMPTY;
			if(moveCursor)
			{
				SetCursorPosition(point);
			}
			else
			{
				Point pt = point.ToAbsolute(this);
				x = pt.X;
				y = pt.Y;
				absflag = MouseFlag.ABSOLUTE;
			}
			switch(action)
			{
				case MouseActions.LeftClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.LEFTDOWN | absflag, MouseFlag.LEFTUP | absflag, x, y);
					break;
				}
				case MouseActions.LeftDown:
				{
					InputSimulator.SimulateClick(MouseFlag.LEFTDOWN | absflag, x, y);
					break;
				}
				case MouseActions.LeftUp:
				{
					InputSimulator.SimulateClick(MouseFlag.LEFTUP | absflag, x, y);
					break;
				}
				case MouseActions.MiddleClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.MIDDLEDOWN | absflag, MouseFlag.MIDDLEUP | absflag, x, y);
					break;
				}
				case MouseActions.MiddleDown:
				{
					InputSimulator.SimulateClick(MouseFlag.MIDDLEDOWN | absflag, x, y);
					break;
				}
				case MouseActions.MiddleUp:
				{
					InputSimulator.SimulateClick(MouseFlag.MIDDLEUP | absflag, x, y);
					break;
				}
				case MouseActions.RightClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.RIGHTDOWN | absflag, MouseFlag.RIGHTUP | absflag, x, y);
					break;
				}
				case MouseActions.RightDown:
				{
					InputSimulator.SimulateClick(MouseFlag.RIGHTDOWN | absflag, x, y);
					break;
				}
				case MouseActions.RightUp:
				{
					InputSimulator.SimulateClick(MouseFlag.RIGHTUP | absflag, x, y);
					break;
				}
				case MouseActions.X1Click:
				{
					InputSimulator.SimulateClickPress(MouseFlag.XDOWN | absflag, MouseFlag.XUP | absflag, 0, x, y);
					break;
				}
				case MouseActions.X1Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDOWN | absflag, 0, x, y);
					break;
				}
				case MouseActions.X1Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUP | absflag, 0, x, y);
					break;
				}
				case MouseActions.X2Click:
				{
					InputSimulator.SimulateClickPress(MouseFlag.XDOWN | absflag, MouseFlag.XUP | absflag, 1, x, y);
					break;
				}
				case MouseActions.X2Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDOWN | absflag, 1, x, y);
					break;
				}
				case MouseActions.X2Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUP | absflag, 1, x, y);
					break;
				}
				default:
				{
					throw new ArgumentException("Invalid action.", "action");
				}
			}
		}

		public virtual void LeftClick(Coordinate point)
		{
			Click(MouseActions.LeftClick, point);
		}

		public virtual void RightClick(Coordinate point)
		{
			Click(MouseActions.RightClick, point);
		}

		public virtual void MiddleClick(Coordinate point)
		{
			Click(MouseActions.MiddleClick, point);
		}

		public virtual void DragDrop(Coordinate from, Coordinate to)
		{
			Click(MouseActions.LeftDown, from);
			WaitRandom(150);
			Click(MouseActions.LeftUp, to);
		}

		public virtual void DoubleClick(Coordinate point)
		{
			Click(MouseActions.LeftClick, point);
			WaitRandom(100);
			Click(MouseActions.LeftClick, point);
		}

#endregion

		#region KeyboardMethods

		public virtual void WinKeyDown()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyDown(VirtualKeyCode.LWIN);
		}

		public virtual void WinKeyUp()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyUp(VirtualKeyCode.LWIN);
		}

		public virtual void KeySend(string keys)
		{
			ActivateIfNeeded();
			SendKeys.SendWait(keys);
		}

		public virtual void KeySendAndWait(string keys)
		{
			KeySend(keys);
			WaitRandom(DefaultTimerKeyWait);
		}

		public virtual void KeySendAndWait(string keys, int msec)
		{
			KeySend(keys);
			Wait(msec);
		}

		#endregion

		#region Properties

		public virtual double TimeMultiplier
		{
			get { return pTimeMultiplier; }
			set { pTimeMultiplier = value; }
		}

		public virtual int Width
		{
			get { return pWidth; }
			set { pWidth = value; }
		}

		public virtual int Height
		{
			get { return pHeight; }
			set { pHeight = value; }
		}

		public virtual int X
		{
			get { return pX; }
			set { pX = value; }
		}

		public virtual int Y
		{
			get { return pY; }
			set { pY = value; }
		}

		public virtual string Title
		{
			get { return pTitle; }
			set { pTitle = value; }
		}

		public virtual WindowHandle Handle
		{
			get { return pHandle; }
			set { pHandle = value; }
		}

		#endregion;
	}
}
