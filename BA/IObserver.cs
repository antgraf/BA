using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExecutionActors;

namespace BA
{
	public interface IObserver
	{
		void Notify(PluginBase module, string stage, int percentage, string message);
		void Notify(PluginBase module, string stage, int percentage);
		void Notify(PluginBase module, string message);
	}
}
