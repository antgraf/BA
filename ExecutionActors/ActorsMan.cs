using System;
using System.Collections.Generic;
using System.Timers;
using Gma.UserActivityMonitor;
using System.Windows.Forms;

namespace ExecutionActors
{
	public class CounterTimer : System.Timers.Timer
	{
		private int pCounter = 1;

		public delegate void TimerCallback();

		public CounterTimer()
		{
			Callback = null;
		}

		public CounterTimer(double interval)
			: base(interval)
		{
			Callback = null;
		}

		public int Counter
		{
			get { return pCounter; }
			set { pCounter = value; }
		}

		public TimerCallback Callback { get; set; }
	}

	public static class ActorsMan
	{
		private const int pDefaultCounter = 1;

		private static readonly List<Actor> pActors = new List<Actor>();
		private static readonly List<CounterTimer> pTimers = new List<CounterTimer>();
		private static bool pPaused = false;

		static ActorsMan()
		{
			HookManager.KeyDown += HookManagerKeyDown;
		}

		public static void HookManagerKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Pause:
				{
					Pause();
					e.Handled = true;
					break;
				}
			}
		}


		public static void Pause()
		{
			lock(pActors)
			{
				foreach(Actor actor in pActors)
				{
					if(pPaused)
					{
						
						actor.Continue();
					}
					else
					{
						actor.Pause();
					}
				}
				pPaused = !pPaused;
			}
		}

		public static Actor NewActor(Type type, IActorObserver observer)
		{
			return NewActor(type, null, observer);
		}

		public static Actor NewActor(Type type, object data, IActorObserver observer)
		{
			Actor actor = (Actor)Activator.CreateInstance(type);
			actor.Init(data, observer);
			pActors.Add(actor);
			return actor;
		}

		public static CounterTimer CreateTimer(CounterTimer.TimerCallback callback, int msec, int count = pDefaultCounter)
		{
			CounterTimer timer = new CounterTimer(msec);
			timer.Elapsed += TimeEvent;
			timer.Counter = count;
			timer.Callback = callback;
			pTimers.Add(timer);
			timer.Start();
			return timer;
		}

		public static void TimeEvent(object source, ElapsedEventArgs e)
		{
			CounterTimer timer = (CounterTimer)source;
			if(--timer.Counter <= 0)
			{
				timer.Stop();
				pTimers.Remove(timer);
			}
			timer.Callback();
		}

		public static Actor[] Actors
		{
			get { return pActors.ToArray(); }
		}

		public static CounterTimer[] Timers
		{
			get { return pTimers.ToArray(); }
		}
	}
}
