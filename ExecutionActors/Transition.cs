namespace ExecutionActors
{
	public class Transition<TFrom, TTo> : TransitionBase
		where TFrom: State, new()
		where TTo: State, new()
	{
		public virtual bool CheckConstraints(TFrom currentState, int eventId)
		{
			return true;
		}

		public virtual TTo CreateNextState(object arg = null)
		{
			TTo nextState = new TTo();
			nextState.Init(arg);
			return nextState;
		}

		public override State Transit(State currentState, int eventId, object arg)
		{
			return !CheckConstraints((TFrom)currentState, eventId) ? null : CreateNextState(arg);
		}
	}
}
