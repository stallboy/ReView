using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	public partial class HTLButtonContainer : UserControl
	{
		public HTLButtonContainer()
		{
			InitializeComponent();
		}

		public FlowLayoutPanel GetFlowLayoutPanel()
		{
			return flowLayout;
		}

		public FlowLayoutPanel GetLogFlagFilterFlowLayoutPanel()
		{
			return logFlagFilterFlowLayout;
		}

		public CheckBox GetToggleGenericTracks()
		{
			return toggleGenericTracks;
		}

		public TextBox GetRegExpFilter()
		{
			return regexpFilter;
		}

		private void customizeButton_Click(object sender, EventArgs e)
		{
			HTLCustomizeDialog cd = new HTLCustomizeDialog();
			cd.ShowDialog(this);
		}
	}
}
