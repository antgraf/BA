using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Common;

namespace ExecutionActors
{
	public enum ActorStatus
	{
		Idle,
		Running,
		Pause,
		UnhandledException,
		Completed
	}

	public abstract class Actor
	{
		protected SuspendableThread pThread = null;
		protected ActorStatus pStatus = ActorStatus.Idle;
		protected Exception pUnhandledException = null;
		protected IActorObserver pObserver = null;

		public virtual void Init(object data, IActorObserver observer)
		{
			pObserver = observer;
			Init(data);
		}

		protected abstract void Init(object data);

		public virtual void Run()
		{
			pThread = new SuspendableThread(new ThreadStart(PreWorker));
			pThread.Start();
			pStatus = ActorStatus.Running;
		}

		public virtual void Pause()
		{
			pThread.Suspend();
			pStatus = ActorStatus.Pause;
		}

		public virtual void Continue()
		{
			pThread.Resume();
			pStatus = ActorStatus.Running;
		}

		public virtual void Exit()
		{
			pThread.Terminate();
			pStatus = ActorStatus.Completed;
		}

		public virtual void GoToFinalState()
		{
			pStatus = ActorStatus.Completed;
		}

		protected virtual void PreWorker()
		{
			try
			{
				Worker();
				pStatus = ActorStatus.Completed;
			}
			catch(Exception ex)
			{
				pUnhandledException = ex;
				pStatus = ActorStatus.UnhandledException;
			}
		}

		protected abstract void Worker();
	}
}
