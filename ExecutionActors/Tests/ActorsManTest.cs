using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace ExecutionActors.Tests
{
	[TestFixture]
	public class ActorsManTest
	{
		private ManualResetEvent pEvent = new ManualResetEvent(false);

		[Test]
		public void Run()
		{
			Actor a = ActorsMan.NewActor(typeof(TestActor), null);
			Assert.NotNull(a);
		}

		public void TestTimerCallback()
		{
			pEvent.Set();
		}

		[Test]
		public void Timer()
		{
			int time = 2 * 1000;
			int timeout = 5 * 1000;
			Assert.AreEqual(0, ActorsMan.Timers.Length);
			long begin = DateTime.Now.Ticks;
			ActorsMan.CreateTimer(new CounterTimer.TimerCallback(TestTimerCallback), time);
			Assert.AreEqual(1, ActorsMan.Timers.Length);
			pEvent.WaitOne(timeout);
			long end = DateTime.Now.Ticks;
			Assert.AreEqual(0, ActorsMan.Timers.Length);
			Assert.GreaterOrEqual(end - begin, time * 10000);
			Assert.Less(end - begin, timeout * 10000);
		}
	}
}
