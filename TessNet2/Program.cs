extern alias tessnet2_32;
extern alias tessnet2_64;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using BACommon;

// ReSharper disable CheckNamespace
namespace Tesseract
// ReSharper restore CheckNamespace
{
    class Program
    {
// ReSharper disable UnusedMember.Local
        static void Main()
// ReSharper restore UnusedMember.Local
        {
			if(Globals.x64)
			{
				Main64();
			}
			else
			{
				Main32();
			}
        }

		static void Main32()
		{
			// Code usage sample
			Ocr32 ocr = new Ocr32();
			using(Bitmap bmp = new Bitmap(@"D:\temp\ocr\b1.bmp"))
			{
				tessnet2_32::tessnet2.Tesseract tessocr = new tessnet2_32::tessnet2.Tesseract();
				tessocr.Init(null, "eng", false);
				tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save("c:\\temp\\" + Guid.NewGuid() + ".bmp");
				// Tessdata directory must be in the directory than this exe
				Console.WriteLine("Multithread version");
				ocr.DoOcrMultiThread(bmp, "eng");
				Console.WriteLine("Normal version");
				Ocr32.DoOcrNormal(bmp, "eng");
			}
		}

		static void Main64()
		{
			// Code usage sample
			Ocr64 ocr = new Ocr64();
			using(Bitmap bmp = new Bitmap(@"D:\temp\ocr\b1.bmp"))
			{
				tessnet2_64::tessnet2.Tesseract tessocr = new tessnet2_64::tessnet2.Tesseract();
				tessocr.Init(null, "eng", false);
				tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save("c:\\temp\\" + Guid.NewGuid() + ".bmp");
				// Tessdata directory must be in the directory than this exe
				Console.WriteLine("Multithread version");
				ocr.DoOcrMultiThread(bmp, "eng");
				Console.WriteLine("Normal version");
				Ocr64.DoOcrNormal(bmp, "eng");
			}
		}
	}

	public class Ocr
	{
	}

    public class Ocr32 : Ocr
    {
		public static void DumpResult(List<tessnet2_32::tessnet2.Word> result)
        {
			foreach(tessnet2_32::tessnet2.Word word in result)
                Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
        }

		public static List<tessnet2_32::tessnet2.Word> DoOcrNormal(Bitmap image, string lang)
        {
			tessnet2_32::tessnet2.Tesseract ocr = new tessnet2_32::tessnet2.Tesseract();
            ocr.Init(null, lang, false);
			List<tessnet2_32::tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
            DumpResult(result);
            return result;
        }

        ManualResetEvent mEvent;

        public void DoOcrMultiThread(Bitmap image, string lang)
        {
			tessnet2_32::tessnet2.Tesseract ocr = new tessnet2_32::tessnet2.Tesseract();
            ocr.Init(null, lang, false);
            // If the OcrDone delegate is not null then this'll be the multithreaded version
			ocr.OcrDone = new tessnet2_32::tessnet2.Tesseract.OcrDoneHandler(Finished);
            // For event to work, must use the multithreaded version
// ReSharper disable RedundantDelegateCreation
			ocr.ProgressEvent += new tessnet2_32::tessnet2.Tesseract.ProgressHandler(OcrProgressEvent);
// ReSharper restore RedundantDelegateCreation
            mEvent = new ManualResetEvent(false);
            ocr.DoOCR(image, Rectangle.Empty);
            // Wait here it's finished
            mEvent.WaitOne();
        }

		public void Finished(List<tessnet2_32::tessnet2.Word> result)
        {
            DumpResult(result);
            mEvent.Set();
        }

    	static void  OcrProgressEvent(int percent)
        {
 	        Console.WriteLine("{0}% progression", percent);
        }
    }

	public class Ocr64 : Ocr
	{
		public static void DumpResult(List<tessnet2_64::tessnet2.Word> result)
		{
			foreach(tessnet2_64::tessnet2.Word word in result)
				Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
		}

		public static List<tessnet2_64::tessnet2.Word> DoOcrNormal(Bitmap image, string lang)
		{
			tessnet2_64::tessnet2.Tesseract ocr = new tessnet2_64::tessnet2.Tesseract();
			ocr.Init(null, lang, false);
			List<tessnet2_64::tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
			DumpResult(result);
			return result;
		}

		ManualResetEvent mEvent;

		public void DoOcrMultiThread(Bitmap image, string lang)
		{
			tessnet2_64::tessnet2.Tesseract ocr = new tessnet2_64::tessnet2.Tesseract();
			ocr.Init(null, lang, false);
			// If the OcrDone delegate is not null then this'll be the multithreaded version
			ocr.OcrDone = new tessnet2_64::tessnet2.Tesseract.OcrDoneHandler(Finished);
			// For event to work, must use the multithreaded version
// ReSharper disable RedundantDelegateCreation
			ocr.ProgressEvent += new tessnet2_64::tessnet2.Tesseract.ProgressHandler(OcrProgressEvent);
// ReSharper restore RedundantDelegateCreation
			mEvent = new ManualResetEvent(false);
			ocr.DoOCR(image, Rectangle.Empty);
			// Wait here it's finished
			mEvent.WaitOne();
		}

		public void Finished(List<tessnet2_64::tessnet2.Word> result)
		{
			DumpResult(result);
			mEvent.Set();
		}

		static void OcrProgressEvent(int percent)
		{
			Console.WriteLine("{0}% progression", percent);
		}
	}
}