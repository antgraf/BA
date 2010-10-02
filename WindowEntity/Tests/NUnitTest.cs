using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class NUnitTest
	{
		[Test]
		public void Ok()
		{
			int a = 12;	// magic number
			Assert.AreEqual(a, 12);
		}

		[Test]
		[ExpectedException(typeof(NUnitTestException))]
		public void Exception()
		{
			throw new NUnitTestException();
		}
	}
}
