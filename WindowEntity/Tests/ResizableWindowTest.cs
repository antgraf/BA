using NUnit.Framework;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class ResizableWindowTest
	{
		[Test]
		public void Create()
		{
			Window w = new ResizableWindow();
			Assert.NotNull(w);
		}
	}
}
