using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutionActors
{
	public abstract class TransitionBase
	{
		public abstract State Transit(State currentState, int eventId, object arg);
	}
}
