using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutionActors
{
	public class StateMachine : State
	{
		private static Dictionary<string, StateMachine> pStateMachines = new Dictionary<string, StateMachine>();

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
	}
}
