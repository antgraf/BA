namespace PixelScout
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.txtboxWindow = new System.Windows.Forms.TextBox();
			this.pnlColor = new System.Windows.Forms.Panel();
			this.chkboxTop = new System.Windows.Forms.CheckBox();
			this.trayMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.trayMenu;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "Pixel Scout";
			this.notifyIcon.Visible = true;
			// 
			// trayMenu
			// 
			this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.trayMenu.Name = "trayMenu";
			this.trayMenu.Size = new System.Drawing.Size(93, 26);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(ExitToolStripMenuItemClick);
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 200;
			this.timer.Tick += new System.EventHandler(this.TimerTick);
			// 
			// txtboxWindow
			// 
			this.txtboxWindow.Location = new System.Drawing.Point(12, 12);
			this.txtboxWindow.Name = "txtboxWindow";
			this.txtboxWindow.ReadOnly = true;
			this.txtboxWindow.Size = new System.Drawing.Size(188, 20);
			this.txtboxWindow.TabIndex = 1;
			// 
			// pnlColor
			// 
			this.pnlColor.Location = new System.Drawing.Point(206, 12);
			this.pnlColor.Name = "pnlColor";
			this.pnlColor.Size = new System.Drawing.Size(50, 50);
			this.pnlColor.TabIndex = 2;
			// 
			// chkboxTop
			// 
			this.chkboxTop.AutoSize = true;
			this.chkboxTop.Checked = true;
			this.chkboxTop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkboxTop.Location = new System.Drawing.Point(12, 45);
			this.chkboxTop.Name = "chkboxTop";
			this.chkboxTop.Size = new System.Drawing.Size(62, 17);
			this.chkboxTop.TabIndex = 3;
			this.chkboxTop.Text = "On Top";
			this.chkboxTop.UseVisualStyleBackColor = true;
			this.chkboxTop.CheckedChanged += new System.EventHandler(this.ChkboxTopCheckedChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(268, 74);
			this.Controls.Add(this.chkboxTop);
			this.Controls.Add(this.pnlColor);
			this.Controls.Add(this.txtboxWindow);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Pixel Scout";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.trayMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ContextMenuStrip trayMenu;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.TextBox txtboxWindow;
		private System.Windows.Forms.Panel pnlColor;
		private System.Windows.Forms.CheckBox chkboxTop;

	}
}

