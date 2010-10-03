using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExecutionActors;
using BACommon;

namespace BA
{
	public partial class MainForm : Form, IObserver
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
					ToolStripMenuItem item = new ToolStripMenuItem();
					item.Text = "<No plugins found>";
					item.Enabled = false;
					mnuitemPlugins.DropDownItems.Add(item);
				}
				else
				{
					foreach(PluginBase plugin in plugins)
					{
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

		#region IObserver Members

		public void Notify(PluginBase module, string stage, int percentage, string message)
		{
			string msg = string.Format(pMsgFormatFull, module, stage, percentage, message);
			txtboxOutput.Text += msg;
		}

		public void Notify(PluginBase module, string stage, int percentage)
		{
			string msg = string.Format(pMsgFormatFull, module, stage, percentage);
			txtboxOutput.Text += msg;
		}

		public void Notify(PluginBase module, string message)
		{
			string msg = string.Format(pMsgFormatFull, module, message);
			txtboxOutput.Text += msg;
		}

		#endregion

		public void Notify(string message)
		{
			txtboxOutput.Text += message + "\r\n";
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void refreshPluginsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadPlugins();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			LoadPlugins();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string about = string.Format("{0}\r\n(C) 2010 antgraf", Globals.VersionText);
			MessageBox.Show(this, about, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
