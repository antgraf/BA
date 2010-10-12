using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutionActors
{
	public class Transition<FROM, TO> : TransitionBase
		where FROM: State, new()
		where TO: State, new()
	{
		public Transition()
		{}

		public virtual bool CheckConstraints(FROM currentState, int eventId)
		{
			return true;
		}

		public virtual TO CreateNextState()
		{
			return CreateNextState(null);
		}

		public virtual TO CreateNextState(object arg)
		{
			TO nextState = new TO();
			nextState.Init(arg);
			return nextState;
		}

		public override State Transit(State currentState, int eventId, object arg)
		{
			if(!CheckConstraints((FROM)currentState, eventId))
			{
				return null;
			}
			return CreateNextState(arg);
		}
	}
}
