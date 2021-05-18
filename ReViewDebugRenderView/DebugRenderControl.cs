using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ReView;
using System.Windows;
using SharpGL;
using System.Windows.Threading;
using System.Threading;
using SharpGL.SceneGraph;

namespace ReViewDebugRenderView
{
	public enum ControlKeys
	{
		W = 0,
		A,
		S,
		D,
		E,
		Q,
		Alt,
		Shift,
		Ctrl,
		Num
	}

	public partial class DebugRenderControl : UserControl
	{
		public DebugRenderControl()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.Selectable, false);

			gl = openGLControl.OpenGL;
			openGLControl.OpenGLInitialized += this.OpenGLControl_OpenGLInitialized;
			openGLControl.OpenGLDraw += this.OpenGLControl_OpenGLDraw;

			RenderInfo = new RenderInfo() { time = 0, normalColor = new Color32(255, 255, 255, 255), normalLength = 0.05, showLineNormals = true, showTriangleNormals = false, showGrid = true };

			SizeChanged += OnSizeChanged;

			Viewport = new Viewport();
			ProjectionMatrix = new Matrix4x4();

			ClearColor = new Color32(32, 32, 32, 255);
			GridColor = new Color32(128, 128, 128, 255);

			PerspectiveCamera = new PerspectiveCamera() { Near = 0.01, Far = 1000.0, FOV = 60.0, Position = new Vector3(0, 0, 5), Aspect = 1.0, Direction = new Vector3(0, 0, -1) };
			OrthoCamera = new OrthoCamera() { Near = 0.01, Far = 1000.0, Aspect = 1.0, Position = new Vector3(0, 0, 5), InverseForward = true, Zoom = 0.5 };

			CurrentCamera = PerspectiveCamera;

			// Register IsVisible listener to catch when this window is being hidden (to start/stop camera update)
			VisibleChanged += OnVisibleChanged;

			openGLControl.KeyUp += OnKeyUp;
			openGLControl.KeyDown += OnKeyDown;
			openGLControl.MouseWheel += OnMouseWheel;

			openGLControl.MouseDown += OnViewportMouseDown;
			openGLControl.MouseUp += OnViewportMouseUp;
			openGLControl.MouseMove += OnViewportMouseMove;

			StartCameraUpdate();
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			//	Resize
			PerspectiveCamera.Aspect = (double)Width / (double)Height;
			OrthoCamera.Aspect = (double)Width / (double)Height;

			gl.Viewport(0, 0, Width, Height);
		}

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
			{
				// Not visible anymore -> Stop camera update
				StopCameraUpdate();
			}
			else
			{
				// Visible -> Start camera update
				StartCameraUpdate();
			}
		}

		private PerspectiveCamera PerspectiveCamera
		{
			get;
			set;
		}

		private OrthoCamera OrthoCamera
		{
			get;
			set;
		}

		private Camera CurrentCamera
		{
			get;
			set;
		}

		public Color32 ClearColor
		{
			get;
			set;
		}

		public Color32 GridColor
		{
			get;
			set;
		}

		public bool ShowAnnotations
		{
			get
			{
				return RenderInfo.showAnnotations;
			}
			set
			{
				RenderInfo.showAnnotations = value;
			}
		}

		public bool ShowGrid
		{
			get
			{
				return RenderInfo.showGrid;
			}
			set
			{
				RenderInfo.showGrid = value;
			}
		}

		public bool OrthoAlignmentInvert
		{
			get
			{
				return OrthoCamera.InverseForward;
			}
			set
			{
				OrthoCamera.InverseForward = value;
			}
		}

		public OrthoMode OrthoAlignment
		{
			get
			{
				return OrthoCamera.OrthoMode;
			}
			set
			{
				OrthoCamera.OrthoMode = value;
			}
		}

		public bool ShowLineNormals
		{
			get
			{
				return RenderInfo.showLineNormals;
			}
			set
			{
				if (RenderInfo.showLineNormals != value)
				{
					RenderInfo.showLineNormals = value;
					Session.DebugRenderStorage.Invalidate();
				}
			}
		}

		public bool ShowTriangleNormals
		{
			get
			{
				return RenderInfo.showTriangleNormals;
			}
			set
			{
				if (RenderInfo.showTriangleNormals != value)
				{
					RenderInfo.showTriangleNormals = value;
					Session.DebugRenderStorage.Invalidate();
				}
			}
		}

		public bool IsPerspective
		{
			get
			{
				return CurrentCamera.IsPerspective;
			}
			set
			{
				CurrentCamera = value ? (Camera)PerspectiveCamera : (Camera)OrthoCamera;
			}
		}

		public void FitToView()
		{
			Bounds bounds = Session.DebugRenderStorage.GetBounds(Session.PlaybackPosition);
			if (IsPerspective)
			{
				PerspectiveCamera.UpDirection = new Vector3(0, 1, 0);
				PerspectiveCamera.Direction = new Vector3(0, 0, -1);
				PerspectiveCamera.Position = new Vector3(0, 0, 5);
			}
			else
			{
				OrthoCamera.Angle = 0.0;
				if (bounds.IsValid)
				{
					OrthoCamera.Position = new Vector3(bounds.Center.x, bounds.Center.y, bounds.Max.z + 1.0);
					double scaleRate = bounds.HalfSize.MaxComponent() * 2.0;
					OrthoCamera.Zoom = 1.0 - (Math.Log10(scaleRate) / 10.0 + 0.5);
				}
				else
				{
					OrthoCamera.Position = new Vector3(0, 0, 1);
					OrthoCamera.Zoom = 0.5;
				}
			}
		}

		private Viewport Viewport
		{
			get;
			set;
		}

		private Matrix4x4 ProjectionMatrix
		{
			get;
			set;
		}

		/// <summary>
		/// Handles the OpenGLDraw event of the OpenGLControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
		private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
		{
			OpenGL gl = openGLControl.OpenGL;

			gl.ClearColor((float)ClearColor.R_Double, (float)ClearColor.G_Double, (float)ClearColor.B_Double, (float)ClearColor.A_Double);
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			UpdateCamera();

			// Render grid (helper)
			if (RenderInfo.showGrid)
			{
				OpenGLDrawHelper.DrawGrid_XY(gl, Session.DebugRenderStorage.GetBounds(RenderInfo.time), 4, 5, GridColor);
			}

			// Render 3D
			RenderInfo.time = Session.PlaybackPosition;
			Session.DebugRenderStorage.Render(RenderInfo);

			lock (RenderInfo)
			{
				// Render selection
				if (RenderInfo.Selection != null)
				{
					int second = DateTime.Now.Second;
					Color32 selectionColor = second % 2 == 0 ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 255);

					OpenGLDrawHelper.DrawBounds(gl, RenderInfo.Selection.Primitive.WorldBounds, selectionColor, true);

					if (RenderInfo.Selection.SelectedTriangleIndex >= 0)
					{
						Vector3 a, b, c;
						if (RenderInfo.Selection.Primitive.GetTriangle(RenderInfo.Selection.SelectedTriangleIndex, RenderInfo.Selection.SelectedTriangleIsOpaque, out a, out b, out c))
						{
							if (!RenderInfo.Selection.SelectedTriangleIsOpaque)
							{
								selectionColor.A_Double = 0.5;
							}

							OpenGLDrawHelper.DrawTriangle(gl, a, b, c, selectionColor, true);
						}
					}
				}

				// Render highlight
			
				if (RenderInfo.Highlight != null)
				{
					int second = DateTime.Now.Second;
					Color32 selectionColor = second % 2 == 0 ? new Color32(255, 255, 0, 255) : new Color32(128, 128, 128, 255);

					OpenGLDrawHelper.DrawBounds(gl, RenderInfo.Highlight.Primitive.WorldBounds, selectionColor, true);

					if (RenderInfo.Highlight.SelectedTriangleIndex >= 0)
					{
						Vector3 a, b, c;
						if (RenderInfo.Highlight.Primitive.GetTriangle(RenderInfo.Highlight.SelectedTriangleIndex, RenderInfo.Highlight.SelectedTriangleIsOpaque, out a, out b, out c))
						{
							if (!RenderInfo.Highlight.SelectedTriangleIsOpaque)
							{
								selectionColor.A_Double = 0.5;
							}

							OpenGLDrawHelper.DrawTriangle(gl, a, b, c, selectionColor, true);
						}
					}
				}
			}

			// Render 2D (Screen-space)
			gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
			gl.Disable(OpenGL.GL_DEPTH_TEST);

			double aspect = Viewport.width / Viewport.height;
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Perspective(60.0, aspect, 0.0, 10.0);

			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();

			OpenGLDrawHelper.DrawAxis(gl, new Vector3(-1.0 * aspect, -1.0, -2.0), CurrentCamera.Direction, CurrentCamera.UpDirection, 0.2);
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Ortho2D(0, Viewport.width, 0, Viewport.height);

			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();

			Session.DebugRenderStorage.RenderScreenSpace(RenderInfo, ProjectionMatrix, Viewport);

			gl.PopAttrib();
		}

		private RenderInfo RenderInfo
		{
			get;
			set;
		}

		public bool FSAA
		{
			get
			{
				return openGLControl.FSAA;
			}
			set
			{
				openGLControl.FSAA = value;
			}
		}

		public static OpenGL gl = null;

		/// <summary>
		/// Handles the OpenGLInitialized event of the OpenGLControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
		private void OpenGLControl_OpenGLInitialized(object sender, EventArgs args)
		{
			gl = openGLControl.OpenGL;

			gl.Enable(OpenGL.GL_DEPTH_TEST);

			float[] global_ambient = new float[] { 0.15f, 0.15f, 0.15f, 1.0f };
			gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

			float[] light0pos = new float[] { 0.0f, 0.0f, 1.0f, 0.0f };
			float[] light0ambient = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
			float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
			float[] light0specular = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };

			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
			gl.Enable(OpenGL.GL_LIGHTING);
			gl.Enable(OpenGL.GL_LIGHT0);

			gl.Enable(OpenGL.GL_NORMALIZE);

			gl.ShadeModel(OpenGL.GL_SMOOTH);

			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
			gl.Enable(OpenGL.GL_BLEND);
		}

		private void UpdateCamera()
		{
			// Setup projection matrix
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			if (IsPerspective)
			{
				gl.Perspective(PerspectiveCamera.FOV, PerspectiveCamera.Aspect, PerspectiveCamera.Near, PerspectiveCamera.Far);
			}
			else
			{
				gl.Ortho(OrthoCamera.Bounds.Min.x, OrthoCamera.Bounds.Max.x, OrthoCamera.Bounds.Min.y, OrthoCamera.Bounds.Max.y, OrthoCamera.Near, OrthoCamera.Far);
			}

			// Setup modelview matrix (camera position and rotation)
			gl.MatrixMode(OpenGL.GL_MODELVIEW);

			gl.LoadIdentity();

			if (IsPerspective)
			{
				gl.LookAt(	PerspectiveCamera.Position.x, PerspectiveCamera.Position.y, PerspectiveCamera.Position.z,
							PerspectiveCamera.Position.x + PerspectiveCamera.Direction.x, PerspectiveCamera.Position.y + PerspectiveCamera.Direction.y, PerspectiveCamera.Position.z + PerspectiveCamera.Direction.z,
							PerspectiveCamera.UpDirection.x, PerspectiveCamera.UpDirection.y, PerspectiveCamera.UpDirection.z);
			}
			else
			{
				gl.LookAt(	OrthoCamera.Position.x, OrthoCamera.Position.y, OrthoCamera.Position.z,
							OrthoCamera.Position.x + OrthoCamera.Direction.x, OrthoCamera.Position.y + OrthoCamera.Direction.y, OrthoCamera.Position.z + OrthoCamera.Direction.z,
							OrthoCamera.UpDirection.x, OrthoCamera.UpDirection.y, OrthoCamera.UpDirection.z);
			}

			// Update projection info
			Matrix4x4 projectionMatrix = new Matrix4x4();
			Matrix4x4 modelViewMatrix = new Matrix4x4();

			gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projectionMatrix.mat);
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelViewMatrix.mat);
			
			ProjectionMatrix = projectionMatrix * modelViewMatrix;

			gl.GetInteger(OpenGL.GL_VIEWPORT, Viewport.data);
		}

		private void StartCameraUpdate()
		{
			lock (cameraManagerLock)
			{
				if (cameraManagerThread == null)
				{
					cameraManagerThread = new Thread(new ThreadStart(CameraUpdate));
					cameraManagerThread.IsBackground = true;
					cameraManagerThread.Start();
				}
			}
		}

		private void StopCameraUpdate()
		{
			lock (cameraManagerLock)
			{
				if (cameraManagerThread != null)
				{
					cameraUpdate = false;
					cameraManagerThread.Join(100);
					cameraManagerThread = null;
				}
			}
		}

		private void CameraUpdate()
		{
			cameraUpdate = true;
			long previousMillis = DateTime.Now.Ticks / 10000;

			int updateFrequency = 50; // 50Hz

			while (cameraUpdate)
			{
				long millisNow = DateTime.Now.Ticks / 10000;
				long deltaMillis = (millisNow - previousMillis);
				double deltaTime = (double)deltaMillis / 1000.0;
				previousMillis = millisNow;

				double speed = deltaTime * (keysDown[(int)ControlKeys.Shift] ? 5.0 : 1.0);
				double moveSpeed = CameraMoveSpeed * speed;

				if (keysDown[(int)ControlKeys.W])
				{
					if (IsPerspective)
					{
						PerspectiveCamera.Position = PerspectiveCamera.Position + PerspectiveCamera.Direction * moveSpeed;
					}
					else
					{
						OrthoCamera.Zoom += speed * 0.1;
					}
				}
				if (keysDown[(int)ControlKeys.S])
				{
					if (IsPerspective)
					{
						PerspectiveCamera.Position = PerspectiveCamera.Position - PerspectiveCamera.Direction * moveSpeed;
					}
					else
					{
						OrthoCamera.Zoom -= speed * 0.1;
					}
				}
				if (keysDown[(int)ControlKeys.D])
				{
					CurrentCamera.Position = CurrentCamera.Position + CurrentCamera.SideDirection * moveSpeed;
				}
				if (keysDown[(int)ControlKeys.A])
				{
					CurrentCamera.Position = CurrentCamera.Position - CurrentCamera.SideDirection * moveSpeed;
				}
				if (keysDown[(int)ControlKeys.E])
				{
					CurrentCamera.Position = CurrentCamera.Position + CurrentCamera.UpDirection * moveSpeed;
				}
				if (keysDown[(int)ControlKeys.Q])
				{
					CurrentCamera.Position = CurrentCamera.Position - CurrentCamera.UpDirection * moveSpeed;
				}

				TimeSinceLastHighlightCheck += (double)(1000 / updateFrequency) / 1000.0;

				if (TimeSinceLastHighlightCheck > 0.1) // 10Hz
				{
					TimeSinceLastHighlightCheck = 0.0;

					// Check mouse over highlighting
					if (LastHighlightMousePosition != PreviousMousePosition)
					{
						LastHighlightMousePosition = PreviousMousePosition;

						Selection oldSelection = RenderInfo.Highlight;
						lock (RenderInfo)
						{
							RenderInfo.Highlight = GetSelection((int)PreviousMousePosition.X, (int)PreviousMousePosition.Y);
						}
						if (oldSelection != RenderInfo.Highlight)
						{
							this.BeginInvoke((MethodInvoker)delegate
							{
								NotifyHighlightChanged();
							});
						}
					}
				}

				Thread.Sleep(1000 / updateFrequency);
			}
		}

		private double TimeSinceLastHighlightCheck
		{
			get;
			set;
		}

		private Vector2 LastHighlightMousePosition
		{
			get;
			set;
		}

		public double CameraMoveSpeed
		{
			get
			{
				return cameraMoveSpeed;
			}
			set
			{
				if (cameraMoveSpeed != value)
				{
					cameraMoveSpeed = value;

					NotifyPropertyChanged("CameraMoveSpeed");
				}
			}
		}

		public double CameraAimSpeed
		{
			get
			{
				return cameraAimSpeed;
			}
			set
			{
				if (cameraAimSpeed != value)
				{
					cameraAimSpeed = value;

					NotifyPropertyChanged("CameraAimSpeed");
				}
			}
		}

		public DebugRenderSession Session
		{
			get
			{
				return session;
			}
			set
			{
				if (session != value)
				{
					session = value;

					NotifyPropertyChanged("Session");
				}
			}
		}

		public void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs args)
		{
			switch (args.KeyCode)
			{
				case System.Windows.Forms.Keys.W:
					{
						keysDown[(int)ControlKeys.W] = true;
					}
					break;
				case System.Windows.Forms.Keys.A:
					{
						keysDown[(int)ControlKeys.A] = true;
					}
					break;
				case System.Windows.Forms.Keys.S:
					{
						keysDown[(int)ControlKeys.S] = true;
					}
					break;
				case System.Windows.Forms.Keys.D:
					{
						keysDown[(int)ControlKeys.D] = true;
					}
					break;
				case System.Windows.Forms.Keys.E:
					{
						keysDown[(int)ControlKeys.E] = true;
					}
					break;
				case System.Windows.Forms.Keys.Q:
					{
						keysDown[(int)ControlKeys.Q] = true;
					}
					break;
				default:
					break;
			}
			keysDown[(int)ControlKeys.Shift] = args.Shift;
			keysDown[(int)ControlKeys.Ctrl] = args.Control;
			keysDown[(int)ControlKeys.Alt] = args.Alt;
		}

		public void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs args)
		{
			switch (args.KeyCode)
			{
				case System.Windows.Forms.Keys.W:
					{
						keysDown[(int)ControlKeys.W] = false;
					}
					break;
				case System.Windows.Forms.Keys.A:
					{
						keysDown[(int)ControlKeys.A] = false;
					}
					break;
				case System.Windows.Forms.Keys.S:
					{
						keysDown[(int)ControlKeys.S] = false;
					}
					break;
				case System.Windows.Forms.Keys.D:
					{
						keysDown[(int)ControlKeys.D] = false;
					}
					break;
				case System.Windows.Forms.Keys.E:
					{
						keysDown[(int)ControlKeys.E] = false;
					}
					break;
				case System.Windows.Forms.Keys.Q:
					{
						keysDown[(int)ControlKeys.Q] = false;
					}
					break;
				default:
					break;
			}
			keysDown[(int)ControlKeys.Shift] = args.Shift;
			keysDown[(int)ControlKeys.Ctrl] = args.Control;
			keysDown[(int)ControlKeys.Alt] = args.Alt;
		}

		public void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs args)
		{
		}

		public Vector2 PreviousMousePosition
		{
			get;
			set;
		}

		private Selection GetSelection(int screenX, int screenY)
		{
			// Try to see what can be selected
			Vector3 rayStart = GeometryMath.Unproject(new Vector3(screenX, Viewport.height - screenY, 0.1), ProjectionMatrix, Viewport);
			Vector3 rayEnd = GeometryMath.Unproject(new Vector3(screenX, Viewport.height - screenY, IsPerspective ? 1.0 : -1.0), ProjectionMatrix, Viewport);
			Vector3 rayDirection = (rayEnd - rayStart).GetNormalized();

			bool closestHitOpaque = false;
			double closestDistance = Double.MaxValue;
			int closestPrimitiveTriangleIndex = -1;
			DebugRenderPrimitive closestPrimitive = null;
			List<DebugRenderPrimitive> debugRenderPrimitives = Session.DebugRenderStorage.GetCachedPrimitives();
			foreach (DebugRenderPrimitive primitive in debugRenderPrimitives)
			{
				bool hitOpaque;
				int triangleIndex;
				Vector3 intersection = new Vector3(0, 0, 0);
				if (primitive.RayCheck(Session.PlaybackPosition, rayStart, rayDirection, out triangleIndex, out hitOpaque, out intersection))
				{
					double distance = rayStart.Distance(intersection);
					if (distance < closestDistance)
					{
						closestPrimitiveTriangleIndex = triangleIndex;
						closestDistance = distance;
						closestPrimitive = primitive;
						closestHitOpaque = hitOpaque;
					}
				}
			}

			return closestPrimitive != null ? new Selection(closestPrimitive, closestPrimitiveTriangleIndex, closestHitOpaque) : null;
		}

		public void OnViewportMouseDown(object sender, MouseEventArgs args)
		{
			Point mousePosition = new Point(args.X, args.Y);

			if (args.Button == System.Windows.Forms.MouseButtons.Right || args.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				// Drag start
				PreviousMousePosition = new Vector2(args.X, args.Y);

				Cursor.Hide();
			}
			else if (args.Button == System.Windows.Forms.MouseButtons.Left)
			{
				lock (RenderInfo)
				{
					RenderInfo.Selection = GetSelection(args.X, args.Y);
				}

				NotifySelectionChanged();
			}

			openGLControl.Focus();
		}

		public void OnViewportMouseUp(object sender, MouseEventArgs args)
		{
			Cursor = Cursors.Default;
			Cursor.Show();
		}

		public void OnViewportMouseMove(object sender, MouseEventArgs args)
		{
			Point mousePosition = new Point(args.X, args.Y);
			Vector2 winformsMousePosition = new Vector2(args.X, args.Y);
			if (PreviousMousePosition == null)
			{
				PreviousMousePosition = new Vector2(winformsMousePosition);
			}
			Vector2 mouseDelta = winformsMousePosition - PreviousMousePosition;
			PreviousMousePosition = winformsMousePosition;

			if (args.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (IsPerspective)
				{
					// Take yaw/pitch from mouse movement
					double yawDelta = SMath.Clamp(mouseDelta.X * CameraAimSpeed, -15.0, 15.0);
					double pitchDelta = SMath.Clamp(mouseDelta.Y * CameraAimSpeed, -15.0, 15.0);

					Matrix4x4 yawRotMat = new Matrix4x4(new Quaternion(PerspectiveCamera.UpDirection, SMath.DEG2RAD * yawDelta));
					PerspectiveCamera.Direction = (yawRotMat * PerspectiveCamera.Direction).GetNormalized();

					Matrix4x4 pitchRotMat = new Matrix4x4(new Quaternion(PerspectiveCamera.SideDirection, SMath.DEG2RAD * pitchDelta));
					PerspectiveCamera.Direction = (pitchRotMat * PerspectiveCamera.Direction).GetNormalized();
				}
				else
				{
					OrthoCamera.Angle += SMath.DEG2RAD * SMath.Clamp(-mouseDelta.X * CameraAimSpeed, -15.0, 15.0);
				}

				if (winformsMousePosition.X > Width || winformsMousePosition.Y > Height || winformsMousePosition.X < 0 || winformsMousePosition.Y < 0)
				{
					PreviousMousePosition = new Vector2(Location.X + Width / 2, Location.Y + Height / 2);
					Cursor.Position = this.PointToScreen(PreviousMousePosition.ToPoint());
				}
			}
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

		protected void NotifySelectionChanged()
		{
			SelectionChanged handler = OnSelectionChanged;
			if (handler != null)
			{
				if (RenderInfo.Selection != null)
				{
					handler(RenderInfo.Selection.Primitive, RenderInfo.Selection.SelectedTriangleIndex, RenderInfo.Selection.SelectedTriangleIsOpaque);
				}
				else
				{
					handler(null, -1, false);
				}
			}
		}

		protected void NotifyHighlightChanged()
		{
			SelectionChanged handler = OnHighlightChanged;
			if (handler != null)
			{
				if (RenderInfo.Highlight != null)
				{
					handler(RenderInfo.Highlight.Primitive, RenderInfo.Highlight.SelectedTriangleIndex, RenderInfo.Highlight.SelectedTriangleIsOpaque);
				}
				else
				{
					handler(null, -1, false);
				}
			}
		}

		public delegate void SelectionChanged(DebugRenderPrimitive selectedPrimitive, int selectedTriangleIndex, bool isSelectedTriangleOpaque);

		public event SelectionChanged OnSelectionChanged;
		public event SelectionChanged OnHighlightChanged;

		// Camera
		private double cameraMoveSpeed = 1.0;
		private double cameraAimSpeed = 0.25;

		// Update thread for camera movement
		private Thread cameraManagerThread;
		private object cameraManagerLock = new object();
		private bool cameraUpdate = false;
		private bool[] keysDown = new bool[(int)ControlKeys.Num];

		// Session data
		private DebugRenderSession session;
	}
}
