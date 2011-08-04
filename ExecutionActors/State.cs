using System;
using System.Collections.Generic;

namespace ExecutionActors
{
	public class State
	{
// ReSharper disable InconsistentNaming
		protected string pStateMachineId = "Common";
		protected State pCurrentSubState = null;
		protected readonly List<TransitionBase> pTransitions = new List<TransitionBase>();
		protected object pArgumentToNextState = null;
// ReSharper restore InconsistentNaming

		public virtual void Init(object arg)
		{}

		protected virtual void HandleError(Exception e)
		{}

		private void ChangeSubState(State from, State to)
		{
			// TODO add events
			from._Leave();
			pCurrentSubState = to;
			to._Enter();
		}

		protected void SendEvent(int eventId)
		{
			StateMachine.GetInstance(pStateMachineId).HandleEvent(eventId);
		}

// ReSharper disable InconsistentNaming
		private State _HandleEvent(int eventId)
// ReSharper restore InconsistentNaming
		{
			State newState = null;
			foreach(TransitionBase transition in pTransitions)
			{
				newState = transition.Transit(this, eventId, pArgumentToNextState);
				if(newState != null)
				{
					break;
				}
			}
			return newState;
		}

		protected virtual State HandleEventCustom(int eventId)
		{
			return null;
		}

		public virtual State HandleEvent(int eventId)
		{
			// TODO add events
			State newState = null;
			try
			{
				newState = HandleEventCustom(eventId);
			}
			catch(Exception e)
			{
				HandleError(e);
			}
			if(newState == null)
			{
				newState = _HandleEvent(eventId);
			}
			if(newState == null && pCurrentSubState != null)
			{
				State newSubState = pCurrentSubState.HandleEvent(eventId);
				if(newSubState != null)
				{
					ChangeSubState(pCurrentSubState, newSubState);
					pCurrentSubState = newSubState;
				}
			}
			return newState;
		}

		public virtual void Leave()
		{}

		public virtual void Enter()
		{}

// ReSharper disable InconsistentNaming
		private void _Leave()
// ReSharper restore InconsistentNaming
		{
			if(pCurrentSubState != null)
			{
				pCurrentSubState._Leave();
			}
			try
			{
				Leave();
			}
			catch(Exception e)
			{
				HandleError(e);
			}
		}

// ReSharper disable InconsistentNaming
		private void _Enter()
// ReSharper restore InconsistentNaming
		{
			try
			{
				Enter();
			}
			catch(Exception e)
			{
				HandleError(e);
			}
			if(pCurrentSubState != null)
			{
				pCurrentSubState._Enter();
			}
		}

		public State CurrentSubState
		{
			get { return pCurrentSubState; }
		}

		public List<TransitionBase> Transitions
		{
			get { return pTransitions; }
		}
	}
}
