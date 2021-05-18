using _TestGame.Managers;
using _TestGame.TestGame;
using ReView;
using ReViewDebugRenderView;
using ReViewDebugRenderView.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace _TestGame
{
	public partial class GameForm : Form
	{
		public GameForm()
		{
			InitializeComponent();

			this.FormClosing += OnFormClosing;

			Game = new Game(gamePanel);

			GameUpdateManager.Instance.RunningStateChanged += OnRunningStateChanged;

			DebugRenderer.Instance.PropertyChanged += OnDebugRendererPropertyChanged;
		}

		private void OnRunningStateChanged(bool newStateIsRunning)
		{
			this.Invoke((MethodInvoker)delegate
			{
				playToggle.Checked = newStateIsRunning;
			});
		}

		private void OnDebugRendererPropertyChanged(object source, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "Enabled")
			{
				this.Invoke((MethodInvoker)delegate
				{
					debugToggle.Checked = DebugRenderer.Instance.Enabled;
				});
			}
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			Game.Shutdown();
		}

		private Game Game
		{
			get;
			set;
		}

		private void debugToggle_CheckedChanged(object sender, EventArgs e)
		{
			DebugRenderer.Instance.SetEnabled((sender as ToolStripButton).Checked, true);
		}

		private void playToggle_CheckedChanged(object sender, EventArgs e)
		{
			GameUpdateManager.Instance.SetRunning((sender as ToolStripButton).Checked, true);
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			playToggle.Checked = false;
			debugToggle.Checked = false;

			Game.Shutdown();

			Game = new Game(gamePanel);
		}
	}
}
