using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ExecutionActors
{
	public class StateMachine : State
	{
		private const double pActivationInterval = 1.0 * 1000;

		private static Dictionary<string, StateMachine> pStateMachines = new Dictionary<string, StateMachine>();
		private Queue<int> pEventQueue = new Queue<int>();
		private Timer pActivationTimer = new Timer(pActivationInterval);

		public StateMachine(State initialState)
		{
			pCurrentSubState = initialState;
			pActivationTimer.AutoReset = true;
			pActivationTimer.Elapsed += new ElapsedEventHandler(pActivationTimer_Elapsed);
			pActivationTimer.Start();
		}

		private void pActivationTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock(pEventQueue)
			{
				pActivationTimer.Stop();
				try
				{
					while(pEventQueue.Count > 0)
					{
						base.HandleEvent(pEventQueue.Dequeue());
					}
				}
				finally
				{
					pActivationTimer.Start();
				}
			}
		}

		public static StateMachine GetInstance(string id)
		{
			return pStateMachines[id];
		}

		public static void Register(string id, StateMachine machine)
		{
			pStateMachines.Add(id, machine);
		}

		public static bool UnRegister(string id)
		{
			return pStateMachines.Remove(id);
		}

		public static void UnRegisterAll()
		{
			pStateMachines.Clear();
		}

		public override State HandleEvent(int eventId)
		{
			lock(pEventQueue)
			{
				pEventQueue.Enqueue(eventId);
			}
			return null;
		}
	}
}
