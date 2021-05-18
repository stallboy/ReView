using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace ReView
{
	[Designer("Viewport.CustomScrollablePanelDesignerSupport, System.Design", typeof(IDesigner))]
	public partial class ViewportContainer : ContainerControl
	{
		public ViewportContainer()
		{
			InitializeComponent();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			InternalResizeControls();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			InternalResizeControls();
		}

		public override DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				base.Dock = DockStyle.Fill;
			}
		}
		
		protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
		{
			return new ControlCollection(this);
		}

		public new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(Control owner) : base(owner)
			{
			}

			public override void Add(Control value)
			{
				VerticalScrollBar vscrollbar = value as VerticalScrollBar;
				if (vscrollbar != null)
				{
					((ViewportContainer)Owner).verticalScrollBar = value;
				}
				HorizontalScrollBar hscrollbar = value as HorizontalScrollBar;
				if (hscrollbar != null)
				{
					((ViewportContainer)Owner).horizontalScrollBar = value;
				}
				base.Add(value);
			}
		}

		public Rectangle GetViewportRectangle()
		{
			Rectangle rect = new Rectangle();
			rect.X = (verticalScrollBar != null && ((VerticalScrollBar)verticalScrollBar).ScrollBarPlacement == VerticalScrollBarPlacement.VSP_Left) ? verticalScrollBar.Width : 0;
			rect.Width = verticalScrollBar != null ? (Size.Width - verticalScrollBar.Width) : Size.Width;

			rect.Y = (horizontalScrollBar != null && ((HorizontalScrollBar)horizontalScrollBar).ScrollBarPlacement == HorizontalScrollBarPlacement.HSP_Top) ? horizontalScrollBar.Height : 0;
			rect.Height = horizontalScrollBar != null ? (Size.Height - horizontalScrollBar.Height) : Size.Height;

			return rect;
		}

		public Rectangle GetHorizontalScrollBarRectangle()
		{
			Rectangle rect = new Rectangle();
			if (viewport != null)
			{
				rect.X = ((ViewportPanel)viewport).HorizontalScrollBarMargins.Left;
				rect.Width = Size.Width - ((ViewportPanel)viewport).HorizontalScrollBarMargins.Left - ((ViewportPanel)viewport).HorizontalScrollBarMargins.Right;
				if (verticalScrollBar != null)
				{
					rect.Width -= verticalScrollBar.Width;
				}
				if (horizontalScrollBar != null)
				{
					rect.Y = ((HorizontalScrollBar)horizontalScrollBar).ScrollBarPlacement == HorizontalScrollBarPlacement.HSP_Bottom ? Size.Height - horizontalScrollBar.Height : 0;
					rect.Height = horizontalScrollBar.Height;
				}
				else
				{
					rect.Y = 0;
					rect.Height = 0;
				}
			}
			return rect;
		}

		public Rectangle GetVerticalScrollBarRectangle()
		{
			Rectangle rect = new Rectangle();
			if (viewport != null && verticalScrollBar != null)
			{
				rect.X = ((VerticalScrollBar)verticalScrollBar).ScrollBarPlacement == VerticalScrollBarPlacement.VSP_Right ? Size.Width - verticalScrollBar.Width : 0;
				rect.Y = ((ViewportPanel)viewport).VerticalScrollBarMargins.Top;
				rect.Width = verticalScrollBar.Width;
				rect.Height = Size.Height - ((ViewportPanel)viewport).VerticalScrollBarMargins.Top - ((ViewportPanel)viewport).VerticalScrollBarMargins.Bottom;
				if (horizontalScrollBar != null)
				{
					rect.Y += horizontalScrollBar.Height;
					rect.Height -= horizontalScrollBar.Height;
				}
			}
			return rect;
		}

		private void InternalResizeControls()
		{
			if (viewport != null)
			{
				Rectangle viewportRectangle = GetViewportRectangle();
				viewport.MinimumSize = new Size(viewportRectangle.Width, viewportRectangle.Height);
				viewport.SetBounds(viewportRectangle.X, viewportRectangle.Y, viewportRectangle.Width, viewportRectangle.Height);
				viewport.Dock = DockStyle.None;

				if (horizontalScrollBar != null)
				{
					Rectangle horizontalScrollBarRect = GetHorizontalScrollBarRectangle();
					horizontalScrollBar.MinimumSize = new Size(horizontalScrollBarRect.Width, horizontalScrollBarRect.Height);
					horizontalScrollBar.SetBounds(horizontalScrollBarRect.X, horizontalScrollBarRect.Y, horizontalScrollBarRect.Width, horizontalScrollBarRect.Height);
					horizontalScrollBar.Invalidate();
				}
				if (verticalScrollBar != null)
				{
					Rectangle verticalScrollBarRect = GetVerticalScrollBarRectangle();
					verticalScrollBar.MinimumSize = new Size(verticalScrollBarRect.Width, verticalScrollBarRect.Height);
					verticalScrollBar.SetBounds(verticalScrollBarRect.X, verticalScrollBarRect.Y, verticalScrollBarRect.Width, verticalScrollBarRect.Height);
					verticalScrollBar.Invalidate();
				}
			}

			NotifyHorizontalScrollBarMarginsChanged();
		}

		private void OnScrollBarMarginsChanged()
		{
			InternalResizeControls();
		}

		public Control VerticalScrollBar
		{
			get { return verticalScrollBar; }
			set 
			{
				VerticalScrollBar scroll = (VerticalScrollBar)value;
				if (scroll == null && value != null)
				{
					throw new ArgumentException("Expected " + VerticalScrollBar.GetType().FullName + " as parameter but got " + value.GetType().FullName + " instead.");
				}
				verticalScrollBar = value; 
			}
		}

		public Control HorizontalScrollBar
		{
			get { return horizontalScrollBar; }
			set
			{
				HorizontalScrollBar scroll = (HorizontalScrollBar)value;
				if (scroll == null && value != null)
				{
					throw new ArgumentException("Expected " + HorizontalScrollBar.GetType().FullName + " as parameter but got " + value.GetType().FullName + " instead.");
				}
				horizontalScrollBar = value;
			}
		}

		public Control Viewport
		{
			get { return viewport; }
			set
			{
				ViewportPanel view = (ViewportPanel)value;
				if (view != null)
				{
					view.OnScrollBarMarginsChanged += OnScrollBarMarginsChanged;
				}
				viewport = (Control)view;
			}
		}

		private void NotifyHorizontalScrollBarMarginsChanged()
		{
			ScrollBarMarginsChanged handle = HorizontalScrollBarMarginsChanged;
			if (handle != null)
			{
				handle();
			}
		}

		public event ScrollBarMarginsChanged HorizontalScrollBarMarginsChanged;

		private Control viewport;
		private Control horizontalScrollBar;
		private Control verticalScrollBar;
	}
}
