using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExecutionActors.Tests
{
	enum TrafficLightsEvents
	{
		TurnOn,
		TurnOff,
		Switch
	}

	enum TrafficLights
	{
		Red,
		Yellow,
		Green
	}

	class TrafficLightsStateMachine : StateMachine
	{
	}

	class TrafficLightsState : State
	{
		private TrafficLights pPrevious;

		public TrafficLightsState()
		{
			pStateMachineId = "antgraf.Test.TrafficLights";
		}

		public override void Init(object arg)
		{
			pPrevious = (TrafficLights)arg;
		}

		public TrafficLightsState HandleEvent(TrafficLightsEvents eventId)
		{
			return (TrafficLightsState)HandleEvent((int)eventId);
		}
	}

	class TrafficLightsTransition : Transition<TrafficLightsState, TrafficLightsState>
	{
	}

	[TestFixture]
	class StateMachineTest
	{
		private TrafficLightsStateMachine CreateStateMachine()
		{
			TrafficLightsStateMachine machine = new TrafficLightsStateMachine();
			// TODO
			return machine;
		}
	}
}
