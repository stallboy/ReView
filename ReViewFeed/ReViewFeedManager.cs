using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Lemniscate;
using ReViewRPC;
using System.Diagnostics;

namespace ReView
{
	public sealed class ReViewFeedManager : IReView_Feed
	{
		private ReViewFeedManager()
		{
			DebugTimerStep = 100;
			ResetDebugTimer();
		}

		/// <summary>
		/// Return singleton instance
		/// </summary>
		public static ReViewFeedManager Instance
		{
			get
			{
				// Check if instance is null, if not then we can simply return the existing instance without having to use a lock
				if (instance == null)
				{
					lock (instanceLock)
					{
						// Check instance against null again since the first check is not thread-safe, this double checking is to prevent use of lock every time
						if (instance == null)
						{
							instance = new ReViewFeedManager();
						}
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Returns true if currently connected to a storage server, false if not.
		/// </summary>
		public bool IsConnected
		{
			get
			{
				lock (operationLock)
				{
					IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
					return (RPCToolProxy != null);
				}
			}
		}

		/// <summary>
		/// Send selection changed
		/// </summary>
		public void SelectionChanged(long selectedId)
		{
			IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
			if (RPCHTLProxy != null)
			{
				RPCHTLProxy.SelectionChanged(selectedId);
			}
		}

		/// <summary>
		/// Send debug toggle changed
		/// </summary>
		public void DebugToggleChanged(string name, bool state)
		{
			IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
			if (RPCToolProxy != null)
			{
				RPCToolProxy.DebugToggleChanged(name, state);
			}
		}

		/// <summary>
		/// Connect to storage server
		/// </summary>
		/// <param name="address">Address to connect to (IP or name, will try to resolve)</param>
		/// <param name="port">Port to conenct to</param>
		/// <returns>true if succeeded, false if not. Also returns false if already connected (check with IsConnected).</returns>
		public bool Connect(string ipAddress, int port)
		{
			lock (operationLock)
			{
				IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
				if (RPCToolProxy != null)
				{
					return false;
				}

				IPAddress resolvedAddress = null;
				if (!IPAddress.TryParse(ipAddress, out resolvedAddress))
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);

					if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
					{
						foreach (IPAddress address in hostEntry.AddressList)
						{
							if (address.AddressFamily == AddressFamily.InterNetwork)
							{
								resolvedAddress = address;
								break;
							}
						}
					}
				}

				IPEndPoint endpoint = new IPEndPoint(resolvedAddress, port);

				NetworkStream stream = Network.Create_Stream_To(endpoint);

				RPC_Manager.Instance.Bind(stream);

				RPC_Manager.Instance.Create_Server_Proxy<RPC_Server_Proxy_IReView_Feed, IReView_Feed>(this);
				RPC_Manager.Instance.Create_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
				RPC_Manager.Instance.Create_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
				RPC_Manager.Instance.Create_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();

				RPC_Manager.Instance.Start_Receiver();

				return true;
			}
		}

		private void CloseRPCProxies()
		{
			RPC_Manager.Instance.Close();
		}

		/// <summary>
		/// Disconnect from storage server
		/// </summary>
		public void Disconnect()
		{
			lock (operationLock)
			{
				CloseRPCProxies();
			}
		}

		/// <summary>
		/// Add Track for given parent with specified name.
		/// This method uses a lock to be thread-safe.
		/// </summary>
		/// <param name="parentId">Unique id, or -1 for no parent.</param>
		/// <param name="name">Name for the track, does not need to be unique.</param>
		/// <returns>Unique debug identifier for the new track.</returns>
		public long AddTrack(long parentId, String name)
		{
			try
			{
				lock (operationLock)
				{
					IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
					if (RPCHTLProxy != null)
					{
						long id = GetUniqueID();

						RPCHTLProxy.AddTrack(parentId, id, name);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		/// <summary>
		/// Add item for given track.
		/// </summary>
		/// <param name="parentId">Parent track id, must be positive number.</param>
		/// <param name="time">Time for this item to start.</param>
		/// <param name="name">Name for this item, does not need to be unique.</param>
		/// <returns>Positive unique identifier if item was successfully added and -1 if it could not be added.</returns>
		public long AddItem(long parentId, int time, String name)
		{
			if (parentId < 0)
			{
				return -1;
			}

			try
			{
				lock (operationLock)
				{
					IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
					if (RPCHTLProxy != null)
					{
						long id = GetUniqueID();
						RPCHTLProxy.AddItem(parentId, id, time, name);
						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
			return -1;
		}

		/// <summary>
		/// Add item for given track. It will automatically create a subtrack if none exists or if no free subtrack exists.
		/// </summary>
		/// <param name="parentId">Parent track id, must be positive number.</param>
		/// <param name="time">Time for this item to start.</param>
		/// <param name="name">Name for this item, does not need to be unique.</param>
		/// <returns>Positive unique identifier if item was successfully added and -1 if it could not be added.</returns>
		public long AddGenericItem(long parentId, int time, String name)
		{
			if (parentId < 0)
			{
				return -1;
			}

			try
			{
				lock (operationLock)
				{
					IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
					if (RPCHTLProxy != null)
					{
						long id = GetUniqueID();
						RPCHTLProxy.AddGenericItem(parentId, id, time, name);
						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
			return -1;
		}

		/// <summary>
		/// End existing open item.
		/// </summary>
		/// <param name="id">Positive id for the item / generic item.</param>
		/// <param name="time">Time when to end this item.</param>
		public void EndItem(long id, int time)
		{
			if (id < 0)
			{
				return;
			}

			try
			{
				lock (operationLock)
				{
					IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
					if (RPCHTLProxy != null)
					{
						RPCHTLProxy.EndItem(id, time);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		/// <summary>
		/// Append log for item specified by positive id.
		/// </summary>
		/// <param name="id">Positive id for the item / generic item.</param>
		/// <param name="time">Time to add the log output to.</param>
		/// <param name="type">Flags for the log output.</param>
		/// <param name="content">Log output content.</param>
		public void AppendLog(long id, int time, uint flags, string content)
		{
			if (id < 0)
			{
				return;
			}

			try
			{
				lock (operationLock)
				{
					IReView_HierarchicalTimelineLog RPCHTLProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_HierarchicalTimelineLog>();
					if (RPCHTLProxy != null)
					{
						RPCHTLProxy.AppendLog(id, time, flags, content);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		/// <summary>
		/// Store arbitrary binary data to the storage server.
		/// </summary>
		/// <param name="id">Positive id for the binary feed</param>
		/// <param name="time">Time for the data</param>
		/// <param name="data">Data itself</param>
		public void StoreBinaryData(long id, int time, ref byte[] data)
		{
			if (id < 0)
			{
				return;
			}

			try
			{
				lock (operationLock)
				{
					IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
					if (RPCToolProxy != null)
					{
						RPCToolProxy.StoreBinaryData(id, time, data);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		/// <summary>
		/// Add binary feed
		/// </summary>
		public ReViewFeedBinaryData RegisterBinaryDataFeed()
		{
			try
			{
				lock (operationLock)
				{
					IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
					if (RPCToolProxy != null)
					{
						ReViewFeedBinaryData newDataFeed = new ReViewFeedBinaryData();
						long id = GetUniqueID();
						newDataFeed.DebugID = id;
						lock (binaryDataFeedsLock)
						{
							binaryDataFeedMap.Add(newDataFeed.DebugID, newDataFeed);
						}
						return newDataFeed;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
			return null;
		}

		/// <summary>
		/// Called when receiving binary data from storage server
		/// </summary>
		public void SendBackBinaryData(long[] id, int[] time, byte[][] data)
		{
			if (id != null && time != null && data != null)
			{
				for (int i = 0; i < id.Length; i++)
				{
					if (binaryDataFeedMap.ContainsKey(id[i]))
					{
						binaryDataFeedMap[id[i]].NotifyDataReceived(time[i], ref data[i]);
					}
				}
			}
		}

		/// <summary>
		/// Remove binary feed
		/// </summary>
		public void UnregisterBinaryDataFeed(ReViewFeedBinaryData dataFeed)
		{
			lock (binaryDataFeedsLock)
			{
				binaryDataFeedMap.Remove(dataFeed.DebugID);
			}
		}

		public void AddAnnotation(long primitive_id, int time, int duration, string text, Color32 color)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.AddAnnotation(primitive_id, time, duration, text, color);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		public void RemoveAllPrimitives(int time)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.RemoveAllPrimitives(time);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		public void RemoveAllAnnotations(int time)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.RemoveAllAnnotations(time);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		public long AddBox(int time, int duration, Matrix4x4 transform, Vector3 pivot, Vector3 half_size, Color32 color)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddBox(id, time, duration, transform, pivot, half_size, color);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public void RemovePrimitive(long primitive_id, int time)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.RemovePrimitive(primitive_id, time);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		public void RemoveAnnotation(long primitive_id, int time)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.RemoveAnnotation(primitive_id, time);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}

		public long AddCylinder(int time, int duration, Matrix4x4 transform, Vector3 pivot, double top_radius, double bottom_radius_scale, double height, int segments, Color32 color, bool create_caps)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddCylinder(id, time, duration, transform, pivot, top_radius, bottom_radius_scale, height, segments, color, create_caps);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public long AddCone(int time, int duration, Matrix4x4 transform, Vector3 pivot, double radius, double height, int segments, Color32 color, bool create_caps)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddCone(id, time, duration, transform, pivot, radius, height, segments, color, create_caps);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public long AddMesh(int time, int duration, Matrix4x4 transform, Vector3 pivot, bool flatShaded)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddMesh(id, time, duration, transform, pivot);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public long AddTriangle(long mesh_id, int time, Vector3 a, Vector3 b, Vector3 c, Color32 color)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						RPCRDRProxy.AddTriangle(mesh_id, time, a, b, c, color);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public long AddLine(int time, int duration, Vector3 a, Vector3 b, Color32 color)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddLine(id, time, duration, a, b, color);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public long AddCircle(int time, int duration, Vector3 center, double radius, Vector3 up, int segments, Color32 color)
		{
			try
			{
				lock (operationLock)
				{
					IReView_RemoteDebugRenderer RPCRDRProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_RemoteDebugRenderer>();
					if (RPCRDRProxy != null)
					{
						long id = GetUniqueID();

						RPCRDRProxy.AddCircle(id, time, duration, center, radius, up, segments, color);

						return id;
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}

			return -1;
		}

		public bool Update(int currentTimeMillis, int inDeltaMillis)
		{
			IReView_Tool RPCToolProxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Tool>();
			if (RPCToolProxy == null)
			{
				return false;
			}

			keepAliveTimer += inDeltaMillis;
			if (keepAliveTimer >= keepAliveDelay)
			{
				keepAliveTimer = 0; // Reset instead of decrementing by 'keepAliveDelay' not to choke either end with messages
				
				// Send heartbeat if connection is still alive
				RPCToolProxy.Heartbeat(currentTimeMillis);
			}

			return true;
		}

		public void MapID(object obj, long id)
		{
			if (objectToIDMap.ContainsKey(obj))
			{
				objectToIDMap[obj] = id;
			}
			else
			{
				objectToIDMap.Add(obj, id);
			}
		}

		public long FindID(object obj)
		{
			return objectToIDMap.ContainsKey(obj) ? objectToIDMap[obj] : -1;
		}

		/// <summary>
		/// Get new unique id. Thread-safe by using a lock.
		/// </summary>
		/// <returns>New unique id.</returns>
		public static long GetUniqueID()
		{
			lock (autoIncrementLock)
			{
				return autoIncrement++;
			}
		}

		public int DebugTimer
		{
			get;
			set;
		}

		public int DebugTimerStep
		{
			get;
			set;
		}

		public void AdvanceDebugTimer()
		{
			DebugTimer += DebugTimerStep;
		}

		public void ResetDebugTimer()
		{
			DebugTimer = 0;
		}

		private int keepAliveTimer = 0; // Count time until reaching "delay"
		private int keepAliveDelay = 100; // 100ms delay between heartbeats

		private Dictionary<object, long> objectToIDMap = new Dictionary<object, long>();

		private object binaryDataFeedsLock = new object();
		private Dictionary<long, ReViewFeedBinaryData> binaryDataFeedMap = new Dictionary<long, ReViewFeedBinaryData>();

		private static volatile ReViewFeedManager instance = null;
		private static object instanceLock = new object();

		private static volatile int autoIncrement = 0;
		private static object autoIncrementLock = new object();

		private static object operationLock = new object();
	}
}
