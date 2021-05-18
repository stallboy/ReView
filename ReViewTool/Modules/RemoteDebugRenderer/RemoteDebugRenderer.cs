using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ReViewDebugRenderView;
using ReView;
using System.Windows.Media.Media3D;
using ReViewDebugRenderView.Primitives;
using ReViewTool.Modules.RemoteDebugRenderer;
using ReViewRPC;
using ReViewTool.Modules.RemoteDebugRender;

namespace ReViewTool
{
	public partial class RemoteDebugRenderer : DebugModule, IReView_RemoteDebugRenderer
	{
		public RemoteDebugRenderer()
		{
			InitializeComponent();

			DisableUIEvents = false;

			RDRButtonContainer = new RDRButtonContainer();

			UserPreferencesManager.Instance.OnUserPreferencesChanged += OnUserPreferencesChanged;
		}

		private void OnUserPreferencesChanged(UserPreferences source)
		{
			SynchronizeButtonContainer();
		}

		private bool DisableUIEvents
		{
			get;
			set;
		}
		
		#region DebugModule Implementation
		
		public override string ModuleName
		{
			get
			{
				return "Debug Renderer";
			}
		}

		public override void OnInitDebugModule()
		{
			Session = new RDRSession();
		}

		public override void OnActivateDebugModule()
		{
			RequestTimelineMarginChange(0, 0);

			NotifyDurationChanged(Session.Duration);
		}

		public override void OnDeactivateDebugModule()
		{
			FindForm().KeyUp -= debugRenderControl.OnKeyUp;
			FindForm().KeyDown -= debugRenderControl.OnKeyDown;
			FindForm().MouseWheel -= debugRenderControl.OnMouseWheel;
		}

		public override void OnRPCStateChanged(bool connected)
		{
			if (connected)
			{
				RPC_Manager.Instance.Create_Server_Proxy<RPC_Server_Proxy_IReView_RemoteDebugRenderer, IReView_RemoteDebugRenderer>(this);
			}
		}

		public override void OnResetSessions()
		{
			if (InvokeRequired)
			{
				if (IsHandleCreated)
				{
					this.BeginInvoke((MethodInvoker)delegate
					{
						Session = new RDRSession();
					});
				}
			} 
			else if (IsHandleCreated)
			{
				Session = new RDRSession();
			}
		}

		public override void OnHeartbeat(int time)
		{
			Session.UpdateDuration(time);
		}

		public override void OnTimelinePlaybackPositionChanged(int playbackPosition)
		{
			if (InvokeRequired)
			{
				if (IsHandleCreated)
				{
					this.BeginInvoke((MethodInvoker)delegate
					{
						Session.PlaybackPosition = playbackPosition;
					});
				}
			}
			else if (IsHandleCreated)
			{
				Session.PlaybackPosition = playbackPosition;
			}
		}

		public override FlowLayoutPanel GetToolbarButtonFlowLayout()
		{
			return RDRButtonContainer.GetFlowLayoutPanel();
		}

		public override int GetNextTimelineEventTime(int time) 
		{
			if (Session != null && Session.DebugRenderStorage != null)
			{
				return Session.DebugRenderStorage.GetNextTimelineEventTime(time);
			}
			return -1; 
		}

		public override int GetPrevTimelineEventTime(int time) 
		{
			if (Session != null && Session.DebugRenderStorage != null)
			{
				return Session.DebugRenderStorage.GetPrevTimelineEventTime(time);
			}
			return -1; 
		}

		#endregion

		public RDRButtonContainer RDRButtonContainer
		{
			get
			{
				return rdrButtonContainer;
			}
			set
			{
				if (rdrButtonContainer != value)
				{
					if (rdrButtonContainer != null)
					{
						rdrButtonContainer.GetToggleAnnotations().CheckedChanged -= toggleAnnotations_CheckedChanged;
						rdrButtonContainer.GetToggleDimensions().CheckedChanged -= toggleDimensions_CheckedChanged;
						rdrButtonContainer.GetToggleFSAA().CheckedChanged -= toggleFSAA_CheckedChanged;
						rdrButtonContainer.GetFitToView().Click -= fitToView_Click;
						rdrButtonContainer.GetClearColor().Click -= clearColor_Click;
						rdrButtonContainer.GetCycleNormalsButton().Click -= cycleNormals_Click;
						rdrButtonContainer.GetToggleGrid().CheckedChanged -= toggleGrid_CheckedChanged;
					}

					rdrButtonContainer = value;

					if (rdrButtonContainer != null)
					{
						SynchronizeButtonContainer();

						rdrButtonContainer.GetToggleAnnotations().CheckedChanged += toggleAnnotations_CheckedChanged;
						rdrButtonContainer.GetToggleDimensions().CheckedChanged += toggleDimensions_CheckedChanged;
						rdrButtonContainer.GetToggleFSAA().CheckedChanged += toggleFSAA_CheckedChanged;
						rdrButtonContainer.GetFitToView().Click += fitToView_Click;
						rdrButtonContainer.GetClearColor().Click += clearColor_Click;
						rdrButtonContainer.GetCycleNormalsButton().Click += cycleNormals_Click;
						rdrButtonContainer.GetToggleGrid().CheckedChanged += toggleGrid_CheckedChanged;
					}
				}
			}
		}

		private void SynchronizeButtonContainer()
		{
			if (UserPreferencesManager.Instance.UserPreferences == null)
			{
				return;
			}

			debugRenderControl.ClearColor = UserPreferencesManager.Instance.UserPreferences.DebugRenderClearColor;
			debugRenderControl.FSAA = UserPreferencesManager.Instance.UserPreferences.DebugRenderFSAA;
			debugRenderControl.IsPerspective = UserPreferencesManager.Instance.UserPreferences.DebugRender3DView;
			debugRenderControl.ShowLineNormals = (UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals == ShowNormalsMode.Lines);
			debugRenderControl.ShowTriangleNormals = (UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals == ShowNormalsMode.Triangles);
			debugRenderControl.ShowAnnotations = UserPreferencesManager.Instance.UserPreferences.DebugRenderAnnotations;
			debugRenderControl.ShowGrid = UserPreferencesManager.Instance.UserPreferences.DebugRenderShowGrid;

			DisableUIEvents = true;

			rdrButtonContainer.GetToggleAnnotations().Checked = UserPreferencesManager.Instance.UserPreferences.DebugRenderAnnotations;
			rdrButtonContainer.GetToggleAnnotations().Text = rdrButtonContainer.GetToggleAnnotations().Checked ? "Annotations (All)" : "Annotations (None)";
			
			rdrButtonContainer.GetToggleDimensions().Checked = UserPreferencesManager.Instance.UserPreferences.DebugRender3DView;
			rdrButtonContainer.GetToggleDimensions().Text = rdrButtonContainer.GetToggleDimensions().Checked ? "3D" : "2D";
	
			rdrButtonContainer.GetToggleFSAA().Checked = UserPreferencesManager.Instance.UserPreferences.DebugRenderFSAA;
			rdrButtonContainer.GetToggleFSAA().Text = rdrButtonContainer.GetToggleFSAA().Checked ? "+ FSAA" : "- FSAA";

			System.Drawing.Color clearColor = System.Drawing.Color.FromArgb(UserPreferencesManager.Instance.UserPreferences.DebugRenderClearColor.R,
																			UserPreferencesManager.Instance.UserPreferences.DebugRenderClearColor.G,
																			UserPreferencesManager.Instance.UserPreferences.DebugRenderClearColor.B);
			rdrButtonContainer.GetClearColor().BackColor = clearColor;
			rdrButtonContainer.GetClearColor().ForeColor = clearColor.GetBrightness() >= 0.5 ? System.Drawing.Color.Black : System.Drawing.Color.White;

			switch (UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals)
			{
				case ShowNormalsMode.None:
					rdrButtonContainer.GetCycleNormalsButton().Text = "Normals (None)";
				break;
				case ShowNormalsMode.Lines:
					rdrButtonContainer.GetCycleNormalsButton().Text = "Normals (Lines)";
				break;
				case ShowNormalsMode.Triangles:
					rdrButtonContainer.GetCycleNormalsButton().Text = "Normals (Triangles)";
				break;
				default:
				break;
			}

			rdrButtonContainer.GetToggleGrid().Checked = UserPreferencesManager.Instance.UserPreferences.DebugRenderShowGrid;
			rdrButtonContainer.GetToggleGrid().Text = rdrButtonContainer.GetToggleGrid().Checked ? "+ Grid" : "- Grid";

			DisableUIEvents = false;
		}

		private void toggleFSAA_CheckedChanged(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				CheckBox cb = sender as CheckBox;

				UserPreferencesManager.Instance.UserPreferences.DebugRenderFSAA = cb.Checked;
				UserPreferencesManager.Instance.Save();
			}
		}

		private void toggleAnnotations_CheckedChanged(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				CheckBox cb = sender as CheckBox;

				UserPreferencesManager.Instance.UserPreferences.DebugRenderAnnotations = cb.Checked;
				UserPreferencesManager.Instance.Save();
			}
		}

		private void toggleGrid_CheckedChanged(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				CheckBox cb = sender as CheckBox;

				UserPreferencesManager.Instance.UserPreferences.DebugRenderShowGrid = cb.Checked;
				UserPreferencesManager.Instance.Save();
			}
		}

		private void toggleDimensions_CheckedChanged(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				CheckBox cb = sender as CheckBox;

				UserPreferencesManager.Instance.UserPreferences.DebugRender3DView = cb.Checked;
				UserPreferencesManager.Instance.Save();
			}
		}

		private void fitToView_Click(object sender, EventArgs e)
		{
			debugRenderControl.FitToView();
		}

		private void cycleNormals_Click(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				switch (UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals)
				{
					case ShowNormalsMode.None:
						UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals = ShowNormalsMode.Lines;
					break;
					case ShowNormalsMode.Lines:
						UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals = ShowNormalsMode.Triangles;
					break;
					case ShowNormalsMode.Triangles:
						UserPreferencesManager.Instance.UserPreferences.DebugRenderShowNormals = ShowNormalsMode.None;
					break;
					default:
					break;
				}
				
				UserPreferencesManager.Instance.Save();
			}
		}

		private void clearColor_Click(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				ColorDialog dlg = new ColorDialog();
				dlg.FullOpen = true;
				dlg.Color = System.Drawing.Color.FromArgb(debugRenderControl.ClearColor.R, debugRenderControl.ClearColor.G, debugRenderControl.ClearColor.B);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					UserPreferencesManager.Instance.UserPreferences.DebugRenderClearColor = new Color32(dlg.Color.R, dlg.Color.G, dlg.Color.B, 255);
					UserPreferencesManager.Instance.Save();
				}
			}
		}

		#region IReView_RemoteDebugRenderer Implementation

		public void RemoveAllPrimitives(int time)
		{
			Session.RemoveAllPrimitives(time);
		}

		public void RemoveAllAnnotations(int time)
		{
			Session.RemoveAllAnnotations(time);
		}

		public void AddAnnotation(long primitive_id, int time, int duration, string text, Color32 color)
		{
			Session.AddAnnotation(primitive_id, time, duration, text, color);
		}

		public void AddBox(long id, int time, int duration, Matrix4x4 transform, Vector3 pivot, Vector3 half_size, Color32 color)
		{
			DebugRenderPrimitive box = new DebugRenderBox(transform, pivot, half_size, color);
			box.StartTime = time;
			box.Duration = duration;
			box.Id = id;
			Session.AddPrimitive(box);
			NotifyDurationChanged(time);
		}

		public void RemovePrimitive(long primitive_id, int time)
		{
			Session.RemovePrimitive(primitive_id, time);
			NotifyDurationChanged(time);
		}

		public void RemoveAnnotation(long primitive_id, int time)
		{
			Session.RemoveAnnotation(primitive_id, time);
			NotifyDurationChanged(time);
		}

		public void AddCylinder(long id, int time, int duration, Matrix4x4 transform, Vector3 pivot, double top_radius, double bottom_radius_scale, double height, int segments, Color32 color, bool create_caps)
		{
			DebugRenderPrimitive cylinder = new DebugRenderCylinder(transform, pivot, top_radius, bottom_radius_scale, height, segments, color, create_caps);
			cylinder.StartTime = time;
			cylinder.Duration = duration;
			cylinder.Id = id;
			Session.AddPrimitive(cylinder);
			NotifyDurationChanged(time);
		}

		public void AddCone(long id, int time, int duration, Matrix4x4 transform, Vector3 pivot, double radius, double height, int segments, Color32 color, bool create_caps)
		{
			DebugRenderPrimitive cone = new DebugRenderCone(transform, pivot, radius, height, segments, color, create_caps);
			cone.StartTime = time;
			cone.Duration = duration;
			cone.Id = id;
			Session.AddPrimitive(cone);
			NotifyDurationChanged(time);
		}

		public void AddMesh(long id, int time, int duration, Matrix4x4 transform, Vector3 pivot)
		{
			DebugRenderPrimitive mesh = new DebugRenderTriMesh(transform, pivot);
			mesh.StartTime = time;
			mesh.Duration = duration;
			mesh.Id = id;
			Session.AddPrimitive(mesh);
			NotifyDurationChanged(time);
		}

		public void AddTriangle(long mesh_id, int time, Vector3 a, Vector3 b, Vector3 c, Color32 color)
		{
			Session.AddTriangle(mesh_id, time, a, b, c, color);
			NotifyDurationChanged(time);
		}

		public void AddLine(long id, int time, int duration, Vector3 start, Vector3 end, Color32 color)
		{
			DebugRenderPrimitive mesh = new DebugRenderLine(start, end, color);
			mesh.StartTime = time;
			mesh.Duration = duration;
			mesh.Id = id;
			Session.AddPrimitive(mesh);
			NotifyDurationChanged(time);
		}

		public void AddCircle(long id, int time, int duration, Vector3 center, double radius, Vector3 up, int segments, Color32 color)
		{
			DebugRenderPrimitive circle = new DebugRenderCircle(center, radius, up, segments, color);
			circle.StartTime = time;
			circle.Duration = duration;
			circle.Id = id;
			Session.AddPrimitive(circle);
			NotifyDurationChanged(time);
		}

		#endregion

		public RDRSession Session
		{
			get
			{
				return debugRenderControl.Session as RDRSession;
			}
			set
			{
				if (debugRenderControl.Session != null)
				{
					(debugRenderControl.Session as RDRSession).SceneInfo.PropertyChanged -= SceneInfo_PropertyChanged;
					debugRenderControl.OnSelectionChanged -= OnSelectionChanged;
					debugRenderControl.OnHighlightChanged -= OnHighlightChanged;
				}
				debugRenderControl.Session = value;
				if (debugRenderControl.Session != null)
				{
					(debugRenderControl.Session as RDRSession).SceneInfo.PropertyChanged += SceneInfo_PropertyChanged;
					debugRenderControl.OnSelectionChanged += OnSelectionChanged;
					debugRenderControl.OnHighlightChanged += OnHighlightChanged;
				}
				propertyGrid.SelectedObject = (debugRenderControl.Session as RDRSession).SceneInfo;
				highlightPropertyGrid.SelectedObject = null;
			}
		}

		private void OnSelectionChanged(DebugRenderPrimitive selectedPrimitive, int selectedTriangleIndex, bool isSelectedTriangleOpaque)
		{
			if (selectedPrimitive != null)
			{
				propertyGrid.SelectedObject = selectedPrimitive.GetSelectionInfo(selectedTriangleIndex, isSelectedTriangleOpaque);
			}
			else
			{
				propertyGrid.SelectedObject = (debugRenderControl.Session as RDRSession).SceneInfo;
			}
		}

		private void OnHighlightChanged(DebugRenderPrimitive selectedPrimitive, int selectedTriangleIndex, bool isSelectedTriangleOpaque)
		{
			if (selectedPrimitive != null)
			{
				highlightPropertyGrid.SelectedObject = selectedPrimitive.GetSelectionInfo(selectedTriangleIndex, isSelectedTriangleOpaque);
			}
			else
			{
				highlightPropertyGrid.SelectedObject = (debugRenderControl.Session as RDRSession).SceneInfo;
			}
		}

		private void SceneInfo_PropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			propertyGrid.Refresh();
			highlightPropertyGrid.Refresh();
		}

		private RDRButtonContainer rdrButtonContainer;
	}
}
