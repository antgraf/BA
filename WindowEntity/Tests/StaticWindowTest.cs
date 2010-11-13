using NUnit.Framework;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class StaticWindowTest
	{
		[Test]
		public void Create()
		{
			Window w = new StaticWindow();
			Assert.NotNull(w);
		}
	}
}
