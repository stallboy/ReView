using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReViewTool.Modules.RemoteDebugRender
{
	public partial class RDRButtonContainer : UserControl
	{
		public RDRButtonContainer()
		{
			InitializeComponent();
		}

		public FlowLayoutPanel GetFlowLayoutPanel()
		{
			return flowLayout;
		}

		public CheckBox GetToggleDimensions()
		{
			return toggleDimensions;
		}

		public Button GetCycleNormalsButton()
		{
			return cycleNormalsButton;
		}

		public CheckBox GetToggleFSAA()
		{
			return toggleFSAA;
		}

		public CheckBox GetToggleAnnotations()
		{
			return toggleAnnotations;
		}

		public Button GetFitToView()
		{
			return fitToViewButton;
		}

		public Button GetClearColor()
		{
			return clearColorButton;
		}

		public CheckBox GetToggleGrid()
		{
			return toggleGrid;
		}
	}
}
