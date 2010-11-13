using NUnit.Framework;
using System.Windows.Forms;

namespace WindowEntity.Tests
{
	[TestFixture]
	public class DesktopTest
	{
		[Test]
		public void Primary()
		{
			Window w = Desktop.Primary;
			Assert.NotNull(w);
			Assert.AreEqual(Screen.PrimaryScreen.Bounds.X, w.X);
			Assert.AreEqual(Screen.PrimaryScreen.Bounds.Y, w.Y);
			Assert.AreEqual(Screen.PrimaryScreen.Bounds.Width, w.Width);
			Assert.AreEqual(Screen.PrimaryScreen.Bounds.Height, w.Height);
		}
	}
}
