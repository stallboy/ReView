using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public enum HorizontalScrollBarPlacement
	{
		HSP_Top,
		HSP_Bottom
	};

	public interface HorizontalScrollBar
	{
		HorizontalScrollBarPlacement ScrollBarPlacement { get; set; }
	}
}
