﻿#define UseDeprecatedThreadFunctions	// We use them not for synchronization but as they are for suspend & resume, so it's ok

using System;
using System.Threading;
using Logger;
using BACommon;

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
		private const string pLogNameFormat = "{0}-{1}({2}).log.txt";

// ReSharper disable InconsistentNaming
#if UseDeprecatedThreadFunctions
		protected Thread pThread = null;
#else
		protected SuspendableThread pThread = null;
#endif
		protected ActorStatus pStatus = ActorStatus.Idle;
		protected Exception pUnhandledException = null;
		protected IActorObserver pObserver = null;
		protected FileLogger pLog = null;
// ReSharper restore InconsistentNaming

		public Actor()
		{
			pLog = new FileLogger(CreateFileName());
		}

		private string CreateFileName()
		{
			DateTime now = DateTime.Now;
			return FileUtils.MakeValidFileName(string.Format(pLogNameFormat, now, Guid.NewGuid(), GetType()));
		}

		public virtual void Init(object data, IActorObserver observer)
		{
			pObserver = observer;
			Init(data);
		}

		protected abstract void Init(object data);

		public virtual void Run()
		{
#if UseDeprecatedThreadFunctions
			pThread = new Thread(PreWorker);
#else
			pThread = new SuspendableThread(PreWorker);
#endif
			pThread.Start();
			pStatus = ActorStatus.Running;
		}

		public virtual void Pause()
		{
#if UseDeprecatedThreadFunctions
#pragma warning disable 0618
			pThread.Suspend();
#pragma warning restore 0618
#else
			switch(pThread.ThreadState)
			{
				case ThreadState.Background:
				case ThreadState.Running:
				case ThreadState.WaitSleepJoin:
				{
					pObserver.Notify(this, "Paused");
					pThread.Suspend();
					pStatus = ActorStatus.Pause;
					break;
				}
			}
#endif
		}

		public virtual void Continue()
		{
#if UseDeprecatedThreadFunctions
#pragma warning disable 0618
			pThread.Resume();
#pragma warning restore 0618
#else
			switch(pThread.ThreadState)
			{
				case ThreadState.Suspended:
				case ThreadState.SuspendRequested:
				{
					pObserver.Notify(this, "Resumed");
					pThread.Resume();
					pStatus = ActorStatus.Running;
					break;
				}
			}
#endif
		}

		public virtual void Exit()
		{
#if UseDeprecatedThreadFunctions
			pThread.Abort();
#else
			pThread.Terminate();
#endif
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
				if(pObserver != null)
				{
					pObserver.Notify(this, ex.ToString());
				}
			}
		}

		protected abstract void Worker();
	}
}
