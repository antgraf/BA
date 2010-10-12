using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutionActors
{
	public class State
	{
		protected string pStateMachineId = "Common";
		protected State pCurrentSubState = null;
		protected List<TransitionBase> pTransitions = new List<TransitionBase>();
		protected object pArgumentToNextState = null;

		public State()
		{}

		public virtual void Init(object arg)
		{}

		private void ChangeSubState(State from, State to)
		{
			// TODO add events
			from.Leave();
			to.Enter();
		}

		protected void SendEvent(int eventId)
		{
			StateMachine.GetInstance(pStateMachineId).HandleEvent(eventId);
		}

		private State _HandleEvent(int eventId)
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

		public State HandleEvent(int eventId)
		{
			// TODO add events
			State newState = HandleEventCustom(eventId);
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
