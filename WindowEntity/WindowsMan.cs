using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace WindowEntity
{
	public static class WindowsMan
	{
		private const int pDefaultWaitTimeout = 2;
		private const int pDefaultCheckPeriod = 0;
		private const int pDefaultRetryAttempts = 3;
		private const int pSecond = 1000;

		private static List<Window> pRegisteredWindows = new List<Window>();

		public static bool RegisterWindow(Window window)
		{
			lock(pRegisteredWindows)
			{
				if(IsRegistered(window))
				{
					return false;
				}
				pRegisteredWindows.Add(window);
				return true;
			}
		}

		public static bool UnRegisterWindow(Window window)
		{
			lock(pRegisteredWindows)
			{
				foreach(Window registered in pRegisteredWindows)
				{
					if(registered.Handle == window.Handle)
					{
						pRegisteredWindows.Remove(registered);
						return true;
					}
				}
				return false;
			}
		}

		public static bool ModifyWindow(Window window)
		{
			lock(pRegisteredWindows)
			{
				foreach(Window registered in pRegisteredWindows)
				{
					if(registered.Handle == window.Handle)
					{
						pRegisteredWindows.Remove(registered);
						pRegisteredWindows.Add(window);
						return true;
					}
				}
				return false;
			}
		}

		public static Window UpdateWindow(Window window)
		{
			lock(pRegisteredWindows)
			{
				foreach(Window registered in pRegisteredWindows)
				{
					if(registered.Handle == window.Handle)
					{
						Window newwindow = Window.FromHandle(window.Handle.Handle);
						if(newwindow == null)
						{
							return null;
						}
						pRegisteredWindows.Remove(registered);
						pRegisteredWindows.Add(newwindow);
						return newwindow;
					}
				}
				return null;
			}
		}

		public static bool IsRegistered(Window window)
		{
			lock(pRegisteredWindows)
			{
				foreach(Window registered in pRegisteredWindows)
				{
					if(registered.Handle == window.Handle)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static Window AttachTo(string proccessName)
		{
			lock(pRegisteredWindows)
			{
				Window[] windows = Window.FindWindowsByProcessName(proccessName);
				foreach(Window window in windows)
				{
					if(RegisterWindow(window))
					{
						return window;
					}
				}
				return null;
			}
		}

		public static Window WaitAndAttachTo(string proccessName)
		{
			return WaitAndAttachTo(proccessName, pDefaultWaitTimeout, pDefaultCheckPeriod, pDefaultRetryAttempts);
		}

		public static Window WaitAndAttachTo(string proccessName, int waitSeconds)
		{
			return WaitAndAttachTo(proccessName, waitSeconds, pDefaultCheckPeriod, pDefaultRetryAttempts);
		}

		public static Window WaitAndAttachTo(string proccessName, int waitSeconds, int checkPeriod)
		{
			return WaitAndAttachTo(proccessName, waitSeconds, checkPeriod, pDefaultRetryAttempts);
		}

		public static Window WaitAndAttachTo(string proccessName, int waitSeconds, int checkPeriod, int retryAttempts)
		{
			lock(pRegisteredWindows)
			{
				if(waitSeconds <= 0) throw new ArgumentException("Waiting timeout should be > 0.", "waitSeconds");
				if(checkPeriod <= 0) checkPeriod = waitSeconds;
				if(retryAttempts < 0) retryAttempts = 0;

				Thread.Sleep(waitSeconds * pSecond);
				Window window = AttachTo(proccessName);
				for(int i = 0; i < retryAttempts && window == null; i++)
				{
					Thread.Sleep(checkPeriod * pSecond);
					window = AttachTo(proccessName);
				}
				return window;
			}
		}

		public static Window[] RegisteredWindows
		{
			get
			{
				lock(pRegisteredWindows)
				{
					return pRegisteredWindows.ToArray();
				}
			}
		}

		public static Process RunProcess(string path)
		{
			return Process.Start(path);
		}
	}
}
