using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReViewDebugRenderView
{
	public class DebugRenderPrimitiveStartTimeComparer : IComparer<DebugRenderPrimitive>
	{
		public int Compare(DebugRenderPrimitive a, DebugRenderPrimitive b)
		{
			return a.StartTime < b.StartTime ? -1 : a.StartTime == b.StartTime ? 0 : 1;
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VertexNormalColor
	{
		public float x;
		public float y;
		public float z;
		public float nx;
		public float ny;
		public float nz;
		public float r;
		public float g;
		public float b;
		public float a;

		public Vector3 Position
		{
			get
			{
				return new Vector3(x, y, z);
			}
			set
			{
				x = (float)value.x;
				y = (float)value.y;
				z = (float)value.z;
			}
		}

		public Vector3 Normal
		{
			get
			{
				return new Vector3(nx, ny, nz);
			}
			set
			{
				nx = (float)value.x;
				ny = (float)value.y;
				nz = (float)value.z;
			}
		}

		public Color32 Color
		{
			get
			{
				return new Color32(r, g, b, a);
			}
			set
			{
				r = (float)value.R_Double;
				g = (float)value.G_Double;
				b = (float)value.B_Double;
				a = (float)value.A_Double;
			}
		}

		// Transform bounds with matrix
		public static VertexNormalColor operator *(Matrix4x4 mat, VertexNormalColor vnc)
		{
			VertexNormalColor result = new VertexNormalColor();
			result.Color = vnc.Color;
			result.Normal = vnc.Normal;
			result.Position = mat * vnc.Position;
			return result;
		}
	}

	public class DebugRenderPrimitive : INotifyPropertyChanged, IComparable<DebugRenderPrimitive>
	{
		public static uint INVALID_VBO_VALUE = 0xffffffff;

		public DebugRenderPrimitive()
		{
			transform = null;
			LocalBounds = new Bounds();
		}

		public DebugRenderPrimitive(Matrix4x4 inTransform, Vector3 inPivot, Vector3 inScale)
		{
			LocalBounds = new Bounds();
			this.transform = inTransform;
			this.pivot = inPivot;
			this.scale = inScale;
		}

		public int CompareTo(DebugRenderPrimitive other)
		{
			return Id < other.Id ? -1 : Id == other.Id ? 0 : 1;
		}

		public int StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				if (startTime != value)
				{
					startTime = value;

					NotifyPropertyChanged("StartTime");
					NotifyPropertyChanged("Duration");
				}
			}
		}

		public long Id
		{
			get
			{
				return id;
			}
			set
			{
				if (id != value)
				{
					id = value;

					NotifyPropertyChanged("Id");
				}
			}
		}

		public int Duration
		{
			get
			{
				return InfiniteLength ? int.MaxValue : (EndTime - StartTime);
			}
			set
			{
				EndTime = (value < 0) ? -1 : StartTime + value;
			}
		}

		public bool InfiniteLength
		{
			get
			{
				return EndTime < 0;
			}
		}

		public int EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				if (endTime != value)
				{
					endTime = value;

					NotifyPropertyChanged("EndTime");
					NotifyPropertyChanged("Duration");
					NotifyPropertyChanged("InfiniteLength");

					NotifyEndTimeChanged();
				}
			}
		}

		public Matrix4x4 GetTransform()
		{
			if (transform == null)
				return null;
			Matrix4x4 rotationTransform = transform;
			rotationTransform.Translation = new Vector3(0, 0, 0);

			Matrix4x4 scaleTransform = new Matrix4x4();
			scaleTransform.Scale(scale);

			Matrix4x4 pivotTransform = new Matrix4x4();
			pivotTransform.Translation += pivot;

			Matrix4x4 final = new Matrix4x4();
			final *= pivotTransform;
			final *= rotationTransform;
			final *= scaleTransform;
			final.Translation = final.Translation + transform.Translation;

			return final;
		}

		public Color32 PrimitiveColor
		{
			get;
			protected set;
		}

		public DebugRenderAnnotation Annotation
		{
			get;
			set;
		}

		public bool IsValidAt(int time)
		{
			return time >= startTime && (endTime < 0 || time < endTime);
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		public virtual void AddLines(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData) { }
		public virtual void AddTriangles(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData) { }
		public virtual void AddTriangles(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData, ArrayList<uint> opaqueIndexData, ArrayList<uint> transparentIndexData) { }

		public Bounds LocalBounds
		{
			get;
			set;
		}

		public Bounds WorldBounds
		{
			get
			{
				return transform != null ? GetTransform() * LocalBounds : LocalBounds;
			}
		}

		public virtual bool GetTriangle(int triangleIndex, bool isOpaque, out Vector3 a, out Vector3 b, out Vector3 c)
		{
			a = b = c = new Vector3(0, 0, 0);
			return false;
		}

		public virtual Color32 GetColor(int triangleIndex, bool isOpaque)
		{
			return PrimitiveColor != null ? PrimitiveColor : new Color32(0, 0, 0, 0);
		}

		protected virtual bool RayCheckInternal(int time, Vector3 rayStart, Vector3 rayDirection, out int triangleIndex, out bool hitOpaque, out Vector3 intersection)
		{
			hitOpaque = false;
			triangleIndex = -1;
			double tmin, tmax;
			Bounds bounds = WorldBounds;
			GeometryMath.IntersectAABB(rayStart, rayDirection, bounds.Min, bounds.Max, out tmin, out tmax);
			intersection = WorldBounds.Center - rayDirection.GetNormalized() * Math.Min(tmin, tmax);
			return true;
		}

		public bool RayCheck(int time, Vector3 rayStart, Vector3 rayDirection, out int triangleIndex, out bool hitOpaque, out Vector3 intersection)
		{
			hitOpaque = false;
			triangleIndex = 0;
			intersection = new Vector3(0, 0, 0);
			double tmin, tmax;
			Bounds bounds = WorldBounds;
			if (GeometryMath.IntersectAABB(rayStart, rayDirection, bounds.Min, bounds.Max, out tmin, out tmax))
			{
				return RayCheckInternal(time, rayStart, rayDirection, out triangleIndex, out hitOpaque, out intersection);
			}

			return false;
		}

		public virtual PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new PrimitiveSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}

		protected void NotifyEndTimeChanged()
		{
			PrimitiveEndTimeChanged handler = OnPrimitiveEndTimeChanged;
			if (handler != null)
			{
				handler(this);
			}
		}
		
		public delegate void PrimitiveEndTimeChanged(DebugRenderPrimitive primitive);
		public event PrimitiveEndTimeChanged OnPrimitiveEndTimeChanged;

		private long id;
		private int startTime;
		private int endTime;

		private Matrix4x4 transform;
		private Vector3 pivot;
		private Vector3 scale;
	}

	public class PrimitiveSelectionInfo
	{
		public PrimitiveSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
		{
			Primitive = selectedPrimitive;
			TriangleIndex = triangleIndex;
			IsSelectedTriangleOpaque = isSelectedTriangleOpaque;
		}

		protected DebugRenderPrimitive Primitive
		{
			get;
			set;
		}

		protected int TriangleIndex
		{
			get;
			set;
		}

		protected bool IsSelectedTriangleOpaque
		{
			get;
			set;
		}
	}
}
