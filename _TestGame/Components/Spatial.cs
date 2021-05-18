using _TestGame.Managers;
using ReView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Components
{
	/// <summary>
	/// Spatial objects have local matrix and a method to recurse through the spatial hierarchy and calculate a world matrix
	/// </summary>
	public class Spatial : Component
	{
		public Spatial()
		{
			debugLocalMatrix = null;
			localMatrix = new Matrix();
		}

		public Vector2 WorldPosition
		{
			get
			{
				Matrix worldMatrix = GetWorldMatrix();
				return new Vector2(worldMatrix.OffsetX, worldMatrix.OffsetY);
			}
		}

		public float WorldHeading
		{
			get
			{
				Matrix worldMatrix = GetWorldMatrix();

				PointF edge = new PointF(1.0f, 0.0f);
				PointF[] points = { edge };
				worldMatrix.TransformVectors(points);

				Vector2 direction = new Vector2(points[0].X, points[0].Y);

				return (float)direction.GetAngle() * SMath.RAD2DEG;
			}
		}

		public Vector2 WorldForward
		{
			get
			{
				Matrix worldMatrix = GetWorldMatrix();

				PointF edge = new PointF(1.0f, 0.0f);
				PointF[] points = { edge };
				worldMatrix.TransformVectors(points);

				Vector2 direction = new Vector2(points[0].X, points[0].Y);

				return direction.Normalize();
			}
		}

		public void SetLocalMatrix(Matrix inMatrix)
		{
			localMatrix = inMatrix;
		}

		public void SetPosition(Vector2 inPosition)
		{
			Matrix newMatrix = new Matrix(localMatrix.Elements[0], localMatrix.Elements[1], localMatrix.Elements[2], localMatrix.Elements[3], inPosition.X, inPosition.Y);

			SetLocalMatrix(newMatrix);
		}

		public void SetFacing(float inAngleInDegrees)
		{
			Matrix newMatrix = new Matrix();
			
			newMatrix.Translate(localMatrix.OffsetX, localMatrix.OffsetY);
			newMatrix.Rotate(inAngleInDegrees);

			SetLocalMatrix(newMatrix);
		}

		public Matrix GetLocalMatrix()
		{
			return debugLocalMatrix != null ? debugLocalMatrix : localMatrix;
		}

		public Matrix GetWorldMatrix()
		{
			Matrix worldMatrix = GetLocalMatrix();
			if (parent != null)
			{
				Matrix mat = parent.GetWorldMatrix();
				mat.Multiply(worldMatrix, MatrixOrder.Append);
				return mat;
			}
			return worldMatrix;
		}

		private void AddChild(Spatial inChild)
		{
			children.Add(inChild);
		}

		private void RemoveChild(Spatial inChild)
		{
			children.Remove(inChild);
		}

		public Spatial Parent
		{
			get
			{
				return parent;
			}
			set
			{
				if (parent != value)
				{
					if (parent != null)
					{
						// Remove child from old parent
						parent.RemoveChild(this);
					}

					// Assign new parent
					parent = value;

					if (parent != null)
					{
						// Add child for new parent
						parent.AddChild(this);
					}
				}
			}
		}

		public override void OnInit()
		{
			base.OnInit();

#if _REVIEW_DEBUG
			binaryDataFeed = ReViewFeedManager.Instance.RegisterBinaryDataFeed();
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived += OnDataReceived;
			}
#endif
		}

		public override void OnUninit()
		{
			base.OnUninit();

#if _REVIEW_DEBUG
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived -= OnDataReceived;
				ReViewFeedManager.Instance.UnregisterBinaryDataFeed(binaryDataFeed);
			}
#endif
		}

#if _REVIEW_DEBUG
		public override void OnGamePlayStateChanged(bool isPlaying)
		{
			base.OnGamePlayStateChanged(isPlaying);

			if (isPlaying)
			{
				debugLocalMatrix = null;
			}
		}

		protected void OnDataReceived(int time, ref byte[] data)
		{
			// Pause game if receiving data
			GameUpdateManager.Instance.SetRunning(false, true);

			if (!GameUpdateManager.Instance.Running)
			{
				float[] values = new float[localMatrix.Elements.Count()];
				for (int i = 0; i < values.Count(); i++)
				{
					values[i] = BitConverter.ToSingle(data, i * 4);
				}
				debugLocalMatrix = new Matrix(values[0], values[1], values[2], values[3], values[4], values[5]);
			}
		}
#endif

		public override void OnPostUpdate()
		{
			base.OnPostUpdate();

#if _REVIEW_DEBUG
			if (GameUpdateManager.Instance.Running && binaryDataFeed != null)
			{
				MemoryStream stream = new MemoryStream();
				for (int i = 0; i < localMatrix.Elements.Count(); i++)
				{
					stream.Write(BitConverter.GetBytes(localMatrix.Elements[i]), 0, 4);
				}
				byte[] buffer = stream.ToArray();
				binaryDataFeed.Store(GameUpdateManager.Instance.Now, ref buffer);
			}
#endif
		}

		private Matrix debugLocalMatrix;
		private Matrix localMatrix;

		private Spatial parent;
		private List<Spatial> children = new List<Spatial>();


#if _REVIEW_DEBUG
		private ReViewFeedBinaryData binaryDataFeed;
#endif
	}
}
