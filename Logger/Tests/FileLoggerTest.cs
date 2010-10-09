using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BACommon;

namespace Logger.Tests
{
	[TestFixture]
	class FileLoggerTest
	{
		private const string pTempFolder = @"C:\Temp";

		[Test]
		public void Log()
		{
			FileLogger logger = new FileLogger(FileUtils.CombineWinPath(pTempFolder, "LoggerTest.log"), false);
			Assert.NotNull(logger);
			logger.Log("Test", "test logger");
		}
	}
}
