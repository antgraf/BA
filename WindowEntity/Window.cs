using System;
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
			// Return true if the fields match:
			return a.Handle == b.Handle;
		}

		public static bool operator !=(WindowHandle a, WindowHandle b)
		{
			return !(a == b);
		}

		public override bool Equals(Object obj)
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
		public const double NormalizationCoefficientForColorDeviation = 441.6729559300637;
		public const double MinColorDeviation = 0.0022641187027044;

		private const int maxTitleLength = 512;
		private const int defaultTimerDeviationPercent = 15;
		private const int defaultTimerKeyWait = 300;
		private const int wheelDelta = 120;

		private static readonly Random pRandom = new Random();

		private double pAllowedColorDeviation = 0.0;
		private double pAllowedImageNoisePixels = 0.0;
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
			if(GetType().IsSubclassOf(typeof(Desktop)) || GetType() == typeof(Desktop))
			{
				return false;
			}
			return WinApi.GetForegroundWindow() != pHandle.Handle;
		}

		protected virtual bool ActivateIfNeeded()
		{
			bool needed = IsActivationNeeded();
			if(needed)
			{
				WinApi.SetForegroundWindow(pHandle.Handle);
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

			int diffr = Math.Abs(color1.R - color2.R);
			int diffg = Math.Abs(color1.G - color2.G);
			int diffb = Math.Abs(color1.B - color2.B);
			double diff = Math.Sqrt(diffr * diffr + diffg * diffg + diffb * diffb) / Math.Sqrt(255 * 255 * 3);
			return diff <= pAllowedColorDeviation;
		}

		public static Window GetWindowAtCursor()
		{
			WindowPoint pt = new WindowPoint(Cursor.Position.X, Cursor.Position.Y);
			IntPtr hWnd = WinApi.WindowFromPoint(pt);
			IntPtr parent = WinApi.GetAncestor(hWnd, GetAncestorFlags.GetRoot);
			return FromHandle(parent);
		}

		protected double Radius(Coordinate center, Coordinate radiusPoint)
		{
			Point centerpt = center.ToAbsolute(this);
			Point radiuspt = radiusPoint.ToAbsolute(this);
			int diffx = centerpt.X - radiuspt.X;
			int diffy = centerpt.Y - radiuspt.Y;
			return Math.Sqrt(diffx * diffx + diffy * diffy);
		}

		protected static bool IsInRadius(Point point, Point center, double radius)
		{
			int diffx = center.X - point.X;
			int diffy = center.Y - point.Y;
			double distance = Math.Sqrt(diffx * diffx + diffy * diffy);
			return distance <= radius;
		}

		public static Window[] FindWindowsByProcessName(string process)
		{
			Process[] procs = Process.GetProcessesByName(process);
			IntPtr hWnd = IntPtr.Zero;
			return procs.Where(proc => (hWnd = proc.MainWindowHandle) != IntPtr.Zero)
				.Select(proc => FromHandle(hWnd))
				.Where(window => window != null)
				.ToArray();
		}

		public static Window FromHandle(IntPtr hwnd)
		{
			WindowRect rect = new WindowRect();
			if(hwnd == IntPtr.Zero || !WinApi.GetWindowRect(hwnd, ref rect))
			{
				return null;
			}
			Window window = new Window
			                	{
			                		Handle = new WindowHandle() {Handle = hwnd},
			                		X = rect.X,
			                		Y = rect.Y,
			                		Width = rect.Width,
			                		Height = rect.Height,
			                		Title = GetTitle(hwnd)
			                	};
			return window;
		}

		public static string GetTitle(IntPtr hwnd)
		{
			StringBuilder title = new StringBuilder(maxTitleLength);
			if(WinApi.GetWindowText(hwnd, title, maxTitleLength) == 0)
			{
				return null;
			}
			return title.ToString();
		}

		public static void WaitGlobal(int msec)
		{
			Thread.Sleep(msec);
		}

		public static void WaitRandomGlobal(int msec, int percentDelta = defaultTimerDeviationPercent)
		{
			//if(percentDelta >= 100) throw new ArgumentException("Percentage deviation should be < 100%", "percentDelta");
			int minmsec = (int)(msec - msec * (percentDelta / 100.0));
			if(minmsec < 0) minmsec = 0;
			int maxmsec = (int)(msec + msec * (percentDelta / 100.0));
			if(maxmsec <= 0) maxmsec = 1;
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

		public virtual void WaitRandom(int msec, int percentDelta = defaultTimerDeviationPercent)
		{
			//if(percentDelta >= 100) throw new ArgumentException("Percentage deviation should be < 100%", "percentDelta");
			int minmsec = (int)(msec - msec * (percentDelta / 100.0));
			if(minmsec < 0) minmsec = 0;
			int maxmsec = (int)(msec + msec * (percentDelta / 100.0));
			if(maxmsec <= 0) maxmsec = 1;
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
			WinApi.SetWindowPos(Handle.Handle, IntPtr.Zero, pt.X, pt.Y, 0, 0, WinApi.SwpNoSize | WinApi.SwpNoZOrder);
		}

		public virtual void Resize(Coordinate rightBottom)
		{
			Point pt = rightBottom.ToAbsolute(Desktop.Primary);
			WinApi.SetWindowPos(Handle.Handle, IntPtr.Zero, 0, 0, pt.X, pt.Y, WinApi.SwpNoMove | WinApi.SwpNoZOrder);
		}

		public virtual void Minimize()
		{
			WindowPlacement wp;
			WinApi.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Minimize;
			WinApi.SetWindowPlacement(Handle.Handle, ref wp);
		}

		public virtual void Maximize()
		{
			WindowPlacement wp;
			WinApi.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Maximize;
			WinApi.SetWindowPlacement(Handle.Handle, ref wp);
		}

		public virtual void Normalize()
		{
			WindowPlacement wp;
			WinApi.GetWindowPlacement(Handle.Handle, out wp);
			wp.ShowCmd = ShowWindowCommand.Normal;
			WinApi.SetWindowPlacement(Handle.Handle, ref wp);
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
			MouseFlag absflag = MouseFlag.Empty;
			if(moveCursor)
			{
				SetCursorPosition(point);
			}
			else
			{
				Point pt = point.ToAbsolute(this);
				x = pt.X;
				y = pt.Y;
				absflag = MouseFlag.Absolute;
			}
			switch(action)
			{
				case MouseActions.LeftClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.LeftDown | absflag, MouseFlag.LeftUp | absflag, x, y);
					break;
				}
				case MouseActions.LeftDown:
				{
					InputSimulator.SimulateClick(MouseFlag.LeftDown | absflag, x, y);
					break;
				}
				case MouseActions.LeftUp:
				{
					InputSimulator.SimulateClick(MouseFlag.LeftUp | absflag, x, y);
					break;
				}
				case MouseActions.MiddleClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.MiddleDown | absflag, MouseFlag.MiddleUp | absflag, x, y);
					break;
				}
				case MouseActions.MiddleDown:
				{
					InputSimulator.SimulateClick(MouseFlag.MiddleDown | absflag, x, y);
					break;
				}
				case MouseActions.MiddleUp:
				{
					InputSimulator.SimulateClick(MouseFlag.MiddleUp | absflag, x, y);
					break;
				}
				case MouseActions.RightClick:
				{
					InputSimulator.SimulateClickPress(MouseFlag.RightDown | absflag, MouseFlag.RightUp | absflag, x, y);
					break;
				}
				case MouseActions.RightDown:
				{
					InputSimulator.SimulateClick(MouseFlag.RightDown | absflag, x, y);
					break;
				}
				case MouseActions.RightUp:
				{
					InputSimulator.SimulateClick(MouseFlag.RightUp | absflag, x, y);
					break;
				}
				case MouseActions.X1Click:
				{
					InputSimulator.SimulateClickPress(MouseFlag.XDown | absflag, MouseFlag.XUp | absflag, 1, x, y);
					break;
				}
				case MouseActions.X1Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDown | absflag, 1, x, y);
					break;
				}
				case MouseActions.X1Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUp | absflag, 1, x, y);
					break;
				}
				case MouseActions.X2Click:
				{
					InputSimulator.SimulateClickPress(MouseFlag.XDown | absflag, MouseFlag.XUp | absflag, 2, x, y);
					break;
				}
				case MouseActions.X2Down:
				{
					InputSimulator.SimulateClick(MouseFlag.XDown | absflag, 2, x, y);
					break;
				}
				case MouseActions.X2Up:
				{
					InputSimulator.SimulateClick(MouseFlag.XUp | absflag, 2, x, y);
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
			WaitRandom(300);
			Click(MouseActions.LeftUp, to);
		}

		public virtual void DragDropRight(Coordinate from, Coordinate to)
		{
			Click(MouseActions.RightDown, from);
			WaitRandom(300);
			Click(MouseActions.RightUp, to);
		}

		public virtual void DoubleClick(Coordinate point)
		{
			Click(MouseActions.LeftClick, point);
			WaitRandom(100);
			Click(MouseActions.LeftClick, point);
		}

		public virtual void WheelUp(int ticks)
		{
			InputSimulator.SimulateClick(MouseFlag.Wheel, ticks * wheelDelta);
		}

		public virtual void WheelDown(int ticks)
		{
			InputSimulator.SimulateClick(MouseFlag.Wheel, ticks * -wheelDelta);
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

		public virtual void CtrlKeyDown()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
		}

		public virtual void CtrlKeyUp()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
		}

		public virtual void AltKeyDown()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyDown(VirtualKeyCode.LMENU);
		}

		public virtual void AltKeyUp()
		{
			ActivateIfNeeded();
			InputSimulator.SimulateKeyUp(VirtualKeyCode.LMENU);
		}

		public virtual void KeySend(string keys)
		{
			ActivateIfNeeded();
			SendKeys.SendWait(keys);
		}

		public virtual void KeySendAndWait(string keys)
		{
			KeySend(keys);
			WaitRandom(defaultTimerKeyWait);
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
			IntPtr hdc = WinApi.GetDC(IntPtr.Zero);
			uint colorint = WinApi.GetPixel(hdc, pt.X, pt.Y);
			WinApi.ReleaseDC(IntPtr.Zero, hdc);
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
			using(Bitmap bmp = Screenshot(topLeft, bottomRight))
			{
				for(int y = 0; y < bmp.Height; y++)
					for(int x = 0; x < bmp.Width; x++)
					{
						if(CompareColors(color, bmp.GetPixel(x, y)))
						{
							Point origin = topLeft.ToAbsolute(this);
							return new Coordinate(CoordinateType.Absolute, new Point() { X = origin.X + x, Y = origin.Y + y });
						}
					}
			}
			return null;
		}

		public Coordinate FindColorInCircle(Color color, Coordinate center, Coordinate radiusPoint)
		{
			ActivateIfNeeded();

			Point centerpt = center.ToAbsolute(this);
			double radius = Radius(center, radiusPoint);
			int left = (int)(centerpt.X - radius);
			left = left < 0 ? 0 : left;
			int top = (int)(centerpt.Y - radius);
			top = top < 0 ? 0 : top;
			int right = (int)(centerpt.X + radius);
			right = right >= Width ? Width - 1 : right;
			int bottom = (int)(centerpt.Y + radius);
			bottom = bottom >= Height ? Height - 1 : bottom;

			using(Bitmap bmp = Screenshot(
				new Coordinate(CoordinateType.Absolute, new Point() { X = left, Y = top }),
				new Coordinate(CoordinateType.Absolute, new Point() { X = right, Y = bottom })))
			{
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
			}
			return null;
		}

		public static bool CompareImagesExactly(Bitmap baseline, Bitmap image)
		{
			return CompareImagesExactly(baseline, image, null);
		}

		protected static bool CompareImagesExactly(Bitmap baseline, Bitmap image, Point? fragment)
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

		public bool CompareImagesWithNoise(Bitmap baseline, Bitmap image)
		{
			return CompareImagesWithNoise(baseline, image, null);
		}

		protected bool CompareImagesWithNoise(Bitmap baseline, Bitmap image, Point? fragment)
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
						if(((double)++diff) / (baseline.Size.Width * baseline.Size.Height) > pAllowedImageNoisePixels)
						{
							return false;
						}
					}
				}
			return true;
		}

		public static Coordinate FindImageExactly(Bitmap image, Bitmap fragment)
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

		public Coordinate FindImageWithNoise(Bitmap image, Bitmap fragment)
		{
			if(fragment.Size.Width > image.Size.Width || fragment.Size.Height > image.Size.Height)
			{
				return null;
			}
			for(int y = 0; y < image.Height - fragment.Height; y++)
				for(int x = 0; x < image.Width - fragment.Width; x++)
				{
					Point pt = new Point() { X = x, Y = y };
					if(CompareImagesWithNoise(image, fragment, pt))
					{
						return new Coordinate(CoordinateType.Relative, pt);
					}
				}
			return null;
		}

		public OcrWord[] RecognizeText(Coordinate topLeft, Coordinate bottomRight)
		{
			ActivateIfNeeded();
			OcrWord[] ret;
			using(Bitmap bmp = Screenshot(topLeft, bottomRight))
			{
				ret = WindowsMan.RecognizeTextWithZoom(bmp);
			}
			return ret;
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

		/// <summary>
		/// normalized by 441.6729559300637 = 0.0 to 1.0
		/// </summary>
		public double AllowedColorDeviation
		{
			get { return pAllowedColorDeviation; }
			set { pAllowedColorDeviation = value; }
		}

		public double AllowedImageNoisePixels
		{
			get { return pAllowedImageNoisePixels; }
			set { pAllowedImageNoisePixels = value; }
		}

		#endregion;
	}
}
