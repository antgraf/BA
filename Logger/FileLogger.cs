using System;
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
			string filename = Guid.NewGuid() + pLogExtension;
			Create(filename, true);
		}

		public FileLogger(string filename, bool local = false)
		{
			Create(filename, local);
		}

		protected void Create(string filename, bool local)
		{
			pFileName = local ? FileUtils.Relative2AbsolutePath(FileUtils.CombineWinPath(pLogFolder, filename)) : filename;
			pLog = File.CreateText(pFileName);
		}

		public void Log(string msg, bool timestamp = true)
		{
			if(timestamp)
			{
				msg = string.Format(pLogMessageFormat, DateTime.Now, msg);
			}
			lock(pLog)
			{
				pLog.WriteLine(msg);
				pLog.Flush();
			}
		}

		public void Log(string source, string msg, bool timestamp = true)
		{
			msg = string.Format(pLogSourceFormat, source, msg);
			Log(msg, timestamp);
		}

		public void Flush()
		{
			pLog.Flush();
		}
	}
}
