namespace ExecutionActors
{
	public interface IActorObserver
	{
		void Notify(Actor actor, string stage, int percentage, string message);
		void Notify(Actor actor, string stage, int percentage);
		void Notify(Actor actor, string message);
	}
}
