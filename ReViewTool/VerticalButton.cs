using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReViewTool
{
	public partial class VerticalButton : Button
	{
		public VerticalButton()
		{
			InitializeComponent();

			format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
		}
		
		protected override void OnPaint(PaintEventArgs args)
		{
			base.OnPaint(args);

			args.Graphics.TranslateTransform(0, Height);
			args.Graphics.RotateTransform(-90);
			args.Graphics.DrawString(VerticalText, Font, new SolidBrush(ForeColor), new Rectangle(0, 0, Height, Width), format);
		}

		public String VerticalText
		{
			get;
			set;
		}

		private StringFormat format;
	}
}
