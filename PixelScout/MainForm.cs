﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using WindowEntity;

namespace PixelScout
{
	public partial class MainForm : Form
	{
		private const Keys pDefaultHookKey = Keys.F8;

		private Keys pHookKey = pDefaultHookKey;
		private HtmlRecorder pLog = new HtmlRecorder();

		public MainForm()
		{
			InitializeComponent();
		}

		private void HookKey()
		{
			HookKey(pDefaultHookKey);
		}

		private void HookKey(Keys key)
		{
			pHookKey = key;
			HookManager.KeyDown += new KeyEventHandler(HookManager_KeyDown);
		}

		private void Exit()
		{
			Application.Exit();
		}

		private void Reset()
		{
			txtboxWindow.Text = string.Empty;
			pnlColor.BackColor = SystemColors.Control;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Exit();
		}

		void HookManager_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == pHookKey)
			{
				pLog.Click();
				e.Handled = true;
			}
		}

		private void timer_Tick(object sender, EventArgs e)
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

		private void chkboxTop_CheckedChanged(object sender, EventArgs e)
		{
			TopMost = chkboxTop.Checked;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			pLog.Close();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			HookKey();
		}
	}
}
