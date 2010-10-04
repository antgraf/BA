using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WindowsInput;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

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
		private const int WHEEL_DELTA = 120;

		private static Random pRandom = new Random();

		private double pAllowedColorDeviation = 0.0;
		private double pAllowedImageDeviation = 0.0;
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

		public bool CompareColors(Color color1, Color color2)
		{
			if(pAllowedColorDeviation == 0.0)
			{
				return color1.ToArgb() == color2.ToArgb();
			}

			int diffr = Math.Abs((int)color1.R - (int)color2.R);
			int diffg = Math.Abs((int)color1.G - (int)color2.G);
			int diffb = Math.Abs((int)color1.B - (int)color2.B);
			double diff = Math.Sqrt(diffr * diffr + diffg * diffg + diffb * diffb) / Math.Sqrt(255 * 255 * 3);
			return diff <= pAllowedColorDeviation;
		}

		protected double Radius(Coordinate center, Coordinate radiusPoint)
		{
			Point centerpt = center.ToAbsolute(this);
			Point radiuspt = radiusPoint.ToAbsolute(this);
			int diffx = centerpt.X - radiuspt.X;
			int diffy = centerpt.Y - radiuspt.Y;
			return Math.Sqrt(diffx * diffx + diffy * diffy);
		}

		protected bool IsInRadius(Point point, Point center, double radius)
		{
			int diffx = center.X - point.X;
			int diffy = center.Y - point.Y;
			double distance = Math.Sqrt(diffx * diffx + diffy * diffy);
			return distance <= radius;
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

		public virtual void Move(Coordinate position)
		{
			Point pt = position.ToAbsolute(Desktop.Primary);
			WinAPI.SetWindowPos(Handle.Handle, IntPtr.Zero, pt.X, pt.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
		}

		public virtual void Resize(Coordinate rightBottom)
		{
			Point pt = rightBottom.ToAbsolute(Desktop.Primary);
			WinAPI.SetWindowPos(Handle.Handle, IntPtr.Zero, 0, 0, pt.X, pt.Y, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOZORDER);
		}

		public virtual void Minimize()
		{
			WINDOWPLACEMENT wp = WINDOWPLACEMENT.Default;
			WinAPI.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Minimize;
			WinAPI.SetWindowPlacement(Handle.Handle, ref wp);
		}

		public virtual void Maximize()
		{
			WINDOWPLACEMENT wp = WINDOWPLACEMENT.Default;
			WinAPI.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Maximize;
			WinAPI.SetWindowPlacement(Handle.Handle, ref wp);
		}

		public virtual void Normalize()
		{
			WINDOWPLACEMENT wp = WINDOWPLACEMENT.Default;
			WinAPI.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Normal;
			WinAPI.SetWindowPlacement(Handle.Handle, ref wp);
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
					InputSimulator.SimulateClickPress(MouseFlag.XDOWN | absflag, MouseFlag.XUP | absflag, 1, x, y);
					break;
				}
				case MouseActions.X1Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDOWN | absflag, 1, x, y);
					break;
				}
				case MouseActions.X1Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUP | absflag, 1, x, y);
					break;
				}
				case MouseActions.X2Click:
				{
					InputSimulator.SimulateClickPress(MouseFlag.XDOWN | absflag, MouseFlag.XUP | absflag, 2, x, y);
					break;
				}
				case MouseActions.X2Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDOWN | absflag, 2, x, y);
					break;
				}
				case MouseActions.X2Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUP | absflag, 2, x, y);
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

		public virtual void WheelUp(int ticks)
		{
			InputSimulator.SimulateClick(MouseFlag.WHEEL, ticks * WHEEL_DELTA);
		}

		public virtual void WheelDown(int ticks)
		{
			InputSimulator.SimulateClick(MouseFlag.WHEEL, ticks * -WHEEL_DELTA);
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

		#region FeedbackMethods

		public Color GetPixelColor(Coordinate pixel)
		{
			ActivateIfNeeded();
			Point pt = pixel.ToAbsolute(this);
			IntPtr hdc = WinAPI.GetDC(IntPtr.Zero);
			uint colorint = WinAPI.GetPixel(hdc, pt.X, pt.Y);
			WinAPI.ReleaseDC(IntPtr.Zero, hdc);
			Color color = Color.FromArgb((int)(colorint & 0x000000FF),
				(int)(colorint & 0x0000FF00) >> 8,
				(int)(colorint & 0x00FF0000) >> 16);
			return color;
		}

		public Bitmap Screenshot()
		{
			return Screenshot(new Coordinate(CoordinateType.Relative, new Point() { X = 0, Y = 0 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = Width, Y = Height }));
		}

		public Bitmap Screenshot(Coordinate topLeft, Coordinate bottomRight)
		{
			ActivateIfNeeded();
			Point pt1 = topLeft.ToAbsolute(this);
			Point pt2 = bottomRight.ToAbsolute(this);
			Bitmap screenshot = new Bitmap(pt2.X - pt1.X, pt2.Y - pt1.Y, PixelFormat.Format32bppArgb);
			using(Graphics gdi = Graphics.FromImage(screenshot))
			{
				gdi.CopyFromScreen(pt1.X, pt1.Y, 0, 0, new Size(pt2.X - pt1.X, pt2.Y - pt1.Y), CopyPixelOperation.SourceCopy);
			}
			return screenshot;
		}

		public Coordinate FindColor(Color color)
		{
			return FindColorInRectangle(color,
				new Coordinate(CoordinateType.Relative, new Point() { X = 0, Y = 0 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = Width, Y = Height }));
		}

		public Coordinate FindColorInRectangle(Color color, Coordinate topLeft, Coordinate bottomRight)
		{
			ActivateIfNeeded();
			Bitmap bmp = Screenshot(topLeft, bottomRight);
			for(int y = 0; y < bmp.Height; y++)
				for(int x = 0; x < bmp.Width; x++)
				{
					if(CompareColors(color, bmp.GetPixel(x, y)))
					{
						Point origin = topLeft.ToAbsolute(this);
						return new Coordinate(CoordinateType.Absolute, new Point()
							{ X = origin.X + x, Y = origin.Y + y });
					}
				}
			return null;
		}

		public Coordinate FindColorInCircle(Color color, Coordinate center, Coordinate radiusPoint)
		{
			ActivateIfNeeded();

			Point centerpt = center.ToAbsolute(this);
			Point radiuspt = radiusPoint.ToAbsolute(this);
			int diffx = centerpt.X - radiuspt.X;
			int diffy = centerpt.Y - radiuspt.Y;
			double radius = Radius(center, radiusPoint);
			int left = (int)(centerpt.X - radius);
			left = left < 0 ? 0 : left;
			int top = (int)(centerpt.Y - radius);
			top = top < 0 ? 0 : top;
			int right = (int)(centerpt.X + radius);
			right = right >= Width ? Width - 1 : right;
			int bottom = (int)(centerpt.Y + radius);
			bottom = bottom >= Height ? Height - 1 : bottom;

			Bitmap bmp = Screenshot(
				new Coordinate(CoordinateType.Absolute, new Point() { X = left, Y = top }),
				new Coordinate(CoordinateType.Absolute, new Point() { X = right, Y = bottom }));
			for(int y = 0; y < bmp.Height; y++)
				for(int x = 0; x < bmp.Width; x++)
				{
					if(CompareColors(color, bmp.GetPixel(x, y)))
					{
						if(IsInRadius(new Point() { X = x + left, Y = y + top }, centerpt, radius))
						{
							return new Coordinate(CoordinateType.Absolute, new Point() { X = left + x, Y = top + y });
						}
					}
				}
			return null;
		}

		public bool CompareImagesExactly(Bitmap baseline, Bitmap image)
		{
			return CompareImagesExactly(baseline, image, null);
		}

		protected bool CompareImagesExactly(Bitmap baseline, Bitmap image, Point? fragment)
		{
			if((fragment == null && (baseline.Size.Width != image.Size.Width || baseline.Size.Height != image.Size.Height)) ||
				(fragment != null && (baseline.Size.Width < image.Size.Width + fragment.Value.X ||
					baseline.Size.Height < image.Size.Height + fragment.Value.Y)))
			{
				return false;
			}
			int startx = fragment == null ? 0 : fragment.Value.X;
			int starty = fragment == null ? 0 : fragment.Value.Y;
			int i = 0; //debug
			for(int y = 0; y < image.Height; y++)
				for(int x = 0; x < image.Width; x++)
				{
					i++; // debug
					if(baseline.GetPixel(x + startx, y + starty).ToArgb() != image.GetPixel(x, y).ToArgb())
					{
						return false;
					}
				}
			return true;
		}

		public bool CompareImagesWithColorDeviation(Bitmap baseline, Bitmap image)
		{
			return CompareImagesWithColorDeviation(baseline, image, null);
		}

		protected bool CompareImagesWithColorDeviation(Bitmap baseline, Bitmap image, Point? fragment)
		{
			if((fragment == null && (baseline.Size.Width != image.Size.Width || baseline.Size.Height != image.Size.Height)) ||
				(fragment != null && (baseline.Size.Width < image.Size.Width + fragment.Value.X ||
					baseline.Size.Height < image.Size.Height + fragment.Value.Y)))
			{
				return false;
			}
			int startx = fragment == null ? 0 : fragment.Value.X;
			int starty = fragment == null ? 0 : fragment.Value.Y;
			for(int y = 0; y < image.Height; y++)
				for(int x = 0; x < image.Width; x++)
				{
					if(!CompareColors(baseline.GetPixel(x + startx, y + starty), image.GetPixel(x, y)))
					{
						return false;
					}
				}
			return true;
		}

		public bool CompareImages(Bitmap baseline, Bitmap image)
		{
			return CompareImages(baseline, image, null);
		}

		protected bool CompareImages(Bitmap baseline, Bitmap image, Point? fragment)
		{
			if((fragment == null && (baseline.Size.Width != image.Size.Width || baseline.Size.Height != image.Size.Height)) ||
				(fragment != null && (baseline.Size.Width < image.Size.Width + fragment.Value.X ||
					baseline.Size.Height < image.Size.Height + fragment.Value.Y)))
			{
				return false;
			}
			int startx = fragment == null ? 0 : fragment.Value.X;
			int starty = fragment == null ? 0 : fragment.Value.Y;
			int diff = 0;
			for(int y = 0; y < image.Height; y++)
				for(int x = 0; x < image.Width; x++)
				{
					if(baseline.GetPixel(x + startx, y + starty).ToArgb() != image.GetPixel(x, y).ToArgb())
					{
						if(((double)++diff) / (baseline.Size.Width * baseline.Size.Height) > pAllowedImageDeviation)
						{
							return false;
						}
					}
				}
			return true;
		}

		public Coordinate FindImageExactly(Bitmap image, Bitmap fragment)
		{
			if(fragment.Size.Width > image.Size.Width || fragment.Size.Height > image.Size.Height)
			{
				return null;
			}
			for(int y = 0; y < image.Height - fragment.Height; y++)
				for(int x = 0; x < image.Width - fragment.Width; x++)
				{
					Point pt = new Point() { X = x, Y = y };
					if(CompareImagesExactly(image, fragment, pt))
					{
						return new Coordinate(CoordinateType.Relative, pt);
					}
				}
			return null;
		}

		public Coordinate FindImageWithColorDeviation(Bitmap image, Bitmap fragment)
		{
			if(fragment.Size.Width > image.Size.Width || fragment.Size.Height > image.Size.Height)
			{
				return null;
			}
			for(int y = 0; y < image.Height - fragment.Height; y++)
				for(int x = 0; x < image.Width - fragment.Width; x++)
				{
					Point pt = new Point() { X = x, Y = y };
					if(CompareImagesWithColorDeviation(image, fragment, pt))
					{
						return new Coordinate(CoordinateType.Relative, pt);
					}
				}
			return null;
		}

		public Coordinate FindImage(Bitmap image, Bitmap fragment)
		{
			if(fragment.Size.Width > image.Size.Width || fragment.Size.Height > image.Size.Height)
			{
				return null;
			}
			for(int y = 0; y < image.Height - fragment.Height; y++)
				for(int x = 0; x < image.Width - fragment.Width; x++)
				{
					Point pt = new Point() { X = x, Y = y };
					if(CompareImages(image, fragment, pt))
					{
						return new Coordinate(CoordinateType.Relative, pt);
					}
				}
			return null;
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

		public double AllowedColorDeviation
		{
			get { return pAllowedColorDeviation; }
			set { pAllowedColorDeviation = value; }
		}

		public double AllowedImageDeviation
		{
			get { return pAllowedImageDeviation; }
			set { pAllowedImageDeviation = value; }
		}

		#endregion;
	}
}
