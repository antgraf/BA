using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace ExecutionActors
{
	public enum ActorStatus
	{
		Idle,
		Running,
		UnhandledException,
		Completed
	}

	public abstract class Actor
	{
		protected Thread pThread = null;
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
			pThread = new Thread(new ThreadStart(PreWorker));
			pThread.Start();
			pStatus = ActorStatus.Running;
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
