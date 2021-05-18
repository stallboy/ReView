using ReView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewTool.Modules.RemoteDebugRenderer
{
	public class SceneInfo : INotifyPropertyChanged
	{
		public SceneInfo()
		{
		}

		[CategoryAttribute("Statistics"), ReadOnlyAttribute(true), DescriptionAttribute("How many opaque triangles is there in the scene at current frame.")]
		public int OpaqueTriangles
		{
			get
			{
				return opaqueTriangles;
			}
			set
			{
				if (opaqueTriangles != value)
				{
					opaqueTriangles = value;
					NotifyPropertyChanged("OpaqueTriangles");
				}
			}
		}

		[CategoryAttribute("Statistics"), ReadOnlyAttribute(true), DescriptionAttribute("How many transparent triangles is there in the scene at current frame.")]
		public int TransparentTriangles
		{
			get
			{
				return transparentTriangles;
			}
			set
			{
				if (transparentTriangles != value)
				{
					transparentTriangles = value;
					NotifyPropertyChanged("TransparentTriangles");
				}
			}
		}

		[CategoryAttribute("Statistics"), ReadOnlyAttribute(true), DescriptionAttribute("How many opaque lines is there in the scene at current frame.")]
		public int OpaqueLines
		{
			get
			{
				return opaqueLines;
			}
			set
			{
				if (opaqueLines != value)
				{
					opaqueLines = value;
					NotifyPropertyChanged("OpaqueLines");
				}
			}
		}

		[CategoryAttribute("Statistics"), ReadOnlyAttribute(true), DescriptionAttribute("How many transparent lines is there in the scene at current frame.")]
		public int TransparentLines
		{
			get
			{
				return transparentLines;
			}
			set
			{
				if (transparentLines != value)
				{
					transparentLines = value;
					NotifyPropertyChanged("TransparentLines");
				}
			}
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private int opaqueTriangles;
		private int transparentTriangles;
		private int opaqueLines;
		private int transparentLines;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
