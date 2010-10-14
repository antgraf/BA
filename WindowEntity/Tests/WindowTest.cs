extern alias tessnet2_32;
extern alias tessnet2_64;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;
using BACommon;
using System.Drawing.Imaging;
using Tesseract;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class WindowTest
	{
		private const string pTempDirectory = @"C:\Temp";
		private const string pTestImagePath = @"..\TessNet2\test\text.png";
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

		[Test]
		public void GetPixelColor()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Assert.AreEqual(Color.Red.ToArgb(), w.GetPixelColor(new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 })).ToArgb());
			w.Close();
		}

		[Test]
		public void Screenshot()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				//bmp.Save(FileUtils.CombineWinPath(pTempDirectory, FileUtils.MakeValidFileName(DateTime.Now.ToString()) + ".png"), ImageFormat.Png);
			}
			w.Close();
		}

		[Test]
		public void FindColor()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Coordinate coord = w.FindColor(Color.Red);
			Point pt = coord.ToRelative(w);
			Assert.AreEqual(97, pt.X);
			Assert.AreEqual(197, pt.Y);
			w.Close();
		}

		[Test]
		public void FindColorInRectangle()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Coordinate coord = w.FindColorInRectangle(Color.Red,
				new Coordinate(CoordinateType.Relative, new Point() { X = 50, Y = 50 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = 250, Y = 250 }));
			Point pt = coord.ToRelative(w);
			Assert.AreEqual(97, pt.X);
			Assert.AreEqual(197, pt.Y);
			w.Close();
		}

		[Test]
		public void FindColorInCircle()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			Coordinate coord = w.FindColorInCircle(Color.Red,
				new Coordinate(CoordinateType.Relative, new Point() { X = 100, Y = 200 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = 110, Y = 200 }));
			Point pt = coord.ToRelative(w);
			Assert.AreEqual(97, pt.X);
			Assert.AreEqual(197, pt.Y);
			// no red pixels
			coord = w.FindColorInCircle(Color.Red,
				new Coordinate(CoordinateType.Relative, new Point() { X = 130, Y = 200 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = 110, Y = 200 }));
			Assert.Null(coord);
			w.Close();
		}

		[Test]
		public void CompareImagesExactly()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				Assert.True(w.CompareImagesExactly(bmp, bmp));
				using(Bitmap newbmp = new Bitmap(bmp))
				{
					Assert.NotNull(newbmp);
					newbmp.SetPixel(0, 0, Color.Magenta);
					Assert.False(w.CompareImagesExactly(bmp, newbmp));
				}
			}
			w.Close();
		}

		[Test]
		public void CompareImagesWithColorDeviation()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				Assert.True(w.CompareImagesWithColorDeviation(bmp, bmp));
			}
			w.Close();
		}

		[Test]
		public void CompareImages()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				Assert.True(w.CompareImages(bmp, bmp));
			}
			w.Close();
		}

		[Test]
		public void FindImageExactly()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				using(Bitmap fragment = w.Screenshot(
					new Coordinate(CoordinateType.Relative, new Point() { X = 300, Y = 300 }),
					new Coordinate(CoordinateType.Relative, new Point() { X = 350, Y = 350 })))
				{
					Assert.NotNull(fragment);
					Coordinate found = w.FindImageExactly(bmp, fragment);
					Assert.NotNull(found);
					Point pt = found.ToRelative(w);
					Assert.AreEqual(300, pt.X);
					Assert.AreEqual(300, pt.Y);
				}
			}
			w.Close();
		}

		[Test]
		public void FindImageWithColorDeviation()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				using(Bitmap fragment = w.Screenshot(
					new Coordinate(CoordinateType.Relative, new Point() { X = 300, Y = 300 }),
					new Coordinate(CoordinateType.Relative, new Point() { X = 350, Y = 350 })))
				{
					Assert.NotNull(fragment);
					Coordinate found = w.FindImageWithColorDeviation(bmp, fragment);
					Assert.NotNull(found);
					Point pt = found.ToRelative(w);
					Assert.AreEqual(300, pt.X);
					Assert.AreEqual(300, pt.Y);
				}
			}
			w.Close();
		}

		[Test]
		public void FindImage()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			using(Bitmap bmp = w.Screenshot())
			{
				Assert.NotNull(bmp);
				using(Bitmap fragment = w.Screenshot(
					new Coordinate(CoordinateType.Relative, new Point() { X = 300, Y = 300 }),
					new Coordinate(CoordinateType.Relative, new Point() { X = 350, Y = 350 })))
				{
					Assert.NotNull(fragment);
					Coordinate found = w.FindImage(bmp, fragment);
					Assert.NotNull(found);
					Point pt = found.ToRelative(w);
					Assert.AreEqual(300, pt.X);
					Assert.AreEqual(300, pt.Y);
				}
			}
			w.Close();
		}

		[Test]
		public void Tesseract()
		{
			if(Globals.x64)
			{
				Tesseract_64();
			}
			else
			{
				Tesseract_32();
			}
		}

		private void Tesseract_32()
		{
			Ocr32 ocr = new Ocr32();
			Assert.NotNull(ocr);
			using(Bitmap bmp = new Bitmap(pTestImagePath))
			{
				Assert.NotNull(bmp);
				tessnet2_32::tessnet2.Tesseract tessocr = new tessnet2_32::tessnet2.Tesseract();
				Assert.NotNull(tessocr);
				tessocr.Init(null, "eng", false);
				tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save(FileUtils.CombineWinPath(pTempDirectory, Guid.NewGuid().ToString()) + ".bmp");
				ocr.DoOCRMultiThred(bmp, "eng");
				List<tessnet2_32::tessnet2.Word> words = ocr.DoOCRNormal(bmp, "eng");
				Assert.NotNull(words);
			}
		}

		private void Tesseract_64()
		{
			Ocr64 ocr = new Ocr64();
			Assert.NotNull(ocr);
			using(Bitmap bmp = new Bitmap(pTestImagePath))
			{
				Assert.NotNull(bmp);
				tessnet2_64::tessnet2.Tesseract tessocr = new tessnet2_64::tessnet2.Tesseract();
				Assert.NotNull(tessocr);
				tessocr.Init(null, "eng", false);
				tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save(FileUtils.CombineWinPath(pTempDirectory, Guid.NewGuid().ToString()) + ".bmp");
				ocr.DoOCRMultiThred(bmp, "eng");
				List<tessnet2_64::tessnet2.Word> words = ocr.DoOCRNormal(bmp, "eng");
				Assert.NotNull(words);
			}
		}

		[Test]
		public void RecognizeText()
		{
			WindowsMan.ResetWindows();
			Process p = WindowsMan.RunProcess(Definitions.PathToSampleApp);
			Assert.NotNull(p);
			pProcessesToClean.Add(p);
			Window w = WindowsMan.WaitAndAttachTo("SampleWindow", 1, 1, 5);
			Assert.NotNull(w);
			OcrWord[] words = w.RecognizeText(
				new Coordinate(CoordinateType.Relative, new Point() { X = 350, Y = 100 }),
				new Coordinate(CoordinateType.Relative, new Point() { X = 430, Y = 120 }));
			Assert.NotNull(words);
			Assert.AreEqual(2, words.Length);
			Assert.AreEqual("Large", words[0].Word);
			Assert.AreNotEqual(OcrQuality.Bad, words[0].Quality);
			Assert.AreEqual("text", words[1].Word);
			Assert.AreNotEqual(OcrQuality.Bad, words[1].Quality);
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
