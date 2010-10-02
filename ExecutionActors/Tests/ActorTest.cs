using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExecutionActors.Tests
{
	internal class TestActor : Actor
	{
		protected override void Worker()
		{
		}
	}

	[TestFixture]
	public class ActorTest
	{
		[Test]
		public void Create()
		{
			Actor a = new TestActor();
			Assert.NotNull(a);
			a.Run();
		}
	}
}
