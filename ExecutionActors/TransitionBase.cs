namespace ExecutionActors
{
	public abstract class TransitionBase
	{
		public abstract State Transit(State currentState, int eventId, object arg);
	}
}
