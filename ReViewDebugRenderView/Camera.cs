using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReViewDebugRenderView
{
	public enum OrthoMode
	{
		XY,
		XZ,
		YZ,
		YX,
		ZX,
		ZY
	}

	public class OrthoCamera : Camera
	{
		public OrthoCamera()
		{
			IsPerspective = false;
			Zoom = 0.5;
			Angle = 0.0;
			OrthoMode = OrthoMode.XY;
			Position = new Vector3(0, 0, 0);
			UpDirection = new Vector3(0, 1, 0);
			Direction = new Vector3(0, 0, 1);
		}

		public OrthoMode OrthoMode
		{
			get;
			set;
		}

		public double Zoom
		{
			get
			{
				return zoom;
			}
			set
			{
				zoom = SMath.Clamp(value, 0.0, 1.0);
			}
		}

		public double Angle
		{
			get;
			set;
		}

		public bool InverseForward
		{
			get;
			set;
		}

		public override Vector3 Direction
		{
			get
			{
				switch (OrthoMode)
				{
					case OrthoMode.YZ:
					case OrthoMode.ZY:
						return new Vector3(InverseForward ? -1 : 1, 0, 0);
					case OrthoMode.XZ:
					case OrthoMode.ZX:
						return new Vector3(0, InverseForward ? -1 : 1, 0);
					case OrthoMode.XY:
					case OrthoMode.YX:
						return new Vector3(0, 0, InverseForward ? -1 : 1);
					default:
						return new Vector3(0, 0, InverseForward ? -1 : 1);
				}
			}
			set { }
		}

		public override Vector3 UpDirection
		{
			get
			{
				Matrix4x4 m = new Matrix4x4(new Quaternion(Direction, Angle));
				switch (OrthoMode)
				{
					case OrthoMode.YX:
					case OrthoMode.ZX:
						return m * new Vector3(1, 0, 0);
					case OrthoMode.XY:
					case OrthoMode.ZY:
						return m * new Vector3(0, 1, 0);

					case OrthoMode.XZ:
					case OrthoMode.YZ:
						return m * new Vector3(0, 0, 1);
					default:
						return m * new Vector3(0, 1, 0);
				}
			}
			set { }
		}

		public Bounds Bounds
		{
			get
			{
				double zoomFactor = Math.Pow(10.0, (Zoom - 0.5) * 10);
				return new Bounds(new Vector3(-0.5 * Aspect / zoomFactor, -0.5 / zoomFactor, 0.0), new Vector3(0.5 * Aspect / zoomFactor, 0.5 / zoomFactor, 0.0));
			}
		}

		private double zoom;
	}

	public class PerspectiveCamera : Camera
	{
		public PerspectiveCamera()
		{
			IsPerspective = true;
			Position = new Vector3(0, 0, 0);
			UpDirection = new Vector3(0, 1, 0);
			Direction = new Vector3(0, 0, 1);
		}

		public double FOV
		{
			get;
			set;
		}

		public override Vector3 Direction
		{
			get { return direction; }
			set { direction = value; UpDirection = SideDirection.Cross(direction); }
		}

		private Vector3 direction;
	}

	public abstract class Camera
	{
		public double Aspect
		{
			get;
			set;
		}

		public bool IsPerspective
		{
			get;
			protected set;
		}

		public double Near
		{
			get;
			set;
		}

		public double Far
		{
			get;
			set;
		}

		public Vector3 Position
		{
			get;
			set;
		}

		public virtual Vector3 Direction
		{
			get;
			set;
		}

		public virtual Vector3 UpDirection
		{
			get;
			set;
		}

		public virtual Vector3 SideDirection
		{
			get
			{
				return Direction.Cross(UpDirection).GetNormalized();
			}
		}
	}
}
