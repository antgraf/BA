using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BACommon;

namespace Logger
{
	public class FileLogger
	{
		private const string pLogFolder = "Logs";
		private const string pLogExtension = ".log";
		private const string pLogMessageFormat = "{0}\t{1}";
		private const string pLogSourceFormat = "[{0}] {1}";

		private string pFileName = null;
		private StreamWriter pLog = null;

		public FileLogger()
		{
			string filename = Guid.NewGuid().ToString() + pLogExtension;
			Create(filename, true);
		}

		public FileLogger(string filename)
			: this(filename, false)
		{}

		public FileLogger(string filename, bool local)
		{
			Create(filename, local);
		}

		protected void Create(string filename, bool local)
		{
			if(local)
			{
				pFileName = FileUtils.Relative2AbsolutePath(FileUtils.CombineWinPath(pLogFolder, filename));
			}
			else
			{
				pFileName = filename;
			}
			pLog = File.CreateText(pFileName);
		}

		public void Log(string msg, bool timestamp)
		{
			if(timestamp)
			{
				msg = string.Format(pLogMessageFormat, DateTime.Now.ToString(), msg);
			}
			lock(pLog)
			{
				pLog.WriteLine(msg);
				pLog.Flush();
			}
		}

		public void Log(string source, string msg, bool timestamp)
		{
			msg = string.Format(pLogSourceFormat, source, msg);
			Log(msg, timestamp);
		}

		public void Log(string msg)
		{
			Log(msg, true);
		}

		public void Log(string source, string msg)
		{
			Log(source, msg, true);
		}

		public void Flush()
		{
			pLog.Flush();
		}
	}
}
