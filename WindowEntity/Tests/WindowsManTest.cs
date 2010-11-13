using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;

namespace WindowEntity.Tests
{
	class WindowsManTest
	{
		private const string pTestImagePath = @"..\WindowEntity\Tests\res\baseline_text.png";

		[Test]
		public void Ok()
		{
			Assert.NotNull(WindowsMan.RegisteredWindows);
		}

		[Test]
		public void Registration()
		{
			WindowsMan.ResetWindows();
			Assert.AreEqual(0, WindowsMan.RegisteredWindows.Length);
			Window w1 = new Window
			            	{
			            		Handle = new WindowHandle() {Handle = new IntPtr(2222)}
			            	};
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
			Assert.AreEqual(2, WindowsMan.RegisteredWindows.Length);
			Assert.True(WindowsMan.UnRegisterWindow(w1));
			Assert.False(WindowsMan.UnRegisterWindow(w1));
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			Assert.AreEqual("BEFORE", WindowsMan.RegisteredWindows[0].Title);
			Assert.True(WindowsMan.ModifyWindow(w3));
			Assert.AreEqual(1, WindowsMan.RegisteredWindows.Length);
			Assert.AreEqual("AFTER", WindowsMan.RegisteredWindows[0].Title);
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

		[Test]
		public void RecognizeText()
		{
			using(Bitmap bmp = new Bitmap(pTestImagePath))
			{
				OcrWord[] words = WindowsMan.RecognizeText(bmp);
				Assert.NotNull(words);
				Assert.AreEqual(6, words.Length);
				Assert.AreEqual("Some", words[0].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[0].Quality);
				Assert.AreEqual("text", words[1].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[1].Quality);
				Assert.AreEqual("with", words[2].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[2].Quality);
				Assert.AreEqual("font", words[3].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[3].Quality);
				Assert.AreEqual("Calibri", words[4].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[4].Quality);
				Assert.AreEqual("26.", words[5].Word);
				Assert.AreNotEqual(OcrQuality.Bad, words[5].Quality);
			}
		}
	}
}
