using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace ExecutionActors.Tests
{
	#region StateMachine

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
		public const string Id = "antgraf.Test.TrafficLights";

		private const int pWaitTimeMs = 1500;

		private TrafficLights pCurrentLight = TrafficLights.Yellow;
		private bool pIsTurnedOff = true;

		public TrafficLightsStateMachine(TrafficLightsState initialState)
			: base(initialState)
		{}

		public static void Wait()
		{
			Thread.Sleep(pWaitTimeMs);
		}

		public TrafficLightsState HandleEvent(TrafficLightsEvents eventId)
		{
			return (TrafficLightsState)HandleEvent((int)eventId);
		}

		public TrafficLights CurrentLight
		{
			get { return pCurrentLight; }
			set { pCurrentLight = value; }
		}

		public bool IsTurnedOff
		{
			get { return pIsTurnedOff; }
			set { pIsTurnedOff = value; }
		}
	}

	class TrafficLightsState : State
	{
		protected TrafficLights pPrevious = TrafficLights.Yellow;
		protected TrafficLights pLight = TrafficLights.Yellow;
		protected bool pOff = true;

		public TrafficLightsState()
		{
			pStateMachineId = TrafficLightsStateMachine.Id;
		}

		public override void Init(object arg)
		{
			if(arg != null)
			{
				pPrevious = (TrafficLights)arg;
			}
		}

		public override void Enter()
		{
			TrafficLightsStateMachine machine = (TrafficLightsStateMachine)StateMachine.GetInstance(pStateMachineId);
			machine.IsTurnedOff = pOff;
			machine.CurrentLight = pLight;
			pArgumentToNextState = pLight;
		}

		public ExecutionActors.Tests.TrafficLights Previous
		{
			get { return pPrevious; }
		}

		public ExecutionActors.Tests.TrafficLights Light
		{
			get { return pLight; }
		}

		public bool Off
		{
			get { return pOff; }
		}
	}

	class TrafficLightsOff : TrafficLightsState
	{
		public TrafficLightsOff()
		{
			pOff = true;
			pTransitions.Add(new TrafficLightsTransitionOffOn());
		}
	}

	class TrafficLightsOn : TrafficLightsState
	{
		public TrafficLightsOn()
		{
			pOff = false;
			pTransitions.Add(new TrafficLightsTransitionOnOff());
			pCurrentSubState = new TrafficLightsYellow();
		}
	}

	class TrafficLightsRed : TrafficLightsState
	{
		public TrafficLightsRed()
		{
			pOff = false;
			pLight = TrafficLights.Red;
			pTransitions.Add(new TrafficLightsTransitionRedYellow());
		}
	}

	class TrafficLightsYellow : TrafficLightsState
	{
		public TrafficLightsYellow()
		{
			pOff = false;
			pLight = TrafficLights.Yellow;
			pTransitions.Add(new TrafficLightsTransitionYellowRed());
			pTransitions.Add(new TrafficLightsTransitionYellowGreen());
		}
	}

	class TrafficLightsGreen : TrafficLightsState
	{
		public TrafficLightsGreen()
		{
			pOff = false;
			pLight = TrafficLights.Green;
			pTransitions.Add(new TrafficLightsTransitionGreenYellow());
		}
	}

	class TrafficLightsTransitionOffOn : Transition<TrafficLightsOff, TrafficLightsOn>
	{
		public override bool CheckConstraints(TrafficLightsOff currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.TurnOn;
		}
	}

	class TrafficLightsTransitionOnOff : Transition<TrafficLightsOn, TrafficLightsOff>
	{
		public override bool CheckConstraints(TrafficLightsOn currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.TurnOff;
		}
	}

	class TrafficLightsTransitionRedYellow : Transition<TrafficLightsRed, TrafficLightsYellow>
	{
		public override bool CheckConstraints(TrafficLightsRed currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.Switch;
		}
	}

	class TrafficLightsTransitionGreenYellow : Transition<TrafficLightsGreen, TrafficLightsYellow>
	{
		public override bool CheckConstraints(TrafficLightsGreen currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.Switch;
		}
	}

	class TrafficLightsTransitionYellowRed : Transition<TrafficLightsYellow, TrafficLightsRed>
	{
		public override bool CheckConstraints(TrafficLightsYellow currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.Switch && currentState.Previous == TrafficLights.Green;
		}
	}

	class TrafficLightsTransitionYellowGreen : Transition<TrafficLightsYellow, TrafficLightsGreen>
	{
		public override bool CheckConstraints(TrafficLightsYellow currentState, int eventId)
		{
			return eventId == (int)TrafficLightsEvents.Switch && currentState.Previous != TrafficLights.Green;
		}
	}

	#endregion

	[TestFixture]
	public class StateMachineTest
	{
		private TrafficLightsStateMachine CreateStateMachine()
		{
			StateMachine.UnRegister(TrafficLightsStateMachine.Id);
			TrafficLightsState initialState = new TrafficLightsOff();
			TrafficLightsStateMachine machine = new TrafficLightsStateMachine(initialState);
			StateMachine.Register(TrafficLightsStateMachine.Id, machine);
			return machine;
		}

		[Test]
		public void TrafficLightsTest()
		{
			TrafficLightsStateMachine machine = CreateStateMachine();
			Assert.NotNull(machine);
			Assert.NotNull(StateMachine.GetInstance(TrafficLightsStateMachine.Id));
			Assert.AreEqual(true, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.TurnOn);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.TurnOn);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.Switch);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Green, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.Switch);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.Switch);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Red, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.Switch);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(false, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
			machine.HandleEvent(TrafficLightsEvents.TurnOff);
			TrafficLightsStateMachine.Wait();
			Assert.AreEqual(true, machine.IsTurnedOff);
			Assert.AreEqual(TrafficLights.Yellow, machine.CurrentLight);
		}
	}
}
