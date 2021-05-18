using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public struct HorizontalMargin
	{
		public HorizontalMargin(int inLeft, int inRight)
		{
			Left = inLeft;
			Right = inRight;
		}

		public int Left;
		public int Right;
	}

	public struct VerticalMargin
	{
		public VerticalMargin(int inTop, int inBottom)
		{
			Top = inTop;
			Bottom = inBottom;
		}

		public int Top;
		public int Bottom;
	}

	public delegate void ScrollBarMarginsChanged();

	public interface ViewportPanel
	{
		HorizontalMargin HorizontalScrollBarMargins { get; set; }
		VerticalMargin VerticalScrollBarMargins { get; set; }

		event ScrollBarMarginsChanged OnScrollBarMarginsChanged;
	}
}
