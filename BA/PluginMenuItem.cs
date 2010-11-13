using System;
using System.Windows.Forms;
using ExecutionActors;

namespace BA
{
	public sealed class PluginMenuItem : ToolStripMenuItem
	{
		private readonly PluginBase pPlugin = null;
		private readonly IPluginObserver pObserver = null;

		public PluginMenuItem(PluginBase plugin, IPluginObserver observer)
		{
			pPlugin = plugin;
			pObserver = observer;
			Click += PluginMenuItemClick;
			Text = plugin.Name;
		}

		private void PluginMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				pObserver.Notify(pPlugin, "Launch plugin");
				pPlugin.ShowUI();
			}
			catch(Exception ex)
			{
				pObserver.Notify(pPlugin, ex.ToString());
			}
		}
	}
}
