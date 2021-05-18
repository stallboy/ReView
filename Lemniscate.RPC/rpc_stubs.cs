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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Lemniscate
{
	//plumbing classes to support generated RPC proxies
	public abstract class RPC_Client_Proxy
	{
		protected RPC_Client_Proxy(Linear_Serializer serializer)
		{
			this.serializer = serializer;
		}

		//to be used when multiplexing on a single stream
		public byte Channel_ID
		{
			set
			{
				prefix = new byte[1]{value};
			}
		}
		
		public byte[] Prefix
		{
			set
			{
				prefix = value;
			}
		}
		
		public object Lock_Object
		{
			set
			{
				lock_object = value;
			}
		}

		protected void Begin()
		{
			if (lock_object != null)
			{
				Monitor.Enter(lock_object);
			}

			//the prefix is to be treated as a byte sequence of fixed length, not as a varying length byte array
			foreach(var byte_value in prefix)
			{
				serializer.Serialize(byte_value);
			}
		}
		
		//called when transitioning from sending to receiving, shouldn't be conditional as it contains the required flush
		protected void Flip()
		{
			serializer.Flush();
		}

		protected void End()
		{
			if (lock_object != null)
			{
				Monitor.Exit(lock_object);
			}
		}

		protected object lock_object;
		protected byte[] prefix = new byte[0];
		protected readonly Linear_Serializer serializer;
	}

	public abstract class RPC_Server_Receiver
	{
		protected RPC_Server_Receiver(Linear_Serializer serializer)
		{
			this.serializer = serializer;
		}

		public byte Channel_ID
		{
			set;
			get;
		}

		//can be used to validate the protocol, by including well known separators in the stream
		public byte[] Expected_Prefix
		{
			set
			{
				expected_prefix = value;
			}
		}

		public void Receive_Calls()
		{
			while(true)
			{
				Receive_Call();
			}
		}
 		public abstract void Receive_Call();

		
		protected void On_Invalid_Method_Index(byte method_index)
		{
			throw new Exception( string.Format("Unexpected Method Index {0}", method_index) );
		}
 

		protected void Begin()
		{
			
		}
		
		protected void Flip()
		{
			
		}

		protected void End()
		{
			serializer.Flush();
		}


		protected byte[] expected_prefix;
		protected readonly Linear_Serializer serializer;
	}

	public abstract class RPC_Server_Proxy<T> : RPC_Server_Receiver
	{
		protected RPC_Server_Proxy(Linear_Serializer serializer, T obj) : base(serializer)
		{
			Debug.Assert(obj != null);
			this.obj = obj;
		}
 
		protected readonly T obj;
	}


	//a class that can be used to coordinate multiple proxies sharing the same channel
	public class RPC_Server_Proxies_Multiplexer
	{
		public RPC_Server_Proxies_Multiplexer(Linear_Serializer inSerializer)
		{
			serializer = inSerializer;
		}

		public void Add_Proxy(byte channel_id, RPC_Server_Receiver proxy)
		{
			proxies[channel_id] = proxy;
		}
		
		public RPC_Server_Receiver this[byte channel_id]
		{
			set
			{
				proxies[channel_id] = value;
			}
		}

		public void Receive_Calls()
		{
			while(true)
			{
				Receive_Call();
			}
		}
 		
		public void Receive_Call()
		{
			byte channel_id;
			serializer.Deserialize(out channel_id);
				
			proxies[channel_id].Receive_Call();
		}

		protected readonly Dictionary<byte, RPC_Server_Receiver> proxies = new Dictionary<byte,RPC_Server_Receiver>();
		protected readonly Linear_Serializer serializer;
	}
}