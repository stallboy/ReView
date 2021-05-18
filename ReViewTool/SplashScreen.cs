using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ReViewTool
{
	public partial class SplashScreen : Form
	{
		public SplashScreen(int timeToWait)
		{
			InitializeComponent();

			this.timeToWait = timeToWait;

			lblVersion.Text = "VERSION " + Assembly.GetExecutingAssembly().GetName().Version;
			tbCopyrightNotice.Text = "Copyright (C) 2013 Gametec Consulting, Mika Vehkala" + Environment.NewLine + Environment.NewLine + "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:" + Environment.NewLine + Environment.NewLine + "The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software." + Environment.NewLine;

			lblVersion.Click += OnClick;
			lblTitle.Click += OnClick;
			tbDisclaimer.Click += OnClick;
			tbCopyrightNotice.Click += OnClick;
			mainTable.Click += OnClick;
			this.Click += OnClick;
		}

		protected void OnClick(object sender, EventArgs e)
		{
			waiting = false;
		}

		protected override void OnShown(EventArgs args)
		{
			waitThread = new Thread(WaitLoop);
			waitThread.IsBackground = true;
			waitThread.Start();
		}

		private void WaitLoop()
		{
			waiting = true;

			int timeLeftToWait = timeToWait;
			while (waiting)
			{
				int sleepTime = 100;
				Thread.Sleep(sleepTime);
				timeLeftToWait -= sleepTime;
				waiting = waiting && (timeToWait < 0 || timeLeftToWait > 0);
			}

			this.Invoke((MethodInvoker)delegate
			{
				Close();
			});
		}

		private int timeToWait;
		private bool waiting;
		private Thread waitThread;
	}
}
