// The MIT License (MIT)
// 
// Copyright (c) 2014 Maurizio de Pascale
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;



namespace Lemniscate
{
	public class Network_Stream_Server : IDisposable
	{
		#region Public

		//call without specifying a port to get one dynamically assigned, and use the Port property to find it out
		public Network_Stream_Server(Action<Stream, IPEndPoint> handler, ushort port = 0)
		{
			this.handler = handler;

			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

			var local_end_point = new IPEndPoint(IPAddress.Any, port);
			listener.Bind(local_end_point);
			
			listener.Listen(1024);

			listener.BeginAccept(On_Incoming_Connection, null);
		}

		public void Dispose()
		{
			listener.Dispose();
			
			//TODO: put this back and make On_Incoming_Connection resilient to errors
			//listener = null;
		}

		public ushort Port
		{
			get
			{
				return (ushort) ( (IPEndPoint) listener.LocalEndPoint ).Port;
			}
		}

		#endregion


		#region Internals
		
		void On_Incoming_Connection(IAsyncResult async_result)
		{
			try
			{
				//TODO: perhaps we should use a pair of using blocks for both the socket and the network_stream

				//until we start accepting again no one would execute this code again
				var socket = listener.EndAccept(async_result);
				socket.SendBufferSize = 1024 * 1024;
				socket.ReceiveBufferSize = 1024 * 1024;
				socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

				var network_stream = new NetworkStream(socket, ownsSocket : true);

				handler(network_stream, (IPEndPoint)socket.RemoteEndPoint);

				//start accepting connections again
				listener.BeginAccept(On_Incoming_Connection, null);
			}
			catch (ObjectDisposedException exception)
			{
				Code.Unreferenced_Params(exception);

				//nothing to be done here... we're just disposing the listener while it was already listening
			}
			catch (SocketException exception)
			{
				Code.Unreferenced_Params(exception);
				
				//TODO: figure out in what cases this happens

				//start accepting connections again
				listener.BeginAccept(On_Incoming_Connection, null);
			}
		}
		
		#endregion

		
		#region Fields
		
		Action<Stream, IPEndPoint> handler;
		Socket listener;
		
		#endregion
	}

	public static class Network
	{
		public static NetworkStream Create_Stream_To(IPEndPoint end_point)
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			socket.SendBufferSize = 1024 * 1024;
			socket.ReceiveBufferSize = 1024 * 1024;

			socket.Connect(end_point);

			return new NetworkStream(socket, ownsSocket : true);
		}

		public static IPEndPoint Address_From_Host_And_Port(string hostname, ushort port)
		{
			var addressList = Dns.GetHostEntry(hostname).AddressList;
			var address = Array.FindLast(addressList, entry => entry.AddressFamily == AddressFamily.InterNetwork);
			
			return	address != null
				?	new IPEndPoint(address, port)
				:	null;
		}

		public static bool Is_TCP_Port_Available(ushort port)
		{
			var ip_global_properties = IPGlobalProperties.GetIPGlobalProperties();
			var tcp_connections = ip_global_properties.GetActiveTcpConnections();

			foreach (var connection in tcp_connections)
			{
				if (connection.LocalEndPoint.Port == port) return false;
			}

			return true;
		}
	}
}