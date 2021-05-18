using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Utility class to do some simple matrix calculations.
	/// </summary>
	[Serializable]
	public class Matrix4x4
	{
		// Static identity matrix
		public static readonly Matrix4x4 Identity = new Matrix4x4();

		// Initialize to identity
		public Matrix4x4()
		{
			mat = new double[] { 1, 0, 0, 0,  0, 1, 0, 0,  0, 0, 1, 0,  0, 0, 0, 1 };
		}

		// Initialize from quaternion
		public Matrix4x4(Quaternion q)
		{
			Quaternion qNormalized = q.Normalize();
			double qx = qNormalized.x;
			double qy = qNormalized.y;
			double qz = qNormalized.z;
			double qw = qNormalized.w;
			mat = new double[] {	1.0 - 2.0 * qy * qy - 2.0 * qz * qz,	2.0 * qx * qy - 2.0 * qw * qz,			2.0 * qx *qz + 2.0 * qw * qy,			0.0,
									2.0 * qx * qy + 2.0 * qw * qz,			1.0 - 2.0 * qx * qx - 2.0 * qz * qz,	2.0 * qy * qz - 2.0 * qw * qx,			0.0,
									2.0 * qx * qz - 2.0 * qw * qy,			2.0 * qy * qz + 2.0 * qw * qx,			1.0 - 2.0 * qx * qx - 2.0 * qy * qy,	0.0,
									0.0,									0.0,									0.0,									1.0};
		}

		// Initialize from 'other'
		public Matrix4x4(Matrix4x4 other)
		{
			mat = new double[16];
			for (int i = 0; i < 16; i++)
			{
				this[i] = other[i];
			}
		}

		// Check if identity
		public bool IsIdentity
		{
			get
			{
				return this == Identity;
			}
		}

		// Get component at 'index'
		public double this[int index]
		{
			get
			{
				return mat[index];
			}
			set
			{
				mat[index] = value;
			}
		}

		// Check equality
		public static bool operator ==(Matrix4x4 a, Matrix4x4 b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			for (int i = 0; i < 16; i++)
			{
				if (a.mat[i] != b.mat[i])
					return false;
			}
			return true;
		}

		// Check inequality
		public static bool operator !=(Matrix4x4 a, Matrix4x4 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			Matrix4x4 other = obj as Matrix4x4;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Scale matrix
		public void Scale(Vector3 scale)
		{
			for (int i = 0; i < 4; i++)
			{
				mat[i * 4 + COL_0] *= scale.x;
				mat[i * 4 + COL_1] *= scale.y;
				mat[i * 4 + COL_2] *= scale.z;
			}
		}

		// Multiply matrices (pre-multiplication)
		public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
		{
			Matrix4x4 result = new Matrix4x4();
			for (int col = 0; col < 4; col++)
			{
				for (int row = 0; row < 4; row++) 
				{
					double val = 0.0;
					for (int k = 0 ; k < 4 ; k++)
					{
						val += b[row * 4 + k] * a[k * 4 + col];
					}
					result[row * 4 + col] = val;
				}
			}
			return result;
		}

		// Invert matrix (http://stackoverflow.com/a/9614511)
		public Matrix4x4 Invert()
		{
			var s0 = mat[M00] * mat[M11] - mat[M10] * mat[M01];
			var s1 = mat[M00] * mat[M12] - mat[M10] * mat[M02];
			var s2 = mat[M00] * mat[M13] - mat[M10] * mat[M03];
			var s3 = mat[M01] * mat[M12] - mat[M11] * mat[M02];
			var s4 = mat[M01] * mat[M13] - mat[M11] * mat[M03];
			var s5 = mat[M02] * mat[M13] - mat[M12] * mat[M03];

			var c5 = mat[M22] * mat[M33] - mat[M32] * mat[M23];
			var c4 = mat[M21] * mat[M33] - mat[M31] * mat[M23];
			var c3 = mat[M21] * mat[M32] - mat[M31] * mat[M22];
			var c2 = mat[M20] * mat[M33] - mat[M30] * mat[M23];
			var c1 = mat[M20] * mat[M32] - mat[M30] * mat[M22];
			var c0 = mat[M20] * mat[M31] - mat[M30] * mat[M21];

			double det = (s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0);
			if (det == 0.0)
			{
				return null;
			}

			var invdet = 1.0 / det;

			Matrix4x4 result = new Matrix4x4();

			result[M00] = (mat[M11] * c5 - mat[M12] * c4 + mat[M13] * c3) * invdet;
			result[M01] = (-mat[M01] * c5 + mat[M02] * c4 - mat[M03] * c3) * invdet;
			result[M02] = (mat[M31] * s5 - mat[M32] * s4 + mat[M33] * s3) * invdet;
			result[M03] = (-mat[M21] * s5 + mat[M22] * s4 - mat[M23] * s3) * invdet;

			result[M10] = (-mat[M10] * c5 + mat[M12] * c2 - mat[M13] * c1) * invdet;
			result[M11] = (mat[M00] * c5 - mat[M02] * c2 + mat[M03] * c1) * invdet;
			result[M12] = (-mat[M30] * s5 + mat[M32] * s2 - mat[M33] * s1) * invdet;
			result[M13] = (mat[M20] * s5 - mat[M22] * s2 + mat[M23] * s1) * invdet;

			result[M20] = (mat[M10] * c4 - mat[M11] * c2 + mat[M13] * c0) * invdet;
			result[M21] = (-mat[M00] * c4 + mat[M01] * c2 - mat[M03] * c0) * invdet;
			result[M22] = (mat[M30] * s4 - mat[M31] * s2 + mat[M33] * s0) * invdet;
			result[M23] = (-mat[M20] * s4 + mat[M21] * s2 - mat[M23] * s0) * invdet;

			result[M30] = (-mat[M10] * c3 + mat[M11] * c1 - mat[M12] * c0) * invdet;
			result[M31] = (mat[M00] * c3 - mat[M01] * c1 + mat[M02] * c0) * invdet;
			result[M32] = (-mat[M30] * s3 + mat[M31] * s1 - mat[M32] * s0) * invdet;
			result[M33] = (mat[M20] * s3 - mat[M21] * s1 + mat[M22] * s0) * invdet;

			return result;
		}

		// Transform vector
		public static Vector3 operator *(Matrix4x4 a, Vector3 b)
		{
			Vector3 result = new Vector3(	a[ROW_0 + COL_0] * b.x + a[ROW_1 + COL_0] * b.y + a[ROW_2 + COL_0] * b.z + a[ROW_3 + COL_0],
											a[ROW_0 + COL_1] * b.x + a[ROW_1 + COL_1] * b.y + a[ROW_2 + COL_1] * b.z + a[ROW_3 + COL_1],
											a[ROW_0 + COL_2] * b.x + a[ROW_1 + COL_2] * b.y + a[ROW_2 + COL_2] * b.z + a[ROW_3 + COL_2]);

			return result;
		}

		// Transform vector
		public static Vector4 operator *(Matrix4x4 a, Vector4 b)
		{
			Vector4 result = new Vector4(	a[ROW_0 + COL_0] * b.x + a[ROW_1 + COL_0] * b.y + a[ROW_2 + COL_0] * b.z + a[ROW_3 + COL_0] * b.w,
											a[ROW_0 + COL_1] * b.x + a[ROW_1 + COL_1] * b.y + a[ROW_2 + COL_1] * b.z + a[ROW_3 + COL_1] * b.w,
											a[ROW_0 + COL_2] * b.x + a[ROW_1 + COL_2] * b.y + a[ROW_2 + COL_2] * b.z + a[ROW_3 + COL_2] * b.w,
											a[ROW_0 + COL_3] * b.x + a[ROW_1 + COL_3] * b.y + a[ROW_2 + COL_3] * b.z + a[ROW_3 + COL_3] * b.w);

			return result;
		}

		// Get/Set translation of this matrix
		public Vector3 Translation
		{
			get
			{
				return new Vector3(mat[ROW_3 + COL_0], mat[ROW_3 + COL_1], mat[ROW_3 + COL_2]);
			}
			set
			{
				mat[ROW_3 + COL_0] = value.x;
				mat[ROW_3 + COL_1] = value.y;
				mat[ROW_3 + COL_2] = value.z;
			}
		}

		private static int ROW_0 = 0 * 4;
		private static int ROW_1 = 1 * 4;
		private static int ROW_2 = 2 * 4;
		private static int ROW_3 = 3 * 4;

		private static int COL_0 = 0;
		private static int COL_1 = 1;
		private static int COL_2 = 2;
		private static int COL_3 = 3;

		private static int M00 = 0 * 4 + 0;
		private static int M01 = 0 * 4 + 1;
		private static int M02 = 0 * 4 + 2;
		private static int M03 = 0 * 4 + 3;
		private static int M10 = 1 * 4 + 0;
		private static int M11 = 1 * 4 + 1;
		private static int M12 = 1 * 4 + 2;
		private static int M13 = 1 * 4 + 3;
		private static int M20 = 2 * 4 + 0;
		private static int M21 = 2 * 4 + 1;
		private static int M22 = 2 * 4 + 2;
		private static int M23 = 2 * 4 + 3;
		private static int M30 = 3 * 4 + 0;
		private static int M31 = 3 * 4 + 1;
		private static int M32 = 3 * 4 + 2;
		private static int M33 = 3 * 4 + 3;

		public double[] mat;
	}
}
