using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ReViewDebugRenderView
{
	public class Selection
	{
		public Selection(DebugRenderPrimitive primitive, int selectedTriangleIndex, bool selectedTriangleIsOpaque)
		{
			Primitive = primitive;
			SelectedTriangleIndex = selectedTriangleIndex;
			SelectedTriangleIsOpaque = selectedTriangleIsOpaque;
		}

		public DebugRenderPrimitive Primitive
		{
			get;
			set;
		}

		public int SelectedTriangleIndex
		{
			get;
			set;
		}

		public bool SelectedTriangleIsOpaque
		{
			get;
			set;
		}

		// Check equality
		public static bool operator ==(Selection a, Selection b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			return a.SelectedTriangleIndex == b.SelectedTriangleIndex && a.SelectedTriangleIsOpaque == b.SelectedTriangleIsOpaque && a.Primitive == b.Primitive;
		}

		// Check inequality
		public static bool operator !=(Selection a, Selection b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			Selection other = obj as Selection;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class RenderInfo
	{
		public int time;
		public bool showLineNormals;
		public bool showTriangleNormals;
		public bool showAnnotations;
		public bool showGrid;
		public Color32 normalColor;
		public double normalLength;
		public Selection Selection;
		public Selection Highlight;
	}

	public delegate void ReViewDebugRenderSessionDataChanged();
 
	public interface DebugRenderSession
	{
		int Duration { get; set; }
		int PlaybackPosition { get; set; }
		bool SuspendReViewDebugRenderSessionDataChangedNotifications { get; set; }
		DebugRenderStorage DebugRenderStorage { get; set; }

		event ReViewDebugRenderSessionDataChanged OnReViewDebugRenderSessionDataChanged;
	}

	public class MappedMemory : IDisposable
	{
		public void Dispose()
		{
			if (handle.IsAllocated)
			{
				handle.Free();
			}
		}

		public GCHandle Handle
		{
			get
			{
				return handle;
			}
			set
			{
				if (handle.IsAllocated)
				{
					handle.Free();
				}
				handle = value;
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}

		private GCHandle handle;
		private int count;
	}

	/// <summary>
	/// Contains data for all debug render primitives
	/// Can perform O(log n) searches by id, startTime (for infinite length primitives)
	/// For finite length primitives there is a "span list" for finding a list of primitives that are potentially visible (conservative so false positives but not false negatives)
	/// </summary>
	public class DebugRenderStorage
	{
		public DebugRenderStorage()
		{
			cachedPrimitivesForRenderer = new List<DebugRenderPrimitive>();

			renderCacheDirty = true;
			cacheDirty = true;
			DisablePrimitiveEndTimeSort = false;
		}

		public void ModificationAt(int time)
		{
			lock (usedTimeEntries)
			{
				int index = usedTimeEntries.BinarySearch(time);
				if (index < 0)
				{
					index = ~index;

					// Entry not found -> Add it to appropriate place
					usedTimeEntries.Insert(index, time);
				}
			}
		}

		private void InsertPrimitive(LinkedList<DebugRenderPrimitive> primitiveList, DebugRenderPrimitive primitive)
		{
			lock (primitiveList)
			{
				// Find node where to insert (EndTime descending)
				if (primitive.InfiniteLength)
				{
					primitiveList.AddFirst(primitive);
				}
				else
				{
					LinkedListNode<DebugRenderPrimitive> iter = primitiveList.First;
					while (iter != null && (iter.Value.InfiniteLength || primitive.EndTime < iter.Value.EndTime))
					{
						iter = iter.Next;
					}

					if (iter != null)
					{
						primitiveList.AddBefore(iter, primitive);
					}
					else
					{
						primitiveList.AddLast(primitive);
					}
				}
			}
		}

		// Add primitive into 'primitiveTimeMap' and 'primitiveIdMap' search structures
		// Uses locking for the data structures, each data structure is locked separately to reduce lock time
		public void AddPrimitive(DebugRenderPrimitive primitive)
		{
			LinkedList<DebugRenderPrimitive> primitiveList = null;

			lock (primitiveTimeMap)
			{
				// Find primitive list for the given StartTime (if not found add it)
				if (!primitiveTimeMap.TryGetValue(primitive.StartTime, out primitiveList))
				{
					primitiveList = new LinkedList<DebugRenderPrimitive>();
					primitiveTimeMap.Add(primitive.StartTime, primitiveList);
				}
			}

			InsertPrimitive(primitiveList, primitive);

			primitive.OnPrimitiveEndTimeChanged += OnPrimitiveEndTimeChanged;

			lock (primitiveIdMap)
			{
				// Add to ID map
				primitiveIdMap.Add(primitive.Id, primitive);
			}

			cacheDirty = true;
			renderCacheDirty = true;
		}

		private bool DisablePrimitiveEndTimeSort
		{
			get;
			set;
		}

		private void OnPrimitiveEndTimeChanged(DebugRenderPrimitive primitive)
		{
			if (DisablePrimitiveEndTimeSort)
			{
				return;
			}

			LinkedList<DebugRenderPrimitive> primitiveList = null;

			lock (primitiveTimeMap)
			{
				// Find primitive list for the given StartTime (if not found add it)
				if (!primitiveTimeMap.TryGetValue(primitive.StartTime, out primitiveList))
				{
					// Primitive not in any list -> Bail out
					return;
				}
			}

			lock (primitiveList)
			{
				// Remove primitive from list
				primitiveList.Remove(primitive);

				// Re-add to new position
				InsertPrimitive(primitiveList, primitive);
			}
		}

		public void Invalidate()
		{
			cacheDirty = true;
			renderCacheDirty = true;
		}

		public List<DebugRenderPrimitive> GetCachedPrimitives()
		{
			return cachedPrimitivesForRenderer;
		}

		public List<DebugRenderPrimitive> GetPrimitivesForTime(int currentTime)	
		{
			if (currentTime != lastRequestedTime)
			{
				lastRequestedTime = currentTime;
				cacheDirty = true;
				renderCacheDirty = true;
			}

			if (cacheDirty)
			{
				cachedPrimitivesForRenderer = GetPrimitives(currentTime);
				cacheDirty = false;

				NotifyCacheUpdated();
			}

			return cachedPrimitivesForRenderer;
		}

		private void NotifyCacheUpdated()
		{
			DlgCacheUpdated handler = CacheUpdated;
			if (handler != null)
			{
				handler();
			}
		}

		private void NotifyRenderCacheUpdated()
		{
			DlgRenderCacheUpdated handler = RenderCacheUpdated;
			if (handler != null)
			{
				handler();
			}
		}

		private List<DebugRenderPrimitive> GetPrimitives(int currentTime)
		{
			List<DebugRenderPrimitive> results = new List<DebugRenderPrimitive>(100);

			List<LinkedList<DebugRenderPrimitive>> primitiveLists = new List<LinkedList<DebugRenderPrimitive>>();
			// Get list of primitiveLists -> Lock primitiveTimeMap only for fetching the information
			lock (primitiveTimeMap)
			{
				foreach (KeyValuePair<int, LinkedList<DebugRenderPrimitive>> keyValue in primitiveTimeMap.Where(kv => kv.Key <= currentTime))
				{
					primitiveLists.Add(keyValue.Value);
				}
			}

			// Iterate through primitiveLists
			foreach (LinkedList<DebugRenderPrimitive> primitiveList in primitiveLists)
			{
				// Lock each list separately to minimize lock time
				lock (primitiveList)
				{
					LinkedListNode<DebugRenderPrimitive> iter = primitiveList.First;

					while (iter != null)
					{
						DebugRenderPrimitive primitive = iter.Value;
						if (primitive.InfiniteLength || primitive.EndTime > currentTime)
						{
							// Valid primitive
							results.Add(primitive);
						}
						else
						{
							// Found first primitive where EndTime has passed -> No more valid primitives in this list
							break;
						}

						iter = iter.Next;
					}
				}
			}

			return results;
		}

		public void RemoveAllPrimitives(int time)
		{
			lock (primitiveTimeMap)
			{
				foreach (LinkedList<DebugRenderPrimitive> primitiveList in primitiveTimeMap.Values)
				{
					DisablePrimitiveEndTimeSort = true;
					foreach (DebugRenderPrimitive primitive in primitiveList)
					{
						if (primitive.EndTime > time || primitive.InfiniteLength)
						{
							primitive.EndTime = time;
						}
					}
					DisablePrimitiveEndTimeSort = false;
				}
			}
		}

		public void RemoveAllAnnotations(int time)
		{
			lock (primitiveTimeMap)
			{
				foreach (LinkedList<DebugRenderPrimitive> primitiveList in primitiveTimeMap.Values)
				{
					foreach (DebugRenderPrimitive primitive in primitiveList)
					{
						if (primitive.Annotation != null && (primitive.Annotation.EndTime > time || primitive.Annotation.InfiniteLength))
						{
							primitive.Annotation.EndTime = time;
						}
					}
				}
			}
		}

		public DebugRenderPrimitive GetPrimitive(long primitive_id)
		{
			DebugRenderPrimitive result = null;
			primitiveIdMap.TryGetValue(primitive_id, out result);
			return result;
		}

		public int GetNextTimelineEventTime(int time)
		{
			lock (usedTimeEntries)
			{
				int index = usedTimeEntries.BinarySearch(time);
				if (index < 0)
				{
					index = ~index;
				}

				if (index >= 0 && index < usedTimeEntries.Count - 1)
				{
					return usedTimeEntries[index + 1];
				}
			}

			return -1;
		}

		public int GetPrevTimelineEventTime(int time)
		{
			lock (usedTimeEntries)
			{
				int index = usedTimeEntries.BinarySearch(time);
				if (index < 0)
				{
					index = ~index;
				}

				if (index > 0 && index < usedTimeEntries.Count)
				{
					return usedTimeEntries[index - 1];
				}
			}

			return -1;
		}

		public Bounds GetBounds(int time)
		{
			Bounds bounds = new Bounds();
			List<DebugRenderPrimitive> primitivesToRender = GetPrimitivesForTime(time);
			foreach (DebugRenderPrimitive primitive in primitivesToRender)
			{
				if (primitive.WorldBounds.IsValid)
				{
					bounds.Encapsulate(primitive.WorldBounds);
				}
			}
			return bounds;
		}

		protected void RenderArrays(MappedMemory vertexData, MappedMemory indexData, bool triangles)
		{
			OpenGL gl = DebugRenderControl.gl;

			IntPtr vertexPtr = vertexData.Handle.AddrOfPinnedObject();

			gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
			if (triangles)
			{
				gl.EnableClientState(OpenGL.GL_NORMAL_ARRAY);
			} 
			gl.EnableClientState(OpenGL.GL_COLOR_ARRAY);

			gl.VertexPointer(3, OpenGL.GL_FLOAT, Marshal.SizeOf(typeof(VertexNormalColor)), vertexPtr);
			if (triangles)
			{
				gl.NormalPointer(OpenGL.GL_FLOAT, Marshal.SizeOf(typeof(VertexNormalColor)), IntPtr.Add(vertexPtr, 12));
			}
			gl.ColorPointer(4, OpenGL.GL_FLOAT, Marshal.SizeOf(typeof(VertexNormalColor)), IntPtr.Add(vertexPtr, 24));

			if (indexData != null)
			{
				IntPtr indexPtr = indexData.Handle.AddrOfPinnedObject();
				gl.DrawElements(triangles ? OpenGL.GL_TRIANGLES : OpenGL.GL_LINES, indexData.Count / 3, OpenGL.GL_UNSIGNED_INT, indexPtr);
			}
			else
			{
				gl.DrawArrays(triangles ? OpenGL.GL_TRIANGLES : OpenGL.GL_LINES, 0, vertexData.Count);
			}

			gl.DisableClientState(OpenGL.GL_COLOR_ARRAY);
			if (triangles)
			{
				gl.DisableClientState(OpenGL.GL_NORMAL_ARRAY);
			}
			gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
		}

		public void Render(RenderInfo info)
		{
			// Collect all triangles and lines into two separate arrays
			List<DebugRenderPrimitive> primitivesToRender = GetPrimitivesForTime(info.time);

			if (renderCacheDirty)
			{
				ArrayList<VertexNormalColor> opaqueIndexedTriangleVertexData = new ArrayList<VertexNormalColor>();
				ArrayList<uint> opaqueIndexedTriangleIndexData = new ArrayList<uint>();
				ArrayList<VertexNormalColor> opaqueTriangleVertexData = new ArrayList<VertexNormalColor>();
				ArrayList<VertexNormalColor> opaqueLineVertexData = new ArrayList<VertexNormalColor>();

				ArrayList<VertexNormalColor> transparentIndexedTriangleVertexData = new ArrayList<VertexNormalColor>();
				ArrayList<uint> transparentIndexedTriangleIndexData = new ArrayList<uint>();
				ArrayList<VertexNormalColor> transparentTriangleVertexData = new ArrayList<VertexNormalColor>();
				ArrayList<VertexNormalColor> transParentLineVertexData = new ArrayList<VertexNormalColor>();

				foreach (DebugRenderPrimitive primitive in primitivesToRender)
				{
					primitive.AddTriangles(info, opaqueIndexedTriangleVertexData, transparentIndexedTriangleVertexData, opaqueIndexedTriangleIndexData, transparentIndexedTriangleIndexData);
					primitive.AddTriangles(info, opaqueTriangleVertexData, transparentTriangleVertexData);
					primitive.AddLines(info, opaqueLineVertexData, transParentLineVertexData);
				}

				opaqueIndexedTriangleVertex.Handle = GCHandle.Alloc(opaqueIndexedTriangleVertexData.BackingArray, GCHandleType.Pinned);
				opaqueIndexedTriangleVertex.Count = opaqueIndexedTriangleVertexData.Count;
				transparentIndexedTriangleVertex.Handle = GCHandle.Alloc(transparentIndexedTriangleVertexData.BackingArray, GCHandleType.Pinned);
				transparentIndexedTriangleVertex.Count = transparentIndexedTriangleVertexData.Count;

				opaqueIndexedTriangleIndex.Handle = GCHandle.Alloc(opaqueIndexedTriangleIndexData.BackingArray, GCHandleType.Pinned);
				opaqueIndexedTriangleIndex.Count = opaqueIndexedTriangleIndexData.Count;
				transparentIndexedTriangleIndex.Handle = GCHandle.Alloc(transparentIndexedTriangleIndexData.BackingArray, GCHandleType.Pinned);
				transparentIndexedTriangleIndex.Count = transparentIndexedTriangleIndexData.Count;

				opaqueTriangleVertex.Handle = GCHandle.Alloc(opaqueTriangleVertexData.BackingArray, GCHandleType.Pinned);
				opaqueTriangleVertex.Count = opaqueTriangleVertexData.Count;
				transparentTriangleVertex.Handle = GCHandle.Alloc(transparentTriangleVertexData.BackingArray, GCHandleType.Pinned);
				transparentTriangleVertex.Count = transparentTriangleVertexData.Count;

				opaqueLineVertex.Handle = GCHandle.Alloc(opaqueLineVertexData.BackingArray, GCHandleType.Pinned);
				opaqueLineVertex.Count = opaqueLineVertexData.Count;
				transparentLineVertex.Handle = GCHandle.Alloc(transParentLineVertexData.BackingArray, GCHandleType.Pinned);
				transparentLineVertex.Count = transParentLineVertexData.Count;

				renderCacheDirty = false;

				NotifyRenderCacheUpdated();
			}

			DebugRenderControl.gl.Enable(OpenGL.GL_COLOR_MATERIAL);
			DebugRenderControl.gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);

			// Render opaque materials
			DebugRenderControl.gl.Disable(OpenGL.GL_BLEND);

			// Render triangle array(s) - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(opaqueIndexedTriangleVertex, opaqueIndexedTriangleIndex, true);

			// Render triangle array(s) - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(opaqueTriangleVertex, null, true);

			// Render line array - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(opaqueLineVertex, null, false);

			// Render transparent materials
			DebugRenderControl.gl.Enable(OpenGL.GL_BLEND);
			DebugRenderControl.gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			// Render triangle array(s) - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(transparentIndexedTriangleVertex, transparentIndexedTriangleIndex, true);

			// Render triangle array(s) - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(transparentTriangleVertex, null, true);

			// Render line array - Not using VBOs but should not cause major performance issue (VBOs don't help if data changes every frame)
			RenderArrays(transparentLineVertex, null, false);
		}

		public void RenderScreenSpace(RenderInfo info, Matrix4x4 transformMatrix, Viewport viewport)
		{
			if (info.showAnnotations)
			{
				Font font = new Font("Calibri", 12.0f * 0.75f);
				List<DebugRenderPrimitive> primitivesToRender = GetCachedPrimitives();
				foreach (DebugRenderPrimitive primitive in primitivesToRender)
				{
					if (primitive.Annotation != null && primitive.Annotation.IsValidAt(info.time))
					{
						Vector3 screenPosition = GeometryMath.Project(primitive.WorldBounds.Center, transformMatrix, viewport);
						if (screenPosition.z >= 0.0)
						{
							Size textSize = TextRenderer.MeasureText(primitive.Annotation.Text, font);
							Vector3 textHalfSize = new Vector3((double)textSize.Width, (double)textSize.Height, 0.0) * 0.5;

							OpenGLDrawHelper.FillRectangle(DebugRenderControl.gl, screenPosition - textHalfSize, screenPosition + textHalfSize, primitive.Annotation.Color.Brightness > 0.5 ? new Color32(0, 0, 0, 255) : new Color32(255, 255, 255, 255), false);

							DebugRenderControl.gl.DrawText((int)(screenPosition.x - textHalfSize.x) + 2, (int)(screenPosition.y - textHalfSize.y * 0.4),
															primitive.Annotation.Color.R, primitive.Annotation.Color.G, primitive.Annotation.Color.B,
															"Calibri", 12.0f, primitive.Annotation.Text);
						}
					}
				}
			}
		}

		public int CachedOpaqueLinesCount
		{
			get
			{
				return opaqueLineVertex != null ? opaqueLineVertex.Count / 2 : 0;
			}
		}

		public int CachedTransparentLinesCount
		{
			get
			{
				return transparentLineVertex != null ? transparentLineVertex.Count / 2 : 0;
			}
		}

		public int CachedOpaqueTrianglesCount
		{
			get
			{
				int countA = opaqueIndexedTriangleIndex != null ? opaqueIndexedTriangleIndex.Count / 3 : 0;
				int countB = opaqueTriangleVertex != null ? opaqueTriangleVertex.Count / 3 : 0;
				return countA + countB;
			}
		}

		public int CachedTransparentTrianglesCount
		{
			get
			{
				int countA = transparentIndexedTriangleIndex != null ? transparentIndexedTriangleIndex.Count / 3 : 0;
				int countB = transparentTriangleVertex != null ? transparentTriangleVertex.Count / 3 : 0;
				return countA + countB;
			}
		}

		public delegate void DlgCacheUpdated();

		public event DlgCacheUpdated CacheUpdated;

		public delegate void DlgRenderCacheUpdated();

		public event DlgRenderCacheUpdated RenderCacheUpdated;

		private List<int> usedTimeEntries = new List<int>();
		private SortedDictionary<int, LinkedList<DebugRenderPrimitive>> primitiveTimeMap = new SortedDictionary<int, LinkedList<DebugRenderPrimitive>>();
		private Dictionary<long, DebugRenderPrimitive> primitiveIdMap = new Dictionary<long, DebugRenderPrimitive>();

		// Cached primitives for renderer
		private MappedMemory opaqueIndexedTriangleVertex = new MappedMemory();
		private MappedMemory transparentIndexedTriangleVertex = new MappedMemory();

		private MappedMemory opaqueIndexedTriangleIndex = new MappedMemory();
		private MappedMemory transparentIndexedTriangleIndex = new MappedMemory();

		private MappedMemory opaqueTriangleVertex = new MappedMemory();
		private MappedMemory transparentTriangleVertex = new MappedMemory();

		private MappedMemory opaqueLineVertex = new MappedMemory();
		private MappedMemory transparentLineVertex = new MappedMemory();

		private bool cacheDirty;
		private bool renderCacheDirty;
		private int lastRequestedTime;
		private List<DebugRenderPrimitive> cachedPrimitivesForRenderer;
	}
}
