using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logger;
using BACommon;
using WindowEntity;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace PixelScout
{
	class HtmlRecorder
	{
		private const string pLogFolder = "Records";
		private const string pImagesFolderExtension = "_files";
		private const string pLogExtension = ".html";
		private const string pImageExtension = ".png";
		private const string pLogHeader = "<html><body>\r\n";
		private const string pLogFooter = "</html></body>\r\n";
		private const string pLogRecordFormat = "<p><img src=\"file:{1}\"></img>{0}</p>\r\n";
		private const int pScreenshotRadius = 50;

		private FileLogger pLog = null;
		private string pLogFileName = null;
		private string pImagesFolder = null;

		public HtmlRecorder()
		{
			pLogFileName = FileUtils.Relative2AbsolutePath(
				FileUtils.CombineWinPath(pLogFolder,
					FileUtils.MakeValidFileName(DateTime.Now.ToString()) + pLogExtension));
			pLog = new FileLogger(pLogFileName, false);
			pLog.Log(pLogHeader, false);
			pImagesFolder = pLogFileName + pImagesFolderExtension;
			Directory.CreateDirectory(pImagesFolder);
		}

		private string GenerateImageFileName()
		{
			string filename =  FileUtils.MakeValidFileName(Guid.NewGuid().ToString()) + pImageExtension;
			return FileUtils.CombineWinPath(pImagesFolder, filename);
		}

		private string GetRelativeImagePath(string fullPath)
		{
			string path = FileUtils.ExtractFileName(pLogFileName) + pImagesFolderExtension;
			string filename = FileUtils.ExtractFileName(fullPath);
			return FileUtils.CombineWinPath(path, filename);
		}

		private string Screenshot()
		{
			int x1 = Cursor.Position.X - pScreenshotRadius < 0 ? 0 : Cursor.Position.X - pScreenshotRadius;
			int y1 = Cursor.Position.Y - pScreenshotRadius < 0 ? 0 : Cursor.Position.Y - pScreenshotRadius;
			int x2 = Cursor.Position.X + pScreenshotRadius >= Desktop.Primary.Width ? Desktop.Primary.Width : Cursor.Position.X + pScreenshotRadius;
			int y2 = Cursor.Position.Y + pScreenshotRadius >= Desktop.Primary.Height ? Desktop.Primary.Height : Cursor.Position.Y + pScreenshotRadius;
			Bitmap bmp = Desktop.Primary.Screenshot(
				new Coordinate(CoordinateType.Absolute, new WindowEntity.Point() { X = x1, Y = y1}),
				new Coordinate(CoordinateType.Absolute, new WindowEntity.Point() { X = x2, Y = y2}));
			string filename = GenerateImageFileName();
			bmp.Save(filename, ImageFormat.Png);
			return filename;
		}

		private string PointInformation()
		{
			StringBuilder text = new StringBuilder();
			Window window = Window.GetWindowAtCursor();
			Coordinate pt = new Coordinate(CoordinateType.Absolute, new WindowEntity.Point() { X = Cursor.Position.X, Y = Cursor.Position.Y });
			Color color = Desktop.Primary.GetPixelColor(pt);
			if(window != null)
			{
				WindowEntity.Point rel = pt.ToRelative(window);
				StretchedPoint st = pt.ToStretched(window);
				text.Append("<b>Window</b>: ");
				text.AppendFormat("[{0}, {1}] ", window.Width, window.Height);
				text.AppendFormat("\"{0}\"", window.Title);
				text.AppendLine();
				text.Append("<br><b>Color</b>: ");
				text.AppendLine(color.ToString());
				text.Append("<br><b>Coordinate</b>: Absolute: ");
				text.AppendFormat("{0}, {1}", pt.X, pt.Y);
				text.Append("; Relative: ");
				text.AppendFormat("{0}, {1}", rel.X, rel.Y);
				text.Append("; Stretched: ");
				text.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}", st.X, st.Y);
			}
			else
			{
				text.Append("<b>Color</b>: ");
				text.AppendLine(color.ToString());
				text.Append("<br><b>Coordinate</b>: Absolute: ");
				text.AppendFormat("{0}, {1}", pt.X, pt.Y);
			}
			return text.ToString();
		}

		public void Click()
		{
			try
			{
				Click(PointInformation(), GetRelativeImagePath(Screenshot()));
			}
			catch(Exception)
			{
				// ignore
			}
		}

		public void Click(string info, string imagePath)
		{
			string record = string.Format(pLogRecordFormat, info, imagePath);
			pLog.Log(record, false);
		}

		public void Close()
		{
			pLog.Log(pLogFooter, false);
			pLog.Flush();
		}
	}
}
