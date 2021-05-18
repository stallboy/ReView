using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public enum VerticalScrollBarPlacement
	{
		VSP_Left,
		VSP_Right
	};

	public interface VerticalScrollBar
	{
		VerticalScrollBarPlacement ScrollBarPlacement { get; set; }
	}
}
