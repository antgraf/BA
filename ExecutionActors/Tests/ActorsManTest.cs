using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExecutionActors.Tests
{
	[TestFixture]
	public class ActorsManTest
	{
		[Test]
		public void Run()
		{
			Actor a = ActorsMan.NewActor(typeof(TestActor), null);
			Assert.NotNull(a);
		}
	}
}
