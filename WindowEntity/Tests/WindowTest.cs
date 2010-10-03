using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class WindowTest
	{
		private List<Process> pProcessesToClean = new List<Process>();

		[Test]
		public void Create()
		{
			Window w = new Window();
			Assert.NotNull(w);
		}

		[Test]
		public void WindowHandle()
		{
			WindowHandle wh1 = new WindowHandle();
			Assert.NotNull(wh1);
			WindowHandle wh2 = new WindowHandle();
			Assert.NotNull(wh2);
			WindowHandle wh3 = new WindowHandle();
			Assert.NotNull(wh3);
			IntPtr ptr1 = new IntPtr(2222);
			IntPtr ptr2 = new IntPtr(3333);
			wh1.Handle = ptr1;
			wh2.Handle = ptr1;
			wh3.Handle = ptr2;
			Assert.True(wh1 == wh2);
			Assert.False(wh1 == wh3);
			Assert.False(wh1.Equals(wh3));
		}

		[Test]
		public void ReadWriteProperties()
		{
			int height = 123;
			int width = 456;
			int x = 789;
			int y = 246;
			string title = "AbCdEfGh!!";
			WindowHandle handle = new WindowHandle() { Handle = new IntPtr(1234) };
			Window w = new Window();
			w.Height = height;
			w.Width = width;
			w.X = x;
			w.Y = y;
			w.Title = title;
			w.Handle = handle;
			Assert.AreEqual(w.Height, height);
			Assert.AreEqual(w.Width, width);
			Assert.AreEqual(w.X, x);
			Assert.AreEqual(w.Y, y);
			Assert.AreEqual(w.Title, title);
			Assert.AreEqual(w.Handle, handle);
		}

		[Test]
		public void Click()
		{
			// open 2 windows
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Process obstacle = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(obstacle);
			pProcessesToClean.Add(obstacle);
			Window obstacle_window = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(obstacle_window);
			// click first
			w.Click(MouseActions.LeftClick,
				new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 }));
			Window.WaitGlobal(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "RED clicked");
			// click second
			obstacle_window.Click(MouseActions.LeftClick,
				new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 }));
			Window.WaitGlobal(100);
			obstacle_window = WindowsMan.UpdateWindow(obstacle_window);
			Assert.NotNull(obstacle_window);
			Assert.AreEqual(obstacle_window.Title, "RED clicked");
			// clean
			p.Kill();
			obstacle.Kill();
		}

		[Test]
		public void ClickUpDown()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			w.Click(MouseActions.LeftDown,
				new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 }));
			w.WaitRandom(10, 30);
			w.Click(MouseActions.LeftUp,
				new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 }));
			w.WaitRandom(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "RED clicked");
			p.Kill();
		}

		[Test]
		public void LeftClick()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			w.LeftClick(new Coordinate(CoordinateType.Relative, new Point() { X = 200, Y = 100 }));
			w.Wait(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "LABEL Lclicked");
			p.Kill();
		}

		[Test]
		public void RightClick()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			w.RightClick(new Coordinate(CoordinateType.Relative, new Point() { X = 200, Y = 100 }));
			w.Wait(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "LABEL Rclicked");
			p.Kill();
		}

		[Test]
		public void MiddleClick()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			w.MiddleClick(new Coordinate(CoordinateType.Relative, new Point() { X = 200, Y = 100 }));
			w.Wait(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "LABEL Mclicked");
			p.Kill();
		}

		[Test]
		public void DoubleClick()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			w.DoubleClick(new Coordinate(CoordinateType.Relative, new Point() { X = 200, Y = 100 }));
			w.Wait(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "LABEL double-clicked");
			p.Kill();
		}

		[Test]
		public void Keyboard()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Process obstacle = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(obstacle);
			pProcessesToClean.Add(obstacle);
			Window.WaitGlobal(100);
			w.LeftClick(new Coordinate(CoordinateType.Relative, new Point() { X = 250, Y = 200 }));
			w.Wait(100);
			w.KeySendAndWait("+aloha+1");
			w.WaitRandom(100);
			w.KeySendAndWait("{TAB}~");
			w = WindowsMan.UpdateWindow(w);
			Assert.NotNull(w);
			Assert.AreEqual(w.Title, "Aloha!");
			w.KeySendAndWait("%{F4}");
			w = WindowsMan.UpdateWindow(w);
			Assert.Null(w);
		}

		[Test]
		public void Close()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			w.Close();
			Window.WaitGlobal(100);
			w = WindowsMan.UpdateWindow(w);
			Assert.Null(w);
		}

		[Test]
		public void MinimizeMaximize()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			int width = w.Width;
			int height = w.Height;
			w.Maximize();
			w.Wait(500);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreNotEqual(w.Width, width);
			Assert.AreNotEqual(w.Height, height);
			w.Normalize();
			w.Wait(500);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreEqual(w.Width, width);
			Assert.AreEqual(w.Height, height);
			w.Minimize();
			w.Wait(500);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreNotEqual(w.Width, width);
			Assert.AreNotEqual(w.Height, height);
			w.Normalize();
			w.Wait(500);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreEqual(w.Width, width);
			Assert.AreEqual(w.Height, height);
			w.Wait(500);
			w.Close();
		}

		[Test]
		public void Move()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			w.Move(new Coordinate(CoordinateType.Absolute, new Point() { X = 0, Y = 0 }));
			w.Wait(200);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreEqual(0, w.X);
			Assert.AreEqual(0, w.Y);
			w.Close();
		}

		[Test]
		public void Resize()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			w.Resize(new Coordinate(CoordinateType.Absolute, new Point() { X = 500, Y = 500 }));
			w.Wait(200);
			w = WindowsMan.UpdateWindow(w);
			Assert.AreEqual(500, w.Width);
			Assert.AreEqual(500, w.Height);
			w.Close();
		}

		[Test]
		public void Wheel()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			w.LeftClick(new Coordinate(CoordinateType.Relative, new Point() { X = 300, Y = 300 }));
			w.WheelDown(10);
			w.Wait(200);
			w.WheelUp(5);
			w.Wait(200);
			w.Close();
		}

		[TearDown]
		public void CleanUpOnError()
		{
			foreach(Process p in pProcessesToClean)
			{
				if(p != null && !p.HasExited)
				{
					p.Kill();
				}
			}
			pProcessesToClean.Clear();
		}
	}
}
