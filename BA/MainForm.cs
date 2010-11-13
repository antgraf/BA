using System;
using System.Windows.Forms;
using BA.Properties;
using ExecutionActors;
using BACommon;

namespace BA
{
	public partial class MainForm : Form, IPluginObserver
	{
		private const string pMsgFormatFull = "[{0}] {1} ({2}%): {3}\r\n";
		private const string pMsgFormatPercent = "[{0}] {1} ({2}%)\r\n";
		private const string pMsgFormatBrief = "[{0}] {1}\r\n";

		public MainForm()
		{
			InitializeComponent();
		}

		private void LoadPlugins()
		{
			try
			{
				PluginsMan.ReadAllPlugins();
				PluginBase[] plugins = PluginsMan.Plugins;
				mnuitemPlugins.DropDownItems.Clear();
				if(plugins.Length == 0)
				{
					ToolStripMenuItem item = new ToolStripMenuItem
					                         	{
					                         		Text = Resources.MainForm_LoadPlugins__No_plugins_found_,
					                         		Enabled = false
					                         	};
					mnuitemPlugins.DropDownItems.Add(item);
				}
				else
				{
					foreach(PluginBase plugin in plugins)
					{
						plugin.Init(this);
						PluginMenuItem menu = new PluginMenuItem(plugin, this);
						mnuitemPlugins.DropDownItems.Add(menu);
					}
				}
			}
			catch(Exception ex)
			{
				Notify(ex.ToString());
			}
		}

		#region IPluginObserver Members

		public void Notify(PluginBase module, string stage, int percentage, string message)
		{
			Invoke(new MethodInvoker(delegate
			{
				string msg = string.Format(pMsgFormatFull, module, stage, percentage, message);
				txtboxOutput.Text += msg;
				lblStatus.Text = stage;
			}));
		}

		public void Notify(PluginBase module, string stage, int percentage)
		{
			Invoke(new MethodInvoker(delegate
			{
				string msg = string.Format(pMsgFormatPercent, module, stage, percentage);
				txtboxOutput.Text += msg;
				lblStatus.Text = stage;
			}));
		}

		public void Notify(PluginBase module, string message)
		{
			Invoke(new MethodInvoker(delegate
			{
				string msg = string.Format(pMsgFormatBrief, module, message);
				txtboxOutput.Text += msg;
			}));
		}

		#endregion

		public void Notify(string message)
		{
			txtboxOutput.Text += message + Resources.MainForm_Notify_;
		}

		private static void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void RefreshPluginsToolStripMenuItemClick(object sender, EventArgs e)
		{
			LoadPlugins();
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			LoadPlugins();
		}

		private void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			string about = string.Format("{0}\r\n(C) 2010 antgraf", Globals.VersionText);
			MessageBox.Show(this, about, Resources.MainForm_AboutToolStripMenuItemClick_About, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
