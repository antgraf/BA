using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace WindowEntity.Tests
{
	class WindowsManTest
	{
		[Test]
		public void Ok()
		{
			Assert.NotNull(WindowsMan.RegisteredWindows);
		}

		[Test]
		public void Registration()
		{
			Window w1 = new Window();
			w1.Handle = new WindowHandle() { Handle = new IntPtr(2222) };
			Assert.NotNull(w1);
			Window w2 = new Window();
			Assert.NotNull(w2);
			w2.Title = "BEFORE";
			w2.Handle = new WindowHandle() { Handle = new IntPtr(3333) };
			Window w3 = new Window();
			Assert.NotNull(w3);
			w3.Title = "AFTER";
			w3.Handle = new WindowHandle() { Handle = new IntPtr(3333) };
			Assert.True(WindowsMan.RegisterWindow(w1));
			Assert.False(WindowsMan.RegisterWindow(w1));
			Assert.True(WindowsMan.RegisterWindow(w2));
			Assert.False(WindowsMan.RegisterWindow(w2));
			Assert.AreEqual(WindowsMan.RegisteredWindows.Length, 2);
			Assert.True(WindowsMan.UnRegisterWindow(w1));
			Assert.False(WindowsMan.UnRegisterWindow(w1));
			Assert.AreEqual(WindowsMan.RegisteredWindows.Length, 1);
			Assert.AreEqual(WindowsMan.RegisteredWindows[0].Title, "BEFORE");
			Assert.True(WindowsMan.ModifyWindow(w3));
			Assert.AreEqual(WindowsMan.RegisteredWindows.Length, 1);
			Assert.AreEqual(WindowsMan.RegisteredWindows[0].Title, "AFTER");
		}

		[Test]
		public void StartProcessAndAttach()
		{
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(w.Width, 640);
			Assert.AreEqual(w.Height, 480);
			Assert.AreEqual(w.Title, "Window Title 111");
			p.Kill();
		}
	}
}
