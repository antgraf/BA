using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
