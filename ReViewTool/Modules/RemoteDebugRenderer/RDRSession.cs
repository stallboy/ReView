using ReView;
using ReViewDebugRenderView;
using ReViewDebugRenderView.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewTool.Modules.RemoteDebugRenderer
{
	public class RDRSession : TimelineModel, DebugRenderSession
	{
		public RDRSession()
		{
			SceneInfo = new SceneInfo();
			DebugRenderStorage = new DebugRenderStorage();
			DebugRenderStorage.RenderCacheUpdated += OnRenderCacheUpdated;

			SuspendReViewDebugRenderSessionDataChangedNotifications = false;
			SuspendPlaybackPositionChangedNotifications = false;
			SuspendDurationChangedNotifications = false;
		}

		private void OnRenderCacheUpdated()
		{
			SceneInfo.OpaqueLines = DebugRenderStorage.CachedOpaqueLinesCount;
			SceneInfo.TransparentLines = DebugRenderStorage.CachedTransparentLinesCount;
			SceneInfo.OpaqueTriangles = DebugRenderStorage.CachedOpaqueTrianglesCount;
			SceneInfo.TransparentTriangles = DebugRenderStorage.CachedTransparentTrianglesCount;
		}

		public SceneInfo SceneInfo
		{
			get;
			set;
		}

		public int Duration
		{
			get { return duration; }
			set
			{
				duration = value;
				if (!SuspendDurationChangedNotifications)
				{
					if (OnDurationChanged != null)
						OnDurationChanged();
				}
			}
		}

		public int PlaybackPosition
		{
			get { return playbackPosition; }
			set
			{
				int clampedValue = value;
				if (clampedValue > duration)
					clampedValue = duration;
				if (clampedValue < 0)
					clampedValue = 0;

				if (playbackPosition != clampedValue)
				{
					playbackPosition = clampedValue;

					if (!SuspendPlaybackPositionChangedNotifications && OnPlaybackPositionChanged != null)
						OnPlaybackPositionChanged(playbackPosition);
				}
			}
		}

		public void Clear()
		{
			DebugRenderStorage = new DebugRenderStorage();

			if (!SuspendReViewDebugRenderSessionDataChangedNotifications && OnReViewDebugRenderSessionDataChanged != null)
				OnReViewDebugRenderSessionDataChanged();
		}

		public void UpdateDuration(int time)
		{
			if (time > duration)
			{
				Duration = time;
			}
		}

		public DebugRenderStorage DebugRenderStorage
		{
			get;
			set;
		}

		public bool SuspendReViewDebugRenderSessionDataChangedNotifications
		{
			get;
			set;
		}

		public bool SuspendPlaybackPositionChangedNotifications
		{
			get;
			set;
		}

		public bool SuspendDurationChangedNotifications
		{
			get;
			set;
		}

		public void RemoveAllPrimitives(int time)
		{
			DebugRenderStorage.ModificationAt(time);

			DebugRenderStorage.RemoveAllPrimitives(time);
		}

		public void RemoveAllAnnotations(int time)
		{
			DebugRenderStorage.ModificationAt(time);

			DebugRenderStorage.RemoveAllAnnotations(time);
		}

		public void AddAnnotation(long primitive_id, int time, int duration, string text, Color32 color)
		{
			DebugRenderPrimitive primitive = DebugRenderStorage.GetPrimitive(primitive_id);
			if (primitive != null)
			{
				primitive.Annotation = new DebugRenderAnnotation(text, color, time, duration);
			}
		}

		public void AddPrimitive(DebugRenderPrimitive primitive)
		{
			DebugRenderStorage.ModificationAt(primitive.StartTime);

			DebugRenderStorage.AddPrimitive(primitive);
			UpdateDuration(primitive.StartTime + (primitive.InfiniteLength ? 0 : primitive.Duration));
		}

		public void RemovePrimitive(long primitive_id, int time)
		{
			DebugRenderPrimitive primitive = DebugRenderStorage.GetPrimitive(primitive_id);
			if (primitive != null)
			{
				DebugRenderStorage.ModificationAt(time);
				primitive.EndTime = time;
				UpdateDuration(time);
			}
		}

		public void RemoveAnnotation(long primitive_id, int time)
		{
			DebugRenderPrimitive primitive = DebugRenderStorage.GetPrimitive(primitive_id);
			if (primitive != null && primitive.Annotation != null)
			{
				DebugRenderStorage.ModificationAt(time);
				primitive.Annotation.EndTime = time;
				UpdateDuration(time);
			}
		}

		public void AddTriangle(long mesh_id, int time, Vector3 a, Vector3 b, Vector3 c, Color32 color)
		{
			DebugRenderStorage.ModificationAt(time);

			DebugRenderTriMesh mesh = DebugRenderStorage.GetPrimitive(mesh_id) as DebugRenderTriMesh;
			if (mesh != null)
			{
				mesh.AddTriangle(time, a, b, c, color);
			}
			UpdateDuration(time);
		}

		public event ReViewDebugRenderSessionDataChanged OnReViewDebugRenderSessionDataChanged;

		public event PlaybackPositionChanged OnPlaybackPositionChanged;
		public event DurationChanged OnDurationChanged;

		protected int playbackPosition = 0;
		protected int duration = 0;
	}
}
