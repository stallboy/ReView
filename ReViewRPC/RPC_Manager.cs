using Lemniscate;
using ReView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReViewRPC
{
	/// <summary>
	/// This is RPC_Manager class that can either be used as a Singleton (using static Instance accessor) or you can instantiate multiple for multiple streams.
	/// </summary>
	public class RPC_Manager
	{
		public RPC_Manager()
		{
		}

		public static RPC_Manager Instance
		{
			get
			{
				// Double-checked locking
				if (sRPCManager == null)
				{
					lock (sInstanceLock)
					{
						if (sRPCManager == null)
						{
							sRPCManager = new RPC_Manager();
						}
					}
				}
				return sRPCManager;
			}
			private set { }
		}

		private Linear_Serializer Linear_Serializer
		{
			get;
			set;
		}

		private RPC_Server_Proxies_Multiplexer RPC_Multiplexer
		{
			get;
			set;
		}

		public bool IsConnected
		{
			get
			{
				return Linear_Serializer != null;
			}
		}

		public bool IsClosed
		{
			get
			{
				return server_proxies_by_type.Count == 0 && client_proxies_by_type.Count == 0 && receiver_thread == null && Linear_Serializer == null; 
			}
		}

		public void Bind(Stream stream)
		{
			if (!IsClosed)
			{
				throw new Exception("RPC_Manager is already running or has not been cleaned properly. Call Close() to cleanup before calling Bind().");
			}
			Linear_Serializer = new Linear_Serializer(stream, false, Linear_Serializer.Array_Length_Encoding.Fixed_Length_32bits);
			RPC_Multiplexer = new RPC_Server_Proxies_Multiplexer(Linear_Serializer);

			NotifyConnectionStateChanged(true);
		}

		public void Start_Receiver()
		{
			if (receiver_thread == null)
			{
				receiver_thread = new Thread(Receive_Calls);
				receiver_thread.IsBackground = true;
				receiver_thread.Start();
			}
		}

		public void Close()
		{
			if (Linear_Serializer != null)
			{
				Linear_Serializer.Close();
				Linear_Serializer = null;
			}

			if (receiver_thread != null && Thread.CurrentThread != receiver_thread)
			{
				receiver_running = false;
				receiver_thread.Join();
				receiver_thread = null;
			}

			if (RPC_Multiplexer != null)
			{
				RPC_Multiplexer = null;
			}

			server_proxies_by_type.Clear();
			client_proxies_by_type.Clear();

			NotifyConnectionStateChanged(false);
		}

		private void Receive_Calls()
		{
			receiver_running = true;

			try
			{
				while (receiver_running)
				{
					RPC_Multiplexer.Receive_Call();
				}
			}
			catch (Exception e)
			{
				Close();

				Log.WriteLine("RPC_Manager is shutting down due to exception in receiving calls.");
				Log.WriteException(e);
			}
		}

		public T1 Create_Server_Proxy<T1, T2>(T2 interface_implementation) where T1 : RPC_Server_Receiver
		{
			T1 proxy = (T1)Activator.CreateInstance(typeof(T1), new object[] { Linear_Serializer, interface_implementation });
			server_proxies_by_type.Add(typeof(T1), proxy);

			RPC_Multiplexer[proxy.Channel_ID] = proxy;

			return proxy;
		}

		public T Get_Server_Proxy<T>() where T : class
		{
			return server_proxies_by_type[typeof(T)] as T;
		}

		public T Create_Client_Proxy<T>() where T : RPC_Client_Proxy
		{
			T proxy = (T)Activator.CreateInstance(typeof(T), new object[] { Linear_Serializer });
			client_proxies_by_type.Add(typeof(T), proxy);

			proxy.Lock_Object = Linear_Serializer;

			return proxy;
		}

		public T Get_Client_Proxy<T>() where T : class
		{
			if (client_proxies_by_type.ContainsKey(typeof(T)))
			{
				return client_proxies_by_type[typeof(T)] as T;
			}
			return null;
		}

		private void NotifyConnectionStateChanged(bool connected)
		{
			DlgConnectionStateChanged handle = ConnectionStateChanged;
			if (handle != null)
			{
				handle(connected);
			}
		}

		public delegate void DlgConnectionStateChanged(bool connected);

		public event DlgConnectionStateChanged ConnectionStateChanged;

		private Dictionary<Type, object> server_proxies_by_type = new Dictionary<Type, object>();
		private Dictionary<Type, object> client_proxies_by_type = new Dictionary<Type, object>();

		private static object sInstanceLock = new Object();
		private static RPC_Manager sRPCManager = null;

		private Thread receiver_thread;
		private bool receiver_running = false;
	}
}
