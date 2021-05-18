using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReView
{
	public partial class ReViewOverviewControl : UserControl, VerticalScrollBar
	{
		public ReViewOverviewControl()
		{
			InitializeComponent();

			this.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);

			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}

		private void UpdatePanOffset(int centerY)
		{
			Rectangle viewportRectangle = GetViewportRectangle();
			int y = ScrollBarToContentCoordinate(centerY - viewportRectangle.Height / 2);
			y = Math.Max(y, 0);
			y = Math.Min(y, contentPanel.ContentHeight - contentPanel.ViewportHeight);

			contentPanel.SetVerticalPanOffset(-y);
			panOffset = new Point(0, y);
		}

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (drag)
			{
				UpdatePanOffset(e.Y);
			}
		}

		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				UpdatePanOffset(e.Y);
				drag = true;
			}
		}

		private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			drag = false;
		}

		public VerticalScrollBarPlacement ScrollBarPlacement
		{
			get { return VerticalScrollBarPlacement.VSP_Right; }
			set { }
		}

		public OverviewInterface OverviewInterface
		{
			get { return contentPanel; }
			set 
			{ 
				contentPanel = value;
				if (contentPanel != null)
				{
					contentPanel.OnOverviewImageChanged += OnOverviewImageChanged;
					contentPanel.OnPanOffsetChanged += OnPanOffsetChanged;
				}
			}
		}

		private void OnOverviewImageChanged(Image image)
		{
			lock (overviewImageLock)
			{
				Image oldImage = overviewImage;
				overviewImage = image;
				if (oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}
			}
			
			Invalidate();
		}

		private void OnPanOffsetChanged(int x, int y, bool userChange)
		{
			panOffset = new Point(x, -y);
			Invalidate();
		}

		private int ContentToScrollBarCoordinate(int coord)
		{
			int contentHeight = contentPanel != null ? contentPanel.ContentHeight : 1;
			int viewportHeight = contentPanel != null ? contentPanel.ViewportHeight : 1;
			return (int)((float)viewportHeight / (float)contentHeight * (float)coord);
		}

		private int ScrollBarToContentCoordinate(int coord)
		{
			int contentHeight = contentPanel != null ? contentPanel.ContentHeight : 1;
			int viewportHeight = contentPanel != null ? contentPanel.ViewportHeight : 1;
			return (int)((float)coord / (float)viewportHeight * (float)contentHeight);
		}

		private Rectangle GetViewportRectangle()
		{
			int y = Math.Max(0, ContentToScrollBarCoordinate(panOffset.Y));
			int height = Math.Min(8192, ContentToScrollBarCoordinate(Height));
			return new Rectangle(0, y, Width - 1, height);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			lock (overviewImageLock)
			{
				if (overviewImage != null)
				{
					e.Graphics.DrawImage(overviewImage, 0, 0, Width, Height);
				}
				else
				{
					Brush backgroundBrush = new SolidBrush(Color.White);
					e.Graphics.FillRectangle(backgroundBrush, 0, 0, Width, Height);
					backgroundBrush.Dispose();
				}
			}

			Rectangle viewportRectangle = GetViewportRectangle();

			using (Pen borderPen = new Pen(Color.Black))
			{
				e.Graphics.DrawRectangle(borderPen, viewportRectangle);
			}
		}

		private OverviewInterface contentPanel;
		private Image overviewImage;
		private Point panOffset = new Point(0, 0);
		private bool drag = false;
		private Object overviewImageLock = new Object();
	}
}
