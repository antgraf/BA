//#define SaveDebugOcrImages

extern alias tessnet2_32;
extern alias tessnet2_64;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Tesseract;
using System.Drawing;
using System.Drawing.Drawing2D;
using BACommon;

namespace WindowEntity
{
	public enum OcrQuality
	{
		Good,
		Normal,
		Bad
	}

	public struct OcrWord
	{
		public string Word;
		public OcrQuality Quality;
	}

	public static class WindowsMan
	{
		private const int pDefaultWaitTimeout = 2;
		private const int pDefaultCheckPeriod = 0;
		private const int pDefaultRetryAttempts = 3;
		private const double pDefaultZoomFactor = 3.0;
		private const int pSecond = 1000;

// ReSharper disable UnaccessedField.Local
		private static Ocr pOcr = null;
// ReSharper restore UnaccessedField.Local
		private static object pTesseract = null;
		private static bool pTesseractInitialized = false;
		private static readonly List<Window> pRegisteredWindows = new List<Window>();

		private static void InitTesseract()
		{
			if(!pTesseractInitialized)
			{
				if(Globals.x64)
				{
					InitTesseract64();
				}
				else
				{
					InitTesseract32();
				}
				pTesseractInitialized = true;
			}
		}

		private static void InitTesseract32()
		{
			pOcr = new Ocr32();
			pTesseract = new tessnet2_32::tessnet2.Tesseract();
			((tessnet2_32::tessnet2.Tesseract)pTesseract).Init(null, "eng", false);
		}

		private static void InitTesseract64()
		{
			pOcr = new Ocr64();
			pTesseract = new tessnet2_64::tessnet2.Tesseract();
			((tessnet2_64::tessnet2.Tesseract)pTesseract).Init(null, "eng", false);
		}

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
				foreach(Window registered in pRegisteredWindows.Where(registered => registered.Handle == window.Handle))
				{
					pRegisteredWindows.Remove(registered);
					return true;
				}
				return false;
			}
		}

		public static bool ModifyWindow(Window window)
		{
			lock(pRegisteredWindows)
			{
				foreach(Window registered in pRegisteredWindows.Where(registered => registered.Handle == window.Handle))
				{
					pRegisteredWindows.Remove(registered);
					pRegisteredWindows.Add(window);
					return true;
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
					if(window != null && registered.Handle == window.Handle)
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
				return pRegisteredWindows.Any(registered => registered.Handle == window.Handle);
			}
		}

		public static Window AttachTo(string proccessName)
		{
			lock(pRegisteredWindows)
			{
				Window[] windows = Window.FindWindowsByProcessName(proccessName);
				return windows.FirstOrDefault(RegisterWindow);
			}
		}

		public static Window WaitAndAttachTo(string proccessName, int waitSeconds = pDefaultWaitTimeout, int checkPeriod = pDefaultCheckPeriod, int retryAttempts = pDefaultRetryAttempts)
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

		public static void ResetWindows()
		{
			pRegisteredWindows.Clear();
		}

		public static Process RunProcess(string path)
		{
			return Process.Start(path);
		}

		public static OcrWord[] RecognizeText(Bitmap image)
		{
			InitTesseract();
			return Globals.x64 ? RecognizeText64(image) : RecognizeText32(image);
		}

		private static OcrWord[] RecognizeText32(Bitmap image)
		{
#if SaveDebugOcrImages
			((tessnet2_32::tessnet2.Tesseract)pTesseract).GetThresholdedImage(image, Rectangle.Empty).Save(Guid.NewGuid().ToString() + ".bmp");
#endif
			List<tessnet2_32::tessnet2.Word> words = Ocr32.DoOcrNormal(image, "eng");
			if(words == null)
			{
				return null;
			}
			List<OcrWord> ret = new List<OcrWord>();
			foreach(tessnet2_32::tessnet2.Word word in words)
			{
				OcrQuality quality;
				if(word.Confidence < 80)
				{
					quality = OcrQuality.Good;
				}
				else if(word.Confidence < 160)
				{
					quality = OcrQuality.Normal;
				}
				else
				{
					quality = OcrQuality.Bad;
				}
				OcrWord newword = new OcrWord() { Word = word.Text, Quality = quality };
				ret.Add(newword);
			}
			return ret.ToArray();
		}

		private static OcrWord[] RecognizeText64(Bitmap image)
		{
#if SaveDebugOcrImages
			((tessnet2_64::tessnet2.Tesseract)pTesseract).GetThresholdedImage(image, Rectangle.Empty).Save(Guid.NewGuid().ToString() + ".bmp");
#endif
			List<tessnet2_64::tessnet2.Word> words = Ocr64.DoOcrNormal(image, "eng");
			if(words == null)
			{
				return null;
			}
			List<OcrWord> ret = new List<OcrWord>();
			foreach(tessnet2_64::tessnet2.Word word in words)
			{
				OcrQuality quality;
				if(word.Confidence < 80)
				{
					quality = OcrQuality.Good;
				}
				else if(word.Confidence < 160)
				{
					quality = OcrQuality.Normal;
				}
				else
				{
					quality = OcrQuality.Bad;
				}
				OcrWord newword = new OcrWord() { Word = word.Text, Quality = quality };
				ret.Add(newword);
			}
			return ret.ToArray();
		}

		public static OcrWord[] RecognizeTextWithZoom(Bitmap image, double zoom = pDefaultZoomFactor)
		{
			OcrWord[] words;
			using(Bitmap result = new Bitmap((int)(image.Width * zoom), (int)(image.Height * zoom)))
			{
				using(Graphics gdi = Graphics.FromImage(result))
				{
					gdi.InterpolationMode = InterpolationMode.HighQualityBicubic;
					gdi.DrawImage(image, 0, 0, result.Width, result.Height);
				}
				words = RecognizeText(result);
			}
			return words;
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
	}
}
