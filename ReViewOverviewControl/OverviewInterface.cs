using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ReView
{
	public delegate void OverviewImageChanged(Image image);
	public delegate void PanOffsetChanged(int x, int y, bool userChange);

	public interface OverviewInterface
	{
		event OverviewImageChanged OnOverviewImageChanged;
		event PanOffsetChanged OnPanOffsetChanged;

		int ContentHeight { get; }
		int ViewportHeight { get; }

		void SetVerticalPanOffset(int y);
	}
}
