using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExecutionActors;

namespace BA
{
	public class PluginMenuItem : ToolStripMenuItem
	{
		private PluginBase pPlugin = null;
		private IPluginObserver pObserver = null;

		public PluginMenuItem(PluginBase plugin, IPluginObserver observer)
		{
			pPlugin = plugin;
			pObserver = observer;
			Click += pluginMenuItem_Click;
			Text = plugin.Name;
		}

		private void pluginMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				pPlugin.ShowUI();
			}
			catch(Exception ex)
			{
				pObserver.Notify(pPlugin, ex.ToString());
			}
		}
	}
}
