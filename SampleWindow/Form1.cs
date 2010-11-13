using System;
using System.Windows.Forms;
using SampleWindow.Properties;

namespace SampleWindow
{
	public partial class SampleForm : Form
	{
		public SampleForm()
		{
			InitializeComponent();
		}

// ReSharper disable InconsistentNaming
		private void button1_Click(object sender, EventArgs e)
		{
			Text = Resources.SampleForm_button1_Click_RED_clicked;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void label1_DoubleClick(object sender, EventArgs e)
		{
			Text = Resources.SampleForm_label1_DoubleClick_LABEL_double_clicked;
		}

		private void label1_Click(object sender, EventArgs e)
		{
			MouseEventArgs m = e as MouseEventArgs;
			if(m != null)
			{
				switch (m.Button)
				{
					case MouseButtons.Left:
						{
							Text = Resources.SampleForm_label1_Click_LABEL_Lclicked;
							break;
						}
					case MouseButtons.Right:
						{
							Text = Resources.SampleForm_label1_Click_LABEL_Rclicked;
							break;
						}
					case MouseButtons.Middle:
						{
							Text = Resources.SampleForm_label1_Click_LABEL_Mclicked;
							break;
						}
				}
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Text = textBox1.Text;
		}
// ReSharper restore InconsistentNaming
	}
}
