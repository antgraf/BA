using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleWindow
{
	public partial class SampleForm : Form
	{
		public SampleForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Text = "RED clicked";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void label1_DoubleClick(object sender, EventArgs e)
		{
			this.Text = "LABEL double-clicked";
		}

		private void label1_Click(object sender, EventArgs e)
		{
			MouseEventArgs m = e as MouseEventArgs;
			switch(m.Button)
			{
				case MouseButtons.Left:
				{
					this.Text = "LABEL Lclicked";
					break;
				}
				case MouseButtons.Right:
				{
					this.Text = "LABEL Rclicked";
					break;
				}
				case MouseButtons.Middle:
				{
					this.Text = "LABEL Mclicked";
					break;
				}
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.Text = textBox1.Text;
		}
	}
}
