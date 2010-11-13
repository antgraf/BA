using System;
using System.Drawing;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using WindowEntity;

namespace PixelScout
{
	public partial class MainForm : Form
	{
		private const Keys pDefaultHookKey = Keys.F8;

		private Keys pHookKey = pDefaultHookKey;
		private readonly HtmlRecorder pLog = new HtmlRecorder();

		public MainForm()
		{
			InitializeComponent();
		}

		private void HookKey(Keys key = pDefaultHookKey)
		{
			pHookKey = key;
			HookManager.KeyDown += HookManagerKeyDown;
		}

		private static void Exit()
		{
			Application.Exit();
		}

		private void Reset()
		{
			txtboxWindow.Text = string.Empty;
			pnlColor.BackColor = SystemColors.Control;
		}

		private static void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Exit();
		}

		void HookManagerKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == pHookKey)
			{
				pLog.Click();
				e.Handled = true;
			}
		}

		private void TimerTick(object sender, EventArgs e)
		{
			Window window = Window.GetWindowAtCursor();
			try
			{
				if(window == null)
				{
					Reset();
				}
				else
				{
					Color color = Desktop.Primary.GetPixelColor(
						new Coordinate(CoordinateType.Absolute,
							new WindowEntity.Point() { X = Cursor.Position.X, Y = Cursor.Position.Y }));
					txtboxWindow.Text = window.Title;
					pnlColor.BackColor = color;
				}
			}
			catch(Exception)
			{
				Reset();
			}
		}

		private void ChkboxTopCheckedChanged(object sender, EventArgs e)
		{
			TopMost = chkboxTop.Checked;
		}

		private void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			pLog.Close();
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			HookKey();
		}
	}
}
