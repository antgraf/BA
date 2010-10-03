using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
	public class SuspendableThread
	{
		#region Data

		private ManualResetEvent suspendChangedEvent = new ManualResetEvent(false);
		private ManualResetEvent terminateEvent = new ManualResetEvent(false);
		private long suspended;
		private Thread thread;
		private System.Threading.ThreadState failsafeThreadState = System.Threading.ThreadState.Unstarted;
		private ThreadStart routine;

		#endregion Data

		public SuspendableThread(ThreadStart start)
		{
			routine = start;
		}

		private void ThreadEntry()
		{
			failsafeThreadState = System.Threading.ThreadState.Stopped;
			routine();
		}

		#region Protected methods

		protected Boolean SuspendIfNeeded()
		{
			Boolean suspendEventChanged = suspendChangedEvent.WaitOne(0, true);
			if(suspendEventChanged)
			{
				Boolean needToSuspend = Interlocked.Read(ref suspended) != 0;
				suspendChangedEvent.Reset();
				if(needToSuspend)
				{
					/// Suspending...
					if(1 == WaitHandle.WaitAny(new WaitHandle[] { suspendChangedEvent, terminateEvent }))
					{
						return true;
					}
					/// ...Waking
				}
			}
			return false;
		}

		protected bool HasTerminateRequest()
		{
			return terminateEvent.WaitOne(0, true);
		}

		#endregion Protected methods

		public void Start()
		{
			thread = new Thread(new ThreadStart(ThreadEntry));

			// make sure this thread won't be automaticaly
			// terminated by the runtime when the
			// application exits
			thread.IsBackground = false;
			thread.Start();
		}

		public void Join()
		{
			if(thread != null)
			{
				thread.Join();
			}
		}

		public Boolean Join(Int32 milliseconds)
		{
			if(thread != null)
			{
				return thread.Join(milliseconds);
			}
			return true;
		}

		/// <remarks>Not supported in .NET Compact Framework</remarks>
		public Boolean Join(TimeSpan timeSpan)
		{
			if(thread != null)
			{
				return thread.Join(timeSpan);
			}
			return true;
		}

		public void Terminate()
		{
			terminateEvent.Set();
		}

		public void TerminateAndWait()
		{
			terminateEvent.Set();
			thread.Join();
		}

		public void Suspend()
		{
			while(1 != Interlocked.Exchange(ref suspended, 1))
			{
			}
			suspendChangedEvent.Set();
		}

		public void Resume()
		{
			while(0 != Interlocked.Exchange(ref suspended, 0))
			{
			}
			suspendChangedEvent.Set();
		}

		public System.Threading.ThreadState ThreadState
		{
			get
			{
				if(null != thread)
				{
					return thread.ThreadState;
				}
				return failsafeThreadState;
			}
		}
	}
}
