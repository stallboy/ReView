using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReView
{
	public static class Conversion
	{
		public static System.Drawing.Point ToPoint(this Vector2 inVector)
		{
			return new System.Drawing.Point((int)inVector.X, (int)inVector.Y);
		}

		public static System.Drawing.PointF ToPointF(this Vector2 inVector)
		{
			return new System.Drawing.PointF((float)inVector.X, (float)inVector.Y);
		}

		public static System.Windows.Media.Media3D.Vector3D ToVector3D(this Vector3 orig)
		{
			return new System.Windows.Media.Media3D.Vector3D(orig.x, orig.y, orig.z);
		}

		public static System.Windows.Media.Media3D.Matrix3D ToMatrix3D(this Matrix4x4 orig)
		{
			return new System.Windows.Media.Media3D.Matrix3D(orig[0], orig[1], orig[2], orig[3],
								orig[4], orig[5], orig[6], orig[7],
								orig[8], orig[9], orig[10], orig[11],
								orig[12], orig[13], orig[14], orig[15]);
		}

		public static System.Windows.Media.Color ToMediaColor(this Color32 orig)
		{
			return System.Windows.Media.Color.FromArgb(orig.A, orig.R, orig.G, orig.B);
		}

		public static System.Drawing.Color ToDrawingColor(this Color32 orig)
		{
			return System.Drawing.Color.FromArgb(orig.A, orig.R, orig.G, orig.B);
		}
	}
}
