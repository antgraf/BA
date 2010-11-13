using NUnit.Framework;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class NUnitTest
	{
		[Test]
		public void Ok()
		{
			const int a = 12;
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
