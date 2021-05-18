using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;

namespace ReView
{
	public class ViewportContainerDesignSupport : ParentControlDesigner
	{
		public ViewportContainerDesignSupport()
		{
		}

		public override void Initialize(IComponent component)
		{
			panel = component as ViewportContainer;
			if (panel == null)
			{
				DisplayError(new Exception("Can only use this design support class for Viewport!"));
			}
			base.Initialize(component);
		}

		protected override void OnPaintAdornments(PaintEventArgs args)
		{
			base.OnPaintAdornments(args);

			Pen p = new Pen(SystemColors.ControlDark, 1);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			args.Graphics.DrawRectangle(p, 0, 0, panel.Width, panel.Height);
			p.Dispose();
		}

		private ViewportContainer panel;
	}
}
